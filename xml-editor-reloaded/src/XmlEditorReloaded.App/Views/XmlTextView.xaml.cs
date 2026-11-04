using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Shared read-only XML presentation for every table tab.
/// </summary>
public partial class XmlTextView : UserControl
{
    private MainViewModel? _viewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlTextView"/> class.
    /// </summary>
    public XmlTextView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnCopyXmlClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel { XmlText.Length: > 0 } viewModel)
        {
            Clipboard.SetText(viewModel.XmlText);
        }
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) =>
        AttachViewModel(DataContext as MainViewModel);

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AttachViewModel(DataContext as MainViewModel);
        ApplySelection();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e) => AttachViewModel(null);

    private void AttachViewModel(MainViewModel? viewModel)
    {
        if (ReferenceEquals(_viewModel, viewModel))
        {
            return;
        }

        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _viewModel = viewModel;
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.XmlSelection))
        {
            ApplySelection();
        }
    }

    private void ApplySelection()
    {
        if (_viewModel?.XmlSelection is not { } selection)
        {
            return;
        }

        var textLength = XmlTextBox.Text.Length;
        if (textLength == 0)
        {
            return;
        }

        var start = Math.Clamp(selection.Start, 0, textLength);
        var length = Math.Clamp(selection.Length, 0, textLength - start);
        XmlTextBox.Select(start, length);
        var line = XmlTextBox.GetLineIndexFromCharacterIndex(start);
        if (line >= 0)
        {
            XmlTextBox.ScrollToLine(line);
        }

        XmlTextBox.Focus();
    }
}
