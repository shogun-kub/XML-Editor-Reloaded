using System.Globalization;
using System.Windows.Controls;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.App.Views;
using XmlEditorReloaded.Core.Api;
using XmlEditorReloaded.Core.Metadata;
using XmlEditorReloaded.Xml.Workspace;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the bulk edit dialog view model and window.
/// </summary>
public class BulkEditViewModelTests
{
    /// <summary>
    /// A valid preview enables Apply, lists only changed or failed rows, and produces the request.
    /// </summary>
    [Fact]
    public void Preview_EnablesApplyAndCreatesRequest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var calls = 0;
        var preview = new BulkEditPreview(
            "ubWeight",
            BulkEditOperation.Set,
            "5",
            includeMissingFields: false,
            new[]
            {
                new BulkEditPreviewRow("0", "1", "5", BulkEditRowStatus.Changed),
                new BulkEditPreviewRow("1", null, null, BulkEditRowStatus.SkippedMissing),
            });
        var context = new BulkEditContext(
            "Bulk edit",
            new[] { new BulkEditColumnOption("ubWeight", "Integer", ColumnDataType.Integer) },
            new[] { "0", "1" },
            _ =>
            {
                calls++;
                return OperationResult<BulkEditPreview>.Success(preview);
            });

        var viewModel = new BulkEditViewModel(context);

        Assert.Equal("ubWeight", viewModel.SelectedColumn!.FieldName);
        Assert.False(viewModel.CanApply);
        Assert.Null(viewModel.CreateRequest());

        viewModel.Operand = "5";

        Assert.Equal(1, calls);
        Assert.True(viewModel.CanApply);
        Assert.False(viewModel.HasError);
        Assert.Single(viewModel.PreviewRows);
        Assert.Equal("0", viewModel.PreviewRows[0].RecordKey);
        Assert.Equal("Changed", viewModel.PreviewRows[0].StatusText);
        Assert.Contains("Rows: 2", viewModel.SummaryText, StringComparison.Ordinal);

        var request = viewModel.CreateRequest();
        Assert.NotNull(request);
        Assert.Equal("ubWeight", request.FieldName);
        Assert.Equal(BulkEditOperation.Set, request.Operation);
        Assert.Equal("5", request.Operand);
        Assert.False(request.IncludeMissingFields);
        Assert.Equal(new[] { "0", "1" }, request.RecordKeys);
    }

    /// <summary>
    /// Switching from a numeric to a non-numeric column falls back to Set instead of keeping an
    /// arithmetic operation that the backend would reject.
    /// </summary>
    [Fact]
    public void SwitchingToNonNumericColumn_ResetsArithmeticToSet()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var context = new BulkEditContext(
            "Bulk edit",
            new[]
            {
                new BulkEditColumnOption("ubWeight", "Integer", ColumnDataType.Integer),
                new BulkEditColumnOption("szItemName", "String", ColumnDataType.String),
            },
            new[] { "0" },
            request => OperationResult<BulkEditPreview>.Success(new BulkEditPreview(
                request.FieldName,
                request.Operation,
                request.Operand,
                request.IncludeMissingFields,
                Array.Empty<BulkEditPreviewRow>())));

        var viewModel = new BulkEditViewModel(context);
        viewModel.SelectedOperation = Assert.Single(
            viewModel.Operations,
            option => option.Kind == BulkEditOperation.Add);
        viewModel.Operand = "1";

        viewModel.SelectedColumn = viewModel.Columns[1];

        Assert.Equal(BulkEditOperation.Set, viewModel.SelectedOperation!.Kind);
        Assert.Equal("1", viewModel.Operand);
    }

    /// <summary>
    /// A backend preview failure is shown and blocks Apply.
    /// </summary>
    [Fact]
    public void Preview_Failure_ShowsErrorAndBlocksApply()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var context = new BulkEditContext(
            "Bulk edit",
            new[] { new BulkEditColumnOption("ubWeight", "Integer", ColumnDataType.Integer) },
            new[] { "0" },
            _ => OperationResult<BulkEditPreview>.Failure(
                ErrorCodes.BulkInvalidOperand,
                "The operand is invalid."));

        var viewModel = new BulkEditViewModel(context)
        {
            Operand = "abc",
        };

        Assert.True(viewModel.HasError);
        Assert.False(viewModel.CanApply);
        Assert.Empty(viewModel.PreviewRows);
        Assert.Contains(ErrorCodes.BulkInvalidOperand, viewModel.ErrorText, StringComparison.Ordinal);
        Assert.Null(viewModel.CreateRequest());
    }

    /// <summary>
    /// The window loads its XAML, binds the view model, and sets localized headers.
    /// </summary>
    [Fact]
    public void Window_LoadsXamlAndBindsViewModel()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                CultureInfo.CurrentUICulture = new CultureInfo("en");
                var context = new BulkEditContext(
                    "Bulk edit",
                    new[] { new BulkEditColumnOption("ubWeight", "Integer", ColumnDataType.Integer) },
                    new[] { "0" },
                    _ => OperationResult<BulkEditPreview>.Success(new BulkEditPreview(
                        "ubWeight",
                        BulkEditOperation.Set,
                        "5",
                        includeMissingFields: false,
                        new[] { new BulkEditPreviewRow("0", "1", "5", BulkEditRowStatus.Changed) })));
                var viewModel = new BulkEditViewModel(context);
                var window = new BulkEditWindow(viewModel);
                var grid = Assert.IsType<DataGrid>(window.FindName("PreviewGrid"));

                Assert.Same(viewModel, window.DataContext);
                Assert.Equal("Bulk edit", window.Title);
                Assert.Equal(4, grid.Columns.Count);
                Assert.Equal("Key", grid.Columns[0].Header);
                Assert.Null(window.Request);
                window.Close();
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        })
        {
            IsBackground = true,
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The bulk edit window test did not finish in time.");
        Assert.Null(failure);
    }
}
