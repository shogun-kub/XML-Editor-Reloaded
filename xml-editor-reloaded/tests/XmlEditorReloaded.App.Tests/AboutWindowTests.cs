using System.Globalization;
using System.Text.RegularExpressions;
using XmlEditorReloaded.App.ViewModels;
using XmlEditorReloaded.App.Views;

namespace XmlEditorReloaded.App.Tests;

/// <summary>
/// Tests for the About window and its assembly-metadata view model.
/// </summary>
public class AboutWindowTests
{
    /// <summary>
    /// The About content comes from the application assembly metadata: product, version, and
    /// description are populated and localized labels are resolved.
    /// </summary>
    [Fact]
    public void ViewModel_ShowsAssemblyProductVersionAndDescription()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");

        var viewModel = new AboutViewModel();

        Assert.Contains("XML Editor Reloaded", viewModel.ProductName, StringComparison.Ordinal);
        Assert.Matches(new Regex(@"^\d+\.\d+"), viewModel.VersionText);
        Assert.Contains("Jagged Alliance 2", viewModel.Description, StringComparison.Ordinal);
        Assert.Equal("About XML Editor Reloaded", viewModel.Title);
        Assert.Equal("Version", viewModel.VersionLabel);
        Assert.Equal("Description", viewModel.DescriptionLabel);
        Assert.True(viewModel.HasDescription);
        Assert.Equal("OK", viewModel.OkText);
    }

    /// <summary>
    /// An assembly without product and description metadata falls back to its simple name and hides the
    /// description row instead of failing.
    /// </summary>
    [Fact]
    public void ViewModel_AssemblyWithoutDescription_FallsBackAndHidesRow()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");

        var viewModel = new AboutViewModel(typeof(AboutWindowTests).Assembly);

        Assert.Equal(typeof(AboutWindowTests).Assembly.GetName().Name, viewModel.ProductName);
        Assert.Equal(string.Empty, viewModel.Description);
        Assert.False(viewModel.HasDescription);
        Assert.False(string.IsNullOrWhiteSpace(viewModel.VersionText));
    }

    /// <summary>
    /// The About labels follow the interface language.
    /// </summary>
    [Fact]
    public void ViewModel_UsesRussianLabels()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("ru");
        try
        {
            var viewModel = new AboutViewModel();

            Assert.Equal("О программе XML Editor Reloaded", viewModel.Title);
            Assert.Equal("Версия", viewModel.VersionLabel);
            Assert.Equal("Описание", viewModel.DescriptionLabel);
            Assert.Equal("ОК", viewModel.OkText);
        }
        finally
        {
            CultureInfo.CurrentUICulture = new CultureInfo("en");
        }
    }

    /// <summary>
    /// The window loads its XAML on an STA thread and binds the view model.
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
                var viewModel = new AboutViewModel();
                var window = new AboutWindow(viewModel);

                Assert.Same(viewModel, window.DataContext);
                Assert.Equal(viewModel.Title, window.Title);
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

        Assert.True(thread.Join(TimeSpan.FromSeconds(10)), "The About window test did not finish in time.");
        Assert.Null(failure);
    }
}
