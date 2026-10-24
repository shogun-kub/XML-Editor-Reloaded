using System.Globalization;
using System.Windows.Controls;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.App.Views;
using XmlEditorReloaded.Core.Metadata;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the searchable relation picker control.
/// </summary>
public class RelationPickerTests
{
    /// <summary>
    /// The picker shows the target name, keeps the bound value as the raw key, marks unknown keys, and
    /// honours the read-only state.
    /// </summary>
    [Fact]
    public void Picker_ShowsDisplayTextAndKeepsRawValue()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");
        var data = RelationColumnData.Create(
            CreateRelation(),
            CreateTarget(),
            new string?[] { "99" });
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                var picker = new RelationPicker
                {
                    RelationData = data,
                    SelectedValue = "1",
                };
                var box = Assert.IsType<TextBox>(picker.FindName("ValueBox"));
                Assert.Equal("Glock 17", box.Text);

                picker.SelectedValue = "99";
                Assert.Equal("99 (unknown)", box.Text);

                picker.SelectedValue = "1";
                picker.IsReadOnly = true;
                Assert.True(box.IsReadOnly);
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

        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The relation picker test did not finish in time.");
        Assert.Null(failure);
    }

    private static TableRelation CreateRelation() =>
        new(
            TableName.Parse("Items.Items"),
            "DefaultAttachment",
            TableName.Parse("Items.Items"),
            TableRelationKind.Reference,
            "uiIndex",
            "szLongItemName");

    private static RelationTarget CreateTarget()
    {
        var all = new[]
        {
            new RelationTargetEntry("0", "Nothing"),
            new RelationTargetEntry("1", "Glock 17"),
        };
        return new RelationTarget(
            TableName.Parse("Items.Items"),
            "uiIndex",
            "szLongItemName",
            all,
            all);
    }
}
