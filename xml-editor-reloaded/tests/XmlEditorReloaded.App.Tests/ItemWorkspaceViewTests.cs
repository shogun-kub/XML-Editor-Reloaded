using System.Globalization;
using XmlEditorReloaded.App.Views;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Smoke tests that the item workspace and shared field editor XAML load on an STA thread.
/// </summary>
public class ItemWorkspaceViewTests
{
    /// <summary>
    /// The item workspace view and the field editor can be constructed without an application instance.
    /// </summary>
    [Fact]
    public void Views_LoadXaml()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                CultureInfo.CurrentUICulture = new CultureInfo("en");
                var workspaceView = new ItemWorkspaceView();
                var fieldEditor = new FieldEditor();
                Assert.NotNull(workspaceView.Content);
                Assert.NotNull(fieldEditor.Content);
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

        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The item workspace view test did not finish in time.");
        Assert.Null(failure);
    }
}
