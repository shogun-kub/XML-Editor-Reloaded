using System.Windows.Automation;

namespace XmlEditorReloaded.UiSmoke.Scenarios;

/// <summary>
/// Drives the native Windows file dialogs that the application opens for folder picking and report
/// export. The dialogs are owned by the application process but are not WPF windows, so they are found
/// by their localized title and answered through the file-name edit and the default button.
/// </summary>
internal static class NativeDialogUi
{
    /// <summary>
    /// Waits until a top-level window of the application carries the given title.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <param name="title">Localized window title.</param>
    /// <param name="description">Description used in the timeout message.</param>
    /// <param name="timeout">Optional timeout.</param>
    /// <returns>The dialog window.</returns>
    /// <exception cref="UiTimeoutException">Thrown when the dialog does not appear in time.</exception>
    public static AutomationElement WaitForDialog(
        UiApp app,
        string title,
        string description,
        TimeSpan? timeout = null) =>
        Ui.WaitFor(() => FindWindow(app, title), description, timeout);

    /// <summary>
    /// Determines whether a top-level window of the application with the given title is still open.
    /// </summary>
    /// <param name="app">Running application.</param>
    /// <param name="title">Localized window title.</param>
    /// <returns><see langword="true"/> while the window exists.</returns>
    public static bool IsOpen(UiApp app, string title) => FindWindow(app, title) is not null;

    /// <summary>
    /// Enters a path into the "File name" or "Folder" box of a native dialog and confirms it. The path
    /// may point to a folder that the dialog has to enter first, so the default button is pressed once
    /// more when the dialog stays open after Enter.
    /// </summary>
    /// <param name="dialog">File or folder dialog.</param>
    /// <param name="path">Absolute path to enter.</param>
    /// <param name="timeout">Optional overall timeout.</param>
    /// <exception cref="UiTimeoutException">Thrown when the dialog cannot be completed.</exception>
    public static void AcceptPath(AutomationElement dialog, string path, TimeSpan? timeout = null)
    {
        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(30));
        var handle = new IntPtr(dialog.Current.NativeWindowHandle);
        NativeInput.PlaceOnPrimaryScreen(handle);
        // The main window is kept topmost for the run, so a file dialog owned by a secondary window can
        // end up underneath it; both the keyboard focus and synthetic clicks then go to the main window.
        NativeInput.KeepOnTop(handle);
        NativeInput.Activate(handle);
        var edit = FindPathEdit(dialog);
        edit.SetText(path);
        Thread.Sleep(300);

        // The file name is committed with the default button. Enter is kept as a fallback because a
        // save dialog may open the auto-complete list first, which would swallow the first Enter.
        ClickDefaultButton(dialog);
        if (WaitForClose(dialog, TimeSpan.FromSeconds(2)))
        {
            return;
        }

        NativeInput.PressIn(edit, NativeInput.VirtualKey.Return);
        if (WaitForClose(dialog, TimeSpan.FromSeconds(2)))
        {
            return;
        }

        ClickDefaultButton(dialog);
        var remaining = deadline - DateTime.UtcNow;
        if (remaining < TimeSpan.Zero)
        {
            remaining = TimeSpan.FromSeconds(10);
        }

        if (!WaitForClose(dialog, remaining))
        {
            throw new UiTimeoutException(
                $"Диалог «{dialog.ReadName()}» не закрылся после ввода пути «{path}».");
        }
    }

    private static void ClickDefaultButton(AutomationElement dialog)
    {
        var button = FindDefaultButton(dialog);
        if (button is not null)
        {
            button.InvokeButton();
            return;
        }

        NativeInput.PressIn(dialog, NativeInput.VirtualKey.Return);
    }

    /// <summary>
    /// Waits until the dialog window is gone.
    /// </summary>
    /// <param name="dialog">Dialog window.</param>
    /// <param name="timeout">How long to wait.</param>
    /// <returns><see langword="true"/> when the window closed in time.</returns>
    public static bool WaitForClose(AutomationElement dialog, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (!IsAvailable(dialog))
            {
                return true;
            }

            Thread.Sleep(100);
        }

        return !IsAvailable(dialog);
    }

    private static bool IsAvailable(AutomationElement element)
    {
        try
        {
            _ = element.Current.Name;
            return true;
        }
        catch (Exception exception) when (
            exception is ElementNotAvailableException or System.Runtime.InteropServices.COMException)
        {
            return false;
        }
    }

    private static AutomationElement? FindWindow(UiApp app, string title)
    {
        try
        {
            var windowCondition = new AndCondition(
                new PropertyCondition(AutomationElement.ProcessIdProperty, app.ProcessId),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
            foreach (var root in AutomationElement.RootElement
                         .FindAll(TreeScope.Children, windowCondition)
                         .Cast<AutomationElement>())
            {
                if (string.Equals(root.ReadName(), title, StringComparison.Ordinal))
                {
                    return root;
                }

                // WPF shows an owned dialog as a child window of its owner in the automation tree, so
                // the native file dialog is searched below the top-level windows as well.
                var nested = root.FindFirst(
                    TreeScope.Descendants,
                    new AndCondition(
                        windowCondition,
                        new PropertyCondition(AutomationElement.NameProperty, title)));
                if (nested is not null)
                {
                    return nested;
                }
            }

            return null;
        }
        catch (Exception exception) when (
            exception is ElementNotAvailableException or InvalidOperationException)
        {
            return null;
        }
    }

    /// <summary>
    /// Finds the edit control that receives the path. The classic identifier is used first; otherwise the
    /// bottom-most editable input is chosen, because the search and address boxes sit above it.
    /// </summary>
    /// <param name="dialog">Native file dialog.</param>
    /// <returns>The path edit control.</returns>
    /// <exception cref="UiTimeoutException">Thrown when no editable input exists.</exception>
    private static AutomationElement FindPathEdit(AutomationElement dialog)
    {
        var classic = dialog.FindFirst(
            TreeScope.Descendants,
            new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit),
                new PropertyCondition(AutomationElement.AutomationIdProperty, "1001")));
        if (classic is not null)
        {
            return classic;
        }

        var edits = dialog
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit))
            .Where(static edit => edit.IsEnabled())
            .OrderByDescending(static edit => edit.Current.BoundingRectangle.Top)
            .ToList();
        return edits.FirstOrDefault()
            ?? throw new UiTimeoutException($"В диалоге «{dialog.ReadName()}» нет поля для пути.");
    }

    private static AutomationElement? FindDefaultButton(AutomationElement dialog)
    {
        // A folder-view item can also carry the identifier "1", so the identifier alone is not enough.
        var byId = dialog.FindFirst(
            TreeScope.Descendants,
            new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                new PropertyCondition(AutomationElement.AutomationIdProperty, "1")));
        if (byId is not null)
        {
            return byId;
        }

        return dialog
            .FindAll(new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button))
            .FirstOrDefault(static button =>
                button.ReadName().Replace("&", string.Empty, StringComparison.Ordinal).Trim()
                    is "Save" or "Сохранить" or "Select Folder" or "Выбрать папку" or "Open" or "Открыть"
                        or "OK" or "ОК");
    }
}
