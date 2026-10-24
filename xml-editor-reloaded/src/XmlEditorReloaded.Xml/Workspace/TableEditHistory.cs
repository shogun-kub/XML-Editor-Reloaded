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
/// The state of one XML element before and after an edit. The element is replaced with a clone on
/// undo and redo, which preserves sparse fields, comments, and lexical value forms.
/// </summary>
internal sealed class ElementSnapshotState
{
    private ElementSnapshotState(
        TableDocument document,
        XElement parent,
        int index,
        XElement? current,
        XElement? before,
        XElement? after)
    {
        Document = document;
        Parent = parent;
        Index = index;
        Current = current;
        Before = before;
        After = after;
    }

    public TableDocument Document { get; }

    public XElement Parent { get; }

    public int Index { get; }

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
            element,
            null,
            new XElement(element));
    }

    public void CompleteAfter(XElement element)
    {
        Current = element;
        After = new XElement(element);
    }

    public void MarkRemoved()
    {
        Current = null;
        After = null;
    }

    public void Undo() => Apply(Before);

    public void Redo() => Apply(After);

    private static int ElementIndex(XElement parent, XElement element) =>
        parent.Elements().TakeWhile(candidate => candidate != element).Count();

    private void Apply(XElement? snapshot)
    {
        if (snapshot is null)
        {
            if (Current is not null && Current.Parent == Parent)
            {
                Current.Remove();
            }

            Current = null;
            return;
        }

        var replacement = new XElement(snapshot);
        if (Current is not null && Current.Parent == Parent)
        {
            Current.ReplaceWith(replacement);
        }
        else
        {
            var reference = Parent.Elements().ElementAtOrDefault(Index);
            if (reference is null)
            {
                Parent.Add(replacement);
            }
            else
            {
                reference.AddBeforeSelf(replacement);
            }
        }

        Current = replacement;
    }
}
