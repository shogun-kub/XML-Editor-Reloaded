using System.Globalization;
using XmlEditorReloaded.App.Services;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for <see cref="LookupIdViewModel"/>.
/// </summary>
public class LookupIdViewModelTests
{
    /// <summary>
    /// A free numeric id is accepted and canonicalized to invariant decimal text.
    /// </summary>
    [Fact]
    public void Accept_FreeId_ReturnsCanonicalText()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var viewModel = CreateViewModel(currentId: "1", suggestedId: "8", "1", "2", "4");

        viewModel.Id = " 0016 ";

        Assert.True(viewModel.IsValid);
        Assert.Empty(viewModel.ValidationMessage);
        Assert.True(viewModel.TryAccept());
        Assert.Equal("16", viewModel.Value);
    }

    /// <summary>
    /// An id of another value is a duplicate even when the lexical form differs, while the current id
    /// of the edited value is allowed.
    /// </summary>
    [Fact]
    public void Validation_RejectsNumericDuplicateAndAllowsCurrentId()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var viewModel = CreateViewModel(currentId: "1", suggestedId: "8", "1", "2", "4");

        viewModel.Id = "02";
        Assert.False(viewModel.IsValid);
        Assert.Contains("2", viewModel.ValidationMessage, StringComparison.Ordinal);
        Assert.False(viewModel.TryAccept());
        Assert.Null(viewModel.Value);

        viewModel.Id = "1";
        Assert.True(viewModel.IsValid);
        Assert.True(viewModel.TryAccept());
        Assert.Equal("1", viewModel.Value);
    }

    /// <summary>
    /// Empty, signed, and out-of-range text is rejected with a validation message.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("-1")]
    [InlineData("+1")]
    [InlineData("1.5")]
    [InlineData("18446744073709551616")]
    [InlineData("zero")]
    public void Validation_RejectsInvalidText(string text)
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var viewModel = CreateViewModel(currentId: "1", suggestedId: "8");

        viewModel.Id = text;

        Assert.False(viewModel.IsValid);
        Assert.NotEmpty(viewModel.ValidationMessage);
        Assert.False(viewModel.TryAccept());
    }

    /// <summary>
    /// A combination is accepted for a bit-mask lookup but warns that it is not a single bit; a single
    /// bit and zero are accepted without a warning.
    /// </summary>
    [Fact]
    public void Validation_FlagLookupWarnsAboutCombination()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var viewModel = CreateViewModel(currentId: "1", suggestedId: "32", isFlagLookup: true, "1", "2", "4", "8");

        viewModel.Id = "3";
        Assert.True(viewModel.IsValid);
        Assert.NotEmpty(viewModel.WarningMessage);
        Assert.True(viewModel.TryAccept());
        Assert.Equal("3", viewModel.Value);

        viewModel.Id = "16";
        Assert.True(viewModel.IsValid);
        Assert.Empty(viewModel.WarningMessage);

        viewModel.Id = "0";
        Assert.True(viewModel.IsValid);
        Assert.Empty(viewModel.WarningMessage);
    }

    /// <summary>
    /// The dialog summary and kind hint reflect the current id, the suggestion, and the lookup kind.
    /// </summary>
    [Fact]
    public void Texts_DescribeCurrentSuggestionAndKind()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var viewModel = CreateViewModel(currentId: "7", suggestedId: "32", isFlagLookup: true);

        Assert.Contains("7", viewModel.SummaryText, StringComparison.Ordinal);
        Assert.Contains("32", viewModel.SummaryText, StringComparison.Ordinal);
        Assert.Contains("bit", viewModel.KindHint, StringComparison.OrdinalIgnoreCase);

        var sequential = CreateViewModel(currentId: "7", suggestedId: "8");
        Assert.Contains("sequential", sequential.KindHint, StringComparison.OrdinalIgnoreCase);
    }

    private static LookupIdViewModel CreateViewModel(
        string currentId,
        string suggestedId,
        params string[] existingIds) =>
        new(new LookupIdEditRequest(
            "Lookup value identifier",
            "Lookup.Cursor",
            currentId,
            suggestedId,
            IsFlagLookup: false,
            existingIds));

    private static LookupIdViewModel CreateViewModel(
        string currentId,
        string suggestedId,
        bool isFlagLookup,
        params string[] existingIds) =>
        new(new LookupIdEditRequest(
            "Lookup value identifier",
            "Lookup.AttachmentPoint",
            currentId,
            suggestedId,
            isFlagLookup,
            existingIds));
}
