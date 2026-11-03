using CommunityToolkit.Mvvm.Input;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// One toolbar action that appends a repeated nested block such as a facility <c>PRODUCTIONLINE</c>.
/// </summary>
public sealed class NestedBlockAddViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NestedBlockAddViewModel"/> class.
    /// </summary>
    /// <param name="text">Localized button text.</param>
    /// <param name="automationId">Stable automation identifier of the button.</param>
    /// <param name="command">Command that appends the block.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is null.</exception>
    public NestedBlockAddViewModel(string text, string automationId, IRelayCommand command)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(automationId);
        ArgumentNullException.ThrowIfNull(command);
        Text = text;
        AutomationId = automationId;
        Command = command;
    }

    /// <summary>
    /// Gets the localized button text.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the stable automation identifier of the button.
    /// </summary>
    public string AutomationId { get; }

    /// <summary>
    /// Gets the command that appends the block.
    /// </summary>
    public IRelayCommand Command { get; }
}
