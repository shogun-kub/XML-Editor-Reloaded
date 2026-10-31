using System.Globalization;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using XmlEditorReloaded.App.ViewModels;

namespace XmlEditorReloaded.App.Views;

/// <summary>
/// Searchable picker for a relation field. The text box shows the target display name while the
/// bound value always remains the raw key, so the document is written with identifiers, never names.
/// Typing filters the options by name or key and opens the list; Enter commits, Escape restores.
/// </summary>
public partial class RelationPicker : UserControl
{
    /// <summary>
    /// Identifies the <see cref="RelationData"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RelationDataProperty = DependencyProperty.Register(
        nameof(RelationData),
        typeof(RelationColumnData),
        typeof(RelationPicker),
        new PropertyMetadata(null, OnRelationDataChanged));

    /// <summary>
    /// Identifies the <see cref="SelectedValue"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register(
        nameof(SelectedValue),
        typeof(string),
        typeof(RelationPicker),
        new FrameworkPropertyMetadata(
            string.Empty,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnSelectedValueChanged));

    /// <summary>
    /// Identifies the <see cref="IsReadOnly"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
        nameof(IsReadOnly),
        typeof(bool),
        typeof(RelationPicker),
        new PropertyMetadata(false, OnIsReadOnlyChanged));

    /// <summary>
    /// Identifies the <see cref="AutomationId"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty AutomationIdProperty = DependencyProperty.Register(
        nameof(AutomationId),
        typeof(string),
        typeof(RelationPicker),
        new PropertyMetadata(null, OnAutomationIdChanged));

    private bool _suppressText;
    private bool _committing;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelationPicker"/> class.
    /// </summary>
    public RelationPicker()
    {
        InitializeComponent();
        Loaded += (_, _) => UpdateText();
    }

    /// <summary>
    /// Gets or sets the resolved relation data that supplies options and display names.
    /// </summary>
    public RelationColumnData? RelationData
    {
        get => (RelationColumnData?)GetValue(RelationDataProperty);
        set => SetValue(RelationDataProperty, value);
    }

    /// <summary>
    /// Gets or sets the raw key written back to the document.
    /// </summary>
    public string SelectedValue
    {
        get => (string)GetValue(SelectedValueProperty);
        set => SetValue(SelectedValueProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the value can be changed.
    /// </summary>
    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    /// Gets or sets the automation identifier assigned to the embedded value text box, so the picker
    /// can be driven by UI automation.
    /// </summary>
    public string? AutomationId
    {
        get => (string?)GetValue(AutomationIdProperty);
        set => SetValue(AutomationIdProperty, value);
    }

    private static void OnRelationDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var picker = (RelationPicker)sender;
        picker.UpdateText();
        picker.CloseDropDown();
    }

    private static void OnSelectedValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) =>
        ((RelationPicker)sender).UpdateText();

    private static void OnIsReadOnlyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var picker = (RelationPicker)sender;
        picker.ValueBox.IsReadOnly = picker.IsReadOnly;
        picker.DropDownButton.IsEnabled = !picker.IsReadOnly;
    }

    private static void OnAutomationIdChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var picker = (RelationPicker)sender;
        AutomationProperties.SetAutomationId(picker.ValueBox, e.NewValue as string ?? string.Empty);
    }

    private void UpdateText()
    {
        if (ValueBox is null)
        {
            return;
        }

        _suppressText = true;
        try
        {
            var value = SelectedValue;
            ValueBox.Text = RelationData?.Display(value) ?? value ?? string.Empty;
        }
        finally
        {
            _suppressText = false;
        }
    }

    private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (!IsReadOnly)
        {
            ValueBox.SelectAll();
        }
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_suppressText || IsReadOnly || RelationData is null)
        {
            return;
        }

        var options = RelationData.Filter(ValueBox.Text);
        OptionsList.ItemsSource = options;
        OptionsList.SelectedIndex = options.Count > 0 ? 0 : -1;
        if (!OptionsPopup.IsOpen)
        {
            OptionsPopup.IsOpen = true;
        }
    }

    private void OnDropDownClick(object sender, RoutedEventArgs e)
    {
        if (IsReadOnly)
        {
            return;
        }

        if (OptionsPopup.IsOpen)
        {
            CloseDropDown();
            return;
        }

        ShowOptions(RelationData?.Filter(null) ?? Array.Empty<RelationOption>());
        ValueBox.Focus();
    }

    private void ShowOptions(IReadOnlyList<RelationOption> options)
    {
        OptionsList.ItemsSource = options;
        OptionsList.SelectedIndex = options.Count > 0 ? 0 : -1;
        OptionsPopup.IsOpen = true;
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (IsReadOnly)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Down:
                e.Handled = true;
                if (!OptionsPopup.IsOpen)
                {
                    ShowOptions(RelationData?.Filter(ValueBox.Text) ?? Array.Empty<RelationOption>());
                }
                else if (OptionsList.Items.Count > 0)
                {
                    OptionsList.SelectedIndex = Math.Min(
                        OptionsList.SelectedIndex + 1,
                        OptionsList.Items.Count - 1);
                }

                break;
            case Key.Up:
                e.Handled = true;
                if (OptionsList.SelectedIndex > 0)
                {
                    OptionsList.SelectedIndex--;
                }

                break;
            case Key.Enter:
                e.Handled = true;
                CommitTyped();
                break;
            case Key.Escape:
                e.Handled = true;
                CloseDropDown();
                UpdateText();
                break;
            case Key.Tab:
                CloseDropDown();
                break;
        }
    }

    private void OnOptionClicked(object sender, MouseButtonEventArgs e)
    {
        if (OptionsList.SelectedItem is RelationOption option)
        {
            Commit(option);
        }
    }

    private void CommitTyped()
    {
        if (OptionsList.SelectedItem is RelationOption selected)
        {
            Commit(selected);
            return;
        }

        var typed = ValueBox.Text.Trim();
        if (typed.Length > 0 && RelationData is not null)
        {
            var match = RelationData.Options.FirstOrDefault(option =>
                string.Equals(option.DisplayText, typed, StringComparison.OrdinalIgnoreCase)
                || string.Equals(option.Value, typed, StringComparison.Ordinal));
            if (match is not null)
            {
                Commit(match);
                return;
            }

            // Power users may enter a key that is absent from the target; it stays raw and is
            // reported by validation instead of being silently remapped.
            if (long.TryParse(typed, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            {
                _committing = true;
                try
                {
                    SelectedValue = typed;
                    OptionsPopup.IsOpen = false;
                }
                finally
                {
                    _committing = false;
                }

                UpdateText();
                return;
            }
        }

        CloseDropDown();
        UpdateText();
    }

    private void Commit(RelationOption option)
    {
        _committing = true;
        try
        {
            SelectedValue = option.Value;
            OptionsPopup.IsOpen = false;
        }
        finally
        {
            _committing = false;
        }

        UpdateText();
    }

    private void OnPopupClosed(object? sender, EventArgs e)
    {
        if (!_committing)
        {
            UpdateText();
        }
    }

    private void CloseDropDown()
    {
        if (OptionsPopup.IsOpen)
        {
            _committing = true;
            try
            {
                OptionsPopup.IsOpen = false;
            }
            finally
            {
                _committing = false;
            }
        }
    }
}
