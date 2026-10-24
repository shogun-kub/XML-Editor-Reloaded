using System.Globalization;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Localization;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.Xml.Tests;

/// <summary>
/// Tests for record-level undo and redo and for bulk column operations.
/// </summary>
public class UndoAndBulkTests
{
    /// <summary>
    /// A scalar edit can be undone back to a clean document and redone.
    /// </summary>
    [Fact]
    public async Task UndoRedo_BaseCellEdit_RestoresValueAndDirtyState()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        Assert.True(table.SetValue("1", "szItemName", "Glock 18").IsSuccess);
        Assert.True(table.IsDirty);
        Assert.True(table.CanUndo);

        Assert.True(table.Undo().IsSuccess);
        Assert.Equal(
            "Glock 17",
            Assert.Single(table.GetRows(), row => row.Key == "1").GetValue("szItemName"));
        Assert.False(table.IsDirty);
        Assert.False(table.CanUndo);
        Assert.True(table.CanRedo);

        Assert.True(table.Redo().IsSuccess);
        Assert.Equal(
            "Glock 18",
            Assert.Single(table.GetRows(), row => row.Key == "1").GetValue("szItemName"));
        Assert.True(table.IsDirty);
        Assert.True(table.CanUndo);
        Assert.False(table.CanRedo);
    }

    /// <summary>
    /// Making a new edit after undo discards the redo branch.
    /// </summary>
    [Fact]
    public async Task Undo_NewEditAfterUndo_ClearsRedo()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        Assert.True(table.SetValue("1", "szItemName", "First").IsSuccess);
        Assert.True(table.Undo().IsSuccess);
        Assert.True(table.CanRedo);

        Assert.True(table.SetValue("1", "szItemName", "Second").IsSuccess);
        Assert.False(table.CanRedo);
        Assert.Equal(
            "Second",
            Assert.Single(table.GetRows(), row => row.Key == "1").GetValue("szItemName"));

        Assert.True(table.Undo().IsSuccess);
        Assert.Equal(
            "Glock 17",
            Assert.Single(table.GetRows(), row => row.Key == "1").GetValue("szItemName"));
    }

    /// <summary>
    /// Undoing a saved scalar edit and saving again restores the original bytes, because the
    /// lossless patcher rewrites only the changed value.
    /// </summary>
    [Fact]
    public async Task Undo_AfterSave_RestoresOriginalBytesAndDirtyState()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);
        var path = table.Descriptor.FilePath;
        var originalBytes = await File.ReadAllBytesAsync(path);

        Assert.True(table.SetValue("1", "szItemName", "Glock 18").IsSuccess);
        Assert.True((await table.SaveAsync()).IsSuccess);
        var savedBytes = await File.ReadAllBytesAsync(path);
        Assert.NotEqual(originalBytes, savedBytes);
        Assert.False(table.IsDirty);

        Assert.True(table.Undo().IsSuccess);
        Assert.True(table.IsDirty);
        Assert.True(table.CanRedo);
        Assert.Equal("Glock 17", table.GetRow("1")!.GetValue("szItemName"));

        Assert.True((await table.SaveAsync()).IsSuccess);
        Assert.Equal(originalBytes, await File.ReadAllBytesAsync(path));
        Assert.False(table.IsDirty);
    }

    /// <summary>
    /// Undoing the addition of a sparse field removes it again, and redo restores it.
    /// </summary>
    [Fact]
    public async Task UndoRedo_AddedField_RemovesAndRestoresIt()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        Assert.True(table.SetValue("0", "ProjectionFactor", "1.5", addIfMissing: true).IsSuccess);
        Assert.Equal("1.5", table.BaseDocument.Records[0].GetRawValue("ProjectionFactor"));

        Assert.True(table.Undo().IsSuccess);
        Assert.False(table.BaseDocument.Records[0].HasField("ProjectionFactor"));
        Assert.False(table.IsDirty);

        Assert.True(table.Redo().IsSuccess);
        Assert.Equal("1.5", table.BaseDocument.Records[0].GetRawValue("ProjectionFactor"));
    }

    /// <summary>
    /// Adding, duplicating, and deleting records can each be undone and redone in order.
    /// </summary>
    [Fact]
    public async Task UndoRedo_RecordOperations_RestoreOrderAndKeys()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        var added = table.AddRecord();
        Assert.True(added.IsSuccess, added.ErrorText);
        Assert.Equal("2", added.Data!.GetKey());
        Assert.Equal(3, table.BaseDocument.Records.Count);

        Assert.True(table.Undo().IsSuccess);
        Assert.Equal(2, table.BaseDocument.Records.Count);
        Assert.Null(table.BaseDocument.FindRecord("2"));

        Assert.True(table.Redo().IsSuccess);
        Assert.Equal("2", table.BaseDocument.Records[2].GetKey());

        var duplicate = table.DuplicateRecord("1");
        Assert.True(duplicate.IsSuccess, duplicate.ErrorText);
        Assert.Equal("3", duplicate.Data!.GetKey());
        Assert.Equal(4, table.BaseDocument.Records.Count);

        Assert.True(table.Undo().IsSuccess);
        Assert.Equal(3, table.BaseDocument.Records.Count);
        Assert.Equal(new[] { "0", "1", "2" }, table.BaseDocument.Records.Select(record => record.GetKey()));

        Assert.True(table.DeleteRecord("0").IsSuccess);
        Assert.Equal(new[] { "1", "2" }, table.BaseDocument.Records.Select(record => record.GetKey()));

        Assert.True(table.Undo().IsSuccess);
        Assert.Equal(new[] { "0", "1", "2" }, table.BaseDocument.Records.Select(record => record.GetKey()));
        Assert.Equal("Nothing", table.BaseDocument.Records[0].GetRawValue("szItemName"));
        Assert.Equal("keep me", table.BaseDocument.Records[0].GetRawValue("CustomUnknown"));

        Assert.True(table.Redo().IsSuccess);
        Assert.Equal(new[] { "1", "2" }, table.BaseDocument.Records.Select(record => record.GetKey()));
    }

    /// <summary>
    /// A new translation record is removed by undo, and editing an existing translation is reversible.
    /// </summary>
    [Fact]
    public async Task UndoRedo_OverlayTranslations_RestoreOverlayState()
    {
        using var fixture = new TestWorkspace();
        var (table, _, _) = await LoadOverlayAsync(fixture);

        Assert.True(table.SetValue("0", "szItemName", "Ничто").IsSuccess);
        Assert.NotNull(table.Overlay!.Document.FindRecord("0"));
        Assert.Equal("Ничто", table.GetRow("0")!.GetValue("szItemName"));

        Assert.True(table.Undo().IsSuccess);
        Assert.Null(table.Overlay.Document.FindRecord("0"));
        Assert.Equal("Nothing", table.GetRow("0")!.GetValue("szItemName"));
        Assert.False(table.IsDirty);

        Assert.True(table.Redo().IsSuccess);
        Assert.Equal("Ничто", table.GetRow("0")!.GetValue("szItemName"));

        Assert.True(table.SetValue("1", "szItemName", "Глок 18").IsSuccess);
        Assert.Equal("Глок 18", table.GetRow("1")!.GetValue("szItemName"));

        Assert.True(table.Undo().IsSuccess);
        Assert.Equal("Глок 17", table.GetRow("1")!.GetValue("szItemName"));
    }

    /// <summary>
    /// Deleting a record with a translation removes both copies, and undo restores both.
    /// </summary>
    [Fact]
    public async Task UndoRedo_DeleteRecordWithOverlay_RestoresBaseAndOverlay()
    {
        using var fixture = new TestWorkspace();
        var (table, _, _) = await LoadOverlayAsync(fixture);
        var overlayCount = table.Overlay!.Document.Records.Count;

        Assert.True(table.DeleteRecord("1").IsSuccess);
        Assert.Null(table.BaseDocument.FindRecord("1"));
        Assert.Null(table.Overlay.Document.FindRecord("1"));
        Assert.Equal(overlayCount - 1, table.Overlay.Document.Records.Count);

        Assert.True(table.Undo().IsSuccess);
        Assert.NotNull(table.BaseDocument.FindRecord("1"));
        Assert.NotNull(table.Overlay.Document.FindRecord("1"));
        Assert.Equal("Глок 17", table.GetRow("1")!.GetValue("szItemName"));
        Assert.Equal(overlayCount, table.Overlay.Document.Records.Count);

        Assert.True(table.Redo().IsSuccess);
        Assert.Null(table.BaseDocument.FindRecord("1"));
        Assert.Null(table.Overlay.Document.FindRecord("1"));
    }

    /// <summary>
    /// By default, bulk set changes only fields that already exist.
    /// </summary>
    [Fact]
    public async Task BulkSet_DefaultSkipsMissingFields()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);
        var request = new BulkEditRequest(
            "ProjectionFactor",
            BulkEditOperation.Set,
            "2.5",
            IncludeMissingFields: false,
            new[] { "0", "1" });

        var previewResult = table.PreviewBulk(request);
        Assert.True(previewResult.IsSuccess, previewResult.ErrorText);
        var preview = previewResult.Data!;
        Assert.Equal(1, preview.ChangedCount);
        Assert.Equal(1, preview.SkippedCount);
        Assert.True(preview.CanApply);
        Assert.False(table.IsDirty);

        var applied = table.ApplyBulk(request);
        Assert.True(applied.IsSuccess, applied.ErrorText);
        Assert.Equal("2.5", table.BaseDocument.Records[1].GetRawValue("ProjectionFactor"));
        Assert.False(table.BaseDocument.Records[0].HasField("ProjectionFactor"));
        Assert.True(table.IsDirty);

        Assert.True(table.Undo().IsSuccess);
        Assert.Equal("1.0", table.BaseDocument.Records[1].GetRawValue("ProjectionFactor"));
        Assert.False(table.BaseDocument.Records[0].HasField("ProjectionFactor"));
        Assert.False(table.IsDirty);
    }

    /// <summary>
    /// The explicit include-missing mode creates absent fields and one undo restores all of them.
    /// </summary>
    [Fact]
    public async Task BulkSet_IncludeMissing_CreatesFieldsAndUndoesAsOneStep()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);
        var request = new BulkEditRequest(
            "ProjectionFactor",
            BulkEditOperation.Set,
            "2.5",
            IncludeMissingFields: true,
            new[] { "0", "1" });

        var preview = table.PreviewBulk(request);
        Assert.True(preview.IsSuccess, preview.ErrorText);
        Assert.Equal(2, preview.Data!.ChangedCount);
        Assert.Equal(0, preview.Data.SkippedCount);

        Assert.True(table.ApplyBulk(request).IsSuccess);
        Assert.Equal("2.5", table.BaseDocument.Records[0].GetRawValue("ProjectionFactor"));
        Assert.Equal("2.5", table.BaseDocument.Records[1].GetRawValue("ProjectionFactor"));

        Assert.True(table.Undo().IsSuccess);
        Assert.False(table.BaseDocument.Records[0].HasField("ProjectionFactor"));
        Assert.Equal("1.0", table.BaseDocument.Records[1].GetRawValue("ProjectionFactor"));
    }

    /// <summary>
    /// Integer and decimal arithmetic use invariant culture and preserve the decimal scale.
    /// </summary>
    [Fact]
    public async Task BulkArithmetic_AddAndMultiply_UseInvariantNumbers()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        var addWeight = new BulkEditRequest(
            "ubWeight",
            BulkEditOperation.Add,
            "5",
            IncludeMissingFields: false,
            new[] { "0", "1" });
        Assert.True(table.ApplyBulk(addWeight).IsSuccess);
        Assert.Equal("5", table.BaseDocument.Records[0].GetRawValue("ubWeight"));
        Assert.Equal("15", table.BaseDocument.Records[1].GetRawValue("ubWeight"));
        Assert.True(table.Undo().IsSuccess);
        Assert.Equal("0", table.BaseDocument.Records[0].GetRawValue("ubWeight"));
        Assert.Equal("10", table.BaseDocument.Records[1].GetRawValue("ubWeight"));

        var addFactor = new BulkEditRequest(
            "ProjectionFactor",
            BulkEditOperation.Add,
            "0.1",
            IncludeMissingFields: false,
            new[] { "1" });
        Assert.True(table.ApplyBulk(addFactor).IsSuccess);
        Assert.Equal("1.1", table.BaseDocument.Records[1].GetRawValue("ProjectionFactor"));

        var multiply = new BulkEditRequest(
            "ubWeight",
            BulkEditOperation.Multiply,
            "2",
            IncludeMissingFields: false,
            new[] { "0", "1" });
        Assert.True(table.ApplyBulk(multiply).IsSuccess);
        Assert.Equal("0", table.BaseDocument.Records[0].GetRawValue("ubWeight"));
        Assert.Equal("20", table.BaseDocument.Records[1].GetRawValue("ubWeight"));
    }

    /// <summary>
    /// Arithmetic rejects a comma decimal separator and an operand outside the target type.
    /// </summary>
    [Fact]
    public async Task BulkArithmetic_InvalidOperands_AreRejected()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        var comma = table.PreviewBulk(new BulkEditRequest(
            "ubWeight",
            BulkEditOperation.Add,
            "1,5",
            IncludeMissingFields: false,
            new[] { "0" }));
        Assert.False(comma.IsSuccess);
        Assert.Equal(ErrorCodes.BulkInvalidOperand, comma.ErrorCode);

        var stringMath = table.PreviewBulk(new BulkEditRequest(
            "szItemName",
            BulkEditOperation.Add,
            "1",
            IncludeMissingFields: false,
            new[] { "0" }));
        Assert.False(stringMath.IsSuccess);
        Assert.Equal(ErrorCodes.BulkUnsupportedOperation, stringMath.ErrorCode);

        var overflowRequest = new BulkEditRequest(
            "ubWeight",
            BulkEditOperation.Add,
            "2147483647",
            IncludeMissingFields: false,
            new[] { "0", "1" });
        var overflow = table.PreviewBulk(overflowRequest);
        Assert.True(overflow.IsSuccess, overflow.ErrorText);
        Assert.Equal(1, overflow.Data!.ErrorCount);
        Assert.False(overflow.Data.CanApply);
        var apply = table.ApplyBulk(overflowRequest);
        Assert.False(apply.IsSuccess);
        Assert.Equal(ErrorCodes.BulkPreviewHasErrors, apply.ErrorCode);
        Assert.Equal("0", table.BaseDocument.Records[0].GetRawValue("ubWeight"));
        Assert.Equal("10", table.BaseDocument.Records[1].GetRawValue("ubWeight"));
    }

    /// <summary>
    /// A bulk translation edit writes only the overlay and is undone as one transaction.
    /// </summary>
    [Fact]
    public async Task BulkSet_Translation_WritesOnlyOverlayAndUndoes()
    {
        using var fixture = new TestWorkspace();
        var (table, basePath, _) = await LoadOverlayAsync(fixture);
        var baseBefore = await File.ReadAllBytesAsync(basePath);
        var request = new BulkEditRequest(
            "szItemName",
            BulkEditOperation.Set,
            "Перевод",
            IncludeMissingFields: true,
            new[] { "0", "1" });

        Assert.True(table.ApplyBulk(request).IsSuccess);
        Assert.Equal(baseBefore, await File.ReadAllBytesAsync(basePath));
        Assert.Equal("Перевод", table.GetRow("0")!.GetValue("szItemName"));
        Assert.Equal("Перевод", table.GetRow("1")!.GetValue("szItemName"));
        Assert.Equal(
            "Перевод",
            Assert.Single(table.Overlay!.Document.Records, record => record.GetKey() == "0")
                .GetRawValue("szItemName"));
        Assert.Equal(
            "Перевод",
            Assert.Single(table.Overlay.Document.Records, record => record.GetKey() == "1")
                .GetRawValue("szItemName"));

        Assert.True(table.Undo().IsSuccess);
        Assert.Equal("Nothing", table.GetRow("0")!.GetValue("szItemName"));
        Assert.Equal("Глок 17", table.GetRow("1")!.GetValue("szItemName"));
        Assert.Null(table.Overlay.Document.FindRecord("0"));
        Assert.Equal("Глок 17", table.Overlay.Document.FindRecord("1")!.GetRawValue("szItemName"));
        Assert.False(table.IsDirty);
    }

    /// <summary>
    /// Bulk keys are processed once each and an unknown key blocks application.
    /// </summary>
    [Fact]
    public async Task BulkPreview_DuplicateAndUnknownKeys_AreHandled()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);

        var duplicate = table.PreviewBulk(new BulkEditRequest(
            "ubWeight",
            BulkEditOperation.Set,
            "7",
            IncludeMissingFields: false,
            new[] { "0", "0", "1" }));
        Assert.True(duplicate.IsSuccess);
        Assert.Equal(2, duplicate.Data!.Rows.Count);
        Assert.Equal(2, duplicate.Data.ChangedCount);

        var unknown = table.PreviewBulk(new BulkEditRequest(
            "ubWeight",
            BulkEditOperation.Set,
            "7",
            IncludeMissingFields: false,
            new[] { "0", "99" }));
        Assert.True(unknown.IsSuccess);
        Assert.Equal(1, unknown.Data!.ErrorCount);
        Assert.False(unknown.Data.CanApply);
        Assert.Equal(ErrorCodes.BulkPreviewHasErrors, table.ApplyBulk(new BulkEditRequest(
            "ubWeight",
            BulkEditOperation.Set,
            "7",
            IncludeMissingFields: false,
            new[] { "0", "99" })).ErrorCode);
    }

    /// <summary>
    /// Keys, repeated fields, and unknown columns are refused before any preview row is computed.
    /// </summary>
    [Fact]
    public async Task BulkPreview_NonEditableAndUnknownColumns_AreRejected()
    {
        using var fixture = new TestWorkspace();
        var table = await LoadItemsAsync(fixture);
        var keys = new[] { "1" };

        var key = table.PreviewBulk(new BulkEditRequest(
            "uiIndex",
            BulkEditOperation.Set,
            "7",
            IncludeMissingFields: false,
            keys));
        Assert.False(key.IsSuccess);
        Assert.Equal(ErrorCodes.BulkFieldNotEditable, key.ErrorCode);

        var list = table.PreviewBulk(new BulkEditRequest(
            "AvailableAttachmentPoint",
            BulkEditOperation.Set,
            "7",
            IncludeMissingFields: false,
            keys));
        Assert.False(list.IsSuccess);
        Assert.Equal(ErrorCodes.BulkFieldNotEditable, list.ErrorCode);

        var unknown = table.PreviewBulk(new BulkEditRequest(
            "NoSuchField",
            BulkEditOperation.Set,
            "7",
            IncludeMissingFields: false,
            keys));
        Assert.False(unknown.IsSuccess);
        Assert.Equal(ErrorCodes.BulkFieldNotFound, unknown.ErrorCode);

        var empty = table.PreviewBulk(new BulkEditRequest(
            "ubWeight",
            BulkEditOperation.Set,
            "7",
            IncludeMissingFields: false,
            Array.Empty<string>()));
        Assert.False(empty.IsSuccess);
        Assert.Equal(ErrorCodes.BulkNoRecords, empty.ErrorCode);
    }

    /// <summary>
    /// Bulk arithmetic interprets numbers independently of the current culture.
    /// </summary>
    [Fact]
    public async Task BulkArithmetic_IsCultureIndependent()
    {
        var previous = CultureInfo.CurrentCulture;
        var previousUi = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            CultureInfo.CurrentUICulture = new CultureInfo("ru-RU");
            using var fixture = new TestWorkspace();
            var table = await LoadItemsAsync(fixture);
            var request = new BulkEditRequest(
                "ProjectionFactor",
                BulkEditOperation.Add,
                "0.5",
                IncludeMissingFields: false,
                new[] { "1" });

            Assert.True(table.ApplyBulk(request).IsSuccess);
            Assert.Equal("1.5", table.BaseDocument.Records[1].GetRawValue("ProjectionFactor"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
            CultureInfo.CurrentUICulture = previousUi;
        }
    }

    private static async Task<TableWorkspace> LoadItemsAsync(TestWorkspace fixture)
    {
        var path = fixture.CopyFixture("items-mini.xml");
        var descriptor = new TableDescriptor(
            TableName.Parse("Items.Items"),
            path,
            "Items/Items.xml",
            IsMapped: true,
            Array.Empty<OverlayDescriptor>());
        var load = await TableWorkspace.LoadAsync(descriptor, OverlayTestData.ItemsMetadata());
        Assert.True(load.IsSuccess, load.ErrorText);
        return load.Data!;
    }

    private static async Task<(TableWorkspace Table, string BasePath, string OverlayPath)> LoadOverlayAsync(
        TestWorkspace fixture)
    {
        var basePath = fixture.CopyFixture("Overlays/items-base.xml");
        var overlayPath = fixture.CopyFixture("Overlays/items-ru.xml");
        var metadata = OverlayTestData.ItemsMetadata();
        var descriptor = new TableDescriptor(
            metadata.TableName,
            basePath,
            "Items/Items.xml",
            IsMapped: true,
            new[]
            {
                new OverlayDescriptor(OverlayTestData.Russian, basePath, overlayPath),
            });
        var load = await TableWorkspace.LoadAsync(descriptor, metadata, OverlayTestData.Russian);
        Assert.True(load.IsSuccess, load.ErrorText);
        return (load.Data!, basePath, overlayPath);
    }
}
