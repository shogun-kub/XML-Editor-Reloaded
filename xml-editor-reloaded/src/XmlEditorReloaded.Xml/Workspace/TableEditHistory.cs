using System.Xml.Linq;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Xml.Documents;

namespace XmlEditorReloaded.Xml.Workspace;

/// <summary>
/// A bounded undo and redo history for record-level edits of a <see cref="TableWorkspace"/>.
/// Each entry stores the affected record elements before and after one user action, so undoing a
/// bulk edit restores every touched value in one step.
/// </summary>
/// <remarks>
/// The history is not thread-safe. It is intended to be used from the thread that owns the workspace.
/// </remarks>
public sealed class TableEditHistory
{
    private const int MaximumTransactions = 50;
    private const int MaximumUndoStates = 10_000;
    private readonly List<TableEditTransaction> _undo = new();
    private readonly List<TableEditTransaction> _redo = new();
    private int _undoStates;

    /// <summary>
    /// Gets a value indicating whether at least one edit can be undone.
    /// </summary>
    public bool CanUndo => _undo.Count > 0;

    /// <summary>
    /// Gets a value indicating whether at least one undone edit can be redone.
    /// </summary>
    public bool CanRedo => _redo.Count > 0;

    /// <summary>
    /// Gets the number of edits that can be undone.
    /// </summary>
    public int UndoCount => _undo.Count;

    /// <summary>
    /// Gets the number of edits that can be redone.
    /// </summary>
    public int RedoCount => _redo.Count;

    /// <summary>
    /// Occurs after an edit is recorded, undone, redone, or the history is cleared.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>
    /// Restores the most recently undone edit.
    /// </summary>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>history.empty</c> when there is nothing to redo.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a recorded element is no longer attached to its document, which indicates that
    /// the document was modified outside the workspace.
    /// </exception>
    public OperationResult Redo()
    {
        if (_redo.Count == 0)
        {
            return OperationResult.Failure(ErrorCodes.HistoryEmpty, "There is no edit to redo.");
        }

        var transaction = _redo[^1];
        transaction.Redo();
        transaction.RefreshDocuments();
        _redo.RemoveAt(_redo.Count - 1);
        _undo.Add(transaction);
        _undoStates += transaction.SnapshotCount;
        Changed?.Invoke(this, EventArgs.Empty);
        return OperationResult.Success();
    }

    /// <summary>
    /// Reverses the most recent edit.
    /// </summary>
    /// <returns>
    /// On success, <c>error_code</c> is <see langword="null"/>.
    /// On failure, <c>error_code</c> is <c>history.empty</c> when there is nothing to undo.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a recorded element is no longer attached to its document, which indicates that
    /// the document was modified outside the workspace.
    /// </exception>
    public OperationResult Undo()
    {
        if (_undo.Count == 0)
        {
            return OperationResult.Failure(ErrorCodes.HistoryEmpty, "There is no edit to undo.");
        }

        var transaction = _undo[^1];
        transaction.Undo();
        transaction.RefreshDocuments();
        _undo.RemoveAt(_undo.Count - 1);
        _undoStates -= transaction.SnapshotCount;
        _redo.Add(transaction);
        Changed?.Invoke(this, EventArgs.Empty);
        return OperationResult.Success();
    }

    /// <summary>
    /// Removes all undo and redo entries.
    /// </summary>
    public void Clear()
    {
        if (_undo.Count == 0 && _redo.Count == 0)
        {
            return;
        }

        _undo.Clear();
        _redo.Clear();
        _undoStates = 0;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    internal void Record(TableEditTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        if (transaction.IsEmpty)
        {
            return;
        }

        _undo.Add(transaction);
        _undoStates += transaction.SnapshotCount;
        while (_undo.Count > 1
               && (_undo.Count > MaximumTransactions || _undoStates > MaximumUndoStates))
        {
            _undoStates -= _undo[0].SnapshotCount;
            _undo.RemoveAt(0);
        }

        _redo.Clear();
        Changed?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// One user action that can be reversed and reapplied as a unit.
/// </summary>
internal sealed class TableEditTransaction
{
    private readonly List<ElementSnapshotState> _states = new();

    public bool IsEmpty => _states.Count == 0;

    public int SnapshotCount => _states.Count;

    public IEnumerable<TableDocument> Documents =>
        _states.Select(static state => state.Document).Distinct();

    public void Add(ElementSnapshotState? state)
    {
        if (state is { IsChanged: true })
        {
            _states.Add(state);
        }
    }

    public void Undo()
    {
        for (var index = _states.Count - 1; index >= 0; index--)
        {
            _states[index].Undo();
        }
    }

    public void Redo()
    {
        foreach (var state in _states)
        {
            state.Redo();
        }
    }

    public void RefreshDocuments()
    {
        foreach (var document in Documents)
        {
            document.SynchronizeRecords();
            document.RecomputeDirty();
        }
    }
}

/// <summary>
/// The state of one XML element before and after an edit. Undo and redo restore the recorded content
/// in place and keep the same <see cref="XElement"/> object attached to the document. Preserving the
/// object identity matters because several snapshots can refer to one element: an edit of a row that
/// was just added shares its element with the insertion snapshot. Replacing the element with a clone
/// would leave the older snapshot pointing to a detached object, so undoing the insertion would
/// silently do nothing and a later redo could add a duplicate row.
/// </summary>
internal sealed class ElementSnapshotState
{
    private ElementSnapshotState(
        TableDocument document,
        XElement parent,
        int index,
        XNode? previousNode,
        XNode? nextNode,
        XElement? current,
        XElement? before,
        XElement? after)
    {
        Document = document;
        Parent = parent;
        Index = index;
        PreviousNode = previousNode;
        NextNode = nextNode;
        Current = current;
        Before = before;
        After = after;
    }

    public TableDocument Document { get; }

    public XElement Parent { get; }

    public int Index { get; }

    /// <summary>
    /// Gets the sibling node that preceded the element when the snapshot was captured. Restoring next to
    /// this node keeps formatting whitespace in its original node order, which keeps an undone edit
    /// deep-equal to the saved baseline.
    /// </summary>
    public XNode? PreviousNode { get; }

    /// <summary>
    /// Gets the sibling node that followed the element when the snapshot was captured, used as a
    /// fallback when the previous node was removed by the same edit.
    /// </summary>
    public XNode? NextNode { get; }

    public XElement? Current { get; private set; }

    public XElement? Before { get; }

    public XElement? After { get; private set; }

    public bool IsChanged => Before is null
        ? After is not null
        : After is null || !XNode.DeepEquals(Before, After);

    public static ElementSnapshotState Capture(TableDocument document, XElement element)
    {
        var parent = element.Parent
            ?? throw new InvalidOperationException("The edited element is not attached to a document.");
        return new ElementSnapshotState(
            document,
            parent,
            ElementIndex(parent, element),
            element.PreviousNode,
            element.NextNode,
            element,
            new XElement(element),
            null);
    }

    public static ElementSnapshotState CaptureInsertion(TableDocument document, XElement element)
    {
        var parent = element.Parent
            ?? throw new InvalidOperationException("The inserted element is not attached to a document.");
        return new ElementSnapshotState(
            document,
            parent,
            ElementIndex(parent, element),
            element.PreviousNode,
            element.NextNode,
            element,
            null,
            new XElement(element));
    }

    public void CompleteAfter(XElement element)
    {
        Current = element;
        After = new XElement(element);
    }

    /// <summary>
    /// Marks the element as deleted while keeping the detached object. The object is reattached on
    /// undo and removed again on redo, so snapshots that reference the same element stay valid.
    /// </summary>
    public void MarkRemoved()
    {
        After = null;
    }

    public void Undo() => Apply(Before);

    public void Redo() => Apply(After);

    private static int ElementIndex(XElement parent, XElement element) =>
        parent.Elements().TakeWhile(candidate => candidate != element).Count();

    /// <summary>
    /// Restores one recorded state. A non-null state always updates the existing element's content;
    /// the element is reattached only when the previous operation detached it. A null state detaches
    /// the element but keeps the object, which is what redo of a deletion or undo of an insertion needs.
    /// </summary>
    /// <param name="snapshot">Recorded element content, or <see langword="null"/> for a removed element.</param>
    private void Apply(XElement? snapshot)
    {
        if (snapshot is null)
        {
            if (Current is { } removed && removed.Parent == Parent)
            {
                removed.Remove();
            }

            return;
        }

        if (Current is { } current)
        {
            ApplyContent(current, snapshot);
            if (current.Parent != Parent)
            {
                if (current.Parent is not null)
                {
                    current.Remove();
                }

                Attach(current);
            }

            return;
        }

        var replacement = new XElement(snapshot);
        Attach(replacement);
        Current = replacement;
    }

    /// <summary>
    /// Copies the recorded attributes and child nodes into the existing element. Nodes come from a
    /// temporary clone, so the snapshot stays intact for repeated undo and redo calls.
    /// </summary>
    private static void ApplyContent(XElement target, XElement snapshot)
    {
        if (target.Name != snapshot.Name)
        {
            target.Name = snapshot.Name;
        }

        var content = new XElement(snapshot);
        target.ReplaceAttributes(content.Attributes());
        target.ReplaceNodes(content.Nodes());
    }

    /// <summary>
    /// Reattaches a detached element next to its recorded neighbour, or at its recorded element index
    /// when both neighbours were removed by later operations.
    /// </summary>
    private void Attach(XElement element)
    {
        if (PreviousNode is { } previous && previous.Parent == Parent)
        {
            previous.AddAfterSelf(element);
            return;
        }

        if (NextNode is { } next && next.Parent == Parent)
        {
            next.AddBeforeSelf(element);
            return;
        }

        var reference = Parent.Elements().ElementAtOrDefault(Index);
        if (reference is null || ReferenceEquals(reference, element))
        {
            Parent.Add(element);
        }
        else
        {
            reference.AddBeforeSelf(element);
        }
    }
}
