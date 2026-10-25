using System.Globalization;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.Core.Abstractions;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for lookup display data and its handling of unknown values and load failures.
/// </summary>
public class LookupColumnDataTests
{
    /// <summary>
    /// Known entries keep their names, while unknown values are appended once and marked explicitly.
    /// </summary>
    [Fact]
    public void Create_MergesKnownAndUnknownValues()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var data = LookupColumnData.Create(
            TableName.Parse("Lookup.Test"),
            new[] { new LookupEntry(1, "One"), new LookupEntry(2, "Two") },
            new string?[] { "2", "99", "abc", null, "99", "1" });

        Assert.True(data.IsAvailable);
        Assert.Equal(4, data.Options.Count);
        Assert.Equal(new[] { "1", "2", "99", "abc" }, data.Options.Select(option => option.IdText));
        Assert.Equal("Two", data.Display("2"));
        Assert.Equal("99 (unknown)", data.Display("99"));
        Assert.Equal("abc (unknown)", data.Display("abc"));
        Assert.Equal(string.Empty, data.Display(null));
        Assert.Equal("Two", data.GetName("2"));
        Assert.Null(data.GetName("99"));
        Assert.Equal("Two (ID 2)", data.GetTooltip("2"));
        Assert.Equal("Unknown lookup value: 99", data.GetTooltip("99"));
        Assert.Equal("One", data.GetExactEntryName("1"));
        Assert.Null(data.GetExactEntryName("3"));
    }

    /// <summary>
    /// Identifiers above the signed 64-bit range are known entries and are not confused with
    /// neighbouring unknown values.
    /// </summary>
    [Fact]
    public void Create_HandlesIdsAboveSignedRange()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var data = LookupColumnData.Create(
            TableName.Parse("Lookup.Test"),
            new[] { new LookupEntry(ulong.MaxValue, "Huge"), new LookupEntry(2, "Two") },
            new string?[] { "18446744073709551615", "18446744073709551614" });

        Assert.True(data.IsAvailable);
        Assert.Equal("Huge", data.GetExactEntryName("18446744073709551615"));
        Assert.Equal("Huge (ID 18446744073709551615)", data.GetTooltip("18446744073709551615"));
        Assert.Null(data.GetName("18446744073709551614"));
        Assert.Equal("18446744073709551614 (unknown)", data.Display("18446744073709551614"));
        Assert.Null(data.GetExactEntryName("18446744073709551614"));
        Assert.Equal(3, data.Options.Count);
    }

    /// <summary>
    /// A failed lookup still shows raw values and exposes the error instead of pretending that every
    /// value is unknown.
    /// </summary>
    [Fact]
    public void Failed_KeepsRawTextAndReportsError()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var data = LookupColumnData.Failed(
            TableName.Parse("Lookup.Missing"),
            "io.file_not_found",
            "The lookup file does not exist.");

        Assert.False(data.IsAvailable);
        Assert.Empty(data.Options);
        Assert.Equal("42", data.Display("42"));
        Assert.Contains("does not exist", data.GetTooltip("42"), StringComparison.Ordinal);
        Assert.Null(data.GetName("42"));
    }
}
