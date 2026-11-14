using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;
using XmlEditorReloaded.Core.Abstractions;

namespace XmlEditorReloaded.App.Themes;

/// <summary>
/// Applies the selected visual theme by swapping the palette resource dictionary. Control styles and
/// icons use <c>DynamicResource</c>, so no window needs to be recreated.
/// </summary>
public static partial class ThemeManager
{
    private const string LightSource = "Themes/Theme.Light.xaml";
    private const string DarkSource = "Themes/Theme.Dark.xaml";

    /// <summary>
    /// Gets the theme mode requested by the user.
    /// </summary>
    public static EditorThemeMode CurrentMode { get; private set; } = EditorThemeMode.System;

    /// <summary>
    /// Gets a value indicating whether the dark palette is currently active.
    /// </summary>
    public static bool IsDark { get; private set; }

    /// <summary>
    /// Applies the requested theme. <see cref="EditorThemeMode.System"/> follows the Windows
    /// "app mode" registry value and falls back to the light palette when it cannot be read.
    /// </summary>
    /// <param name="mode">Theme selected in settings.</param>
    public static void Apply(EditorThemeMode mode)
    {
        CurrentMode = mode;
        var application = Application.Current;
        if (application is null)
        {
            return;
        }

        IsDark = mode switch
        {
            EditorThemeMode.Dark => true,
            EditorThemeMode.Light => false,
            _ => SystemPrefersDark(),
        };

        var dictionaries = application.Resources.MergedDictionaries;
        var index = FindThemeDictionary(dictionaries);
        var theme = new ResourceDictionary
        {
            Source = new Uri(IsDark ? DarkSource : LightSource, UriKind.Relative),
        };

        if (index >= 0)
        {
            dictionaries[index] = theme;
        }
        else
        {
            dictionaries.Insert(0, theme);
        }

        foreach (Window window in application.Windows)
        {
            ApplyWindowChrome(window);
        }
    }

    /// <summary>
    /// Applies the immersive dark title bar to one window when the dark palette is active.
    /// </summary>
    /// <param name="window">Window whose non-client area should follow the theme.</param>
    public static void ApplyWindowChrome(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);
        if (!window.IsLoaded)
        {
            window.SourceInitialized += OnWindowSourceInitialized;
            return;
        }

        var handle = new WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero)
        {
            return;
        }

        var value = IsDark ? 1 : 0;
        if (DwmSetWindowAttribute(handle, 20, ref value, sizeof(int)) != 0)
        {
            // Attribute 19 is the pre-20H1 name for the same immersive-dark-mode flag.
            DwmSetWindowAttribute(handle, 19, ref value, sizeof(int));
        }
    }

    private static void OnWindowSourceInitialized(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            window.SourceInitialized -= OnWindowSourceInitialized;
            ApplyWindowChrome(window);
        }
    }

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

    /// <summary>
    /// Reads the Windows application theme preference.
    /// </summary>
    /// <returns><see langword="true"/> when Windows is configured for dark application mode.</returns>
    public static bool SystemPrefersDark()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            return key?.GetValue("AppsUseLightTheme") is int value && value == 0;
        }
        catch (Exception)
        {
            // A locked-down registry is not a reason to fail startup; the light palette is a safe default.
            return false;
        }
    }

    private static int FindThemeDictionary(Collection<ResourceDictionary> dictionaries)
    {
        for (var index = 0; index < dictionaries.Count; index++)
        {
            var source = dictionaries[index].Source?.OriginalString;
            if (source is not null
                && (source.EndsWith(LightSource, StringComparison.OrdinalIgnoreCase)
                    || source.EndsWith(DarkSource, StringComparison.OrdinalIgnoreCase)))
            {
                return index;
            }
        }

        return -1;
    }
}
