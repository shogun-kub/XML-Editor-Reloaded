using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XmlEditorReloaded.UiSmoke.Scenarios;

namespace XmlEditorReloaded.UiSmoke;

/// <summary>
/// Captures reference screenshots of the application shell, the item workspace, and a table editor.
/// Used for before/after reviews of shell layout changes; the captures are not part of the smoke
/// verdict and are written to the directory passed with <c>--shots</c>.
/// </summary>
internal static class Screenshots
{
    /// <summary>
    /// Starts the application against the read-only v5 data set, captures the requested views, and
    /// closes it again.
    /// </summary>
    /// <param name="options">Runner options.</param>
    /// <returns>Zero when every capture was written.</returns>
    public static int Run(SmokeOptions options)
    {
        var directory = options.ShotsDirectory!;
        Directory.CreateDirectory(directory);
        var gameRoot = Path.Combine(options.ReferencesRoot, "JA 2 1.13 v5");
        var settings = new SettingsModel
        {
            DataDirectories = [gameRoot],
            ActiveDataDirectory = gameRoot,
            UiLanguageCode = options.UiLanguage,
        };

        TryEnablePerMonitorDpi();
        using var app = UiApp.Start(options, Path.Combine(directory, "appdata"), settings);
        try
        {
            Thread.Sleep(800);
            CaptureMainWindow(app, Path.Combine(directory, "01-main.png"));

            var window = app.Window;
            var itemsRow = ScenarioUi.FindTableRow(window, "Items.Items")
                ?? throw new InvalidOperationException("Items.Items was not found in the table list.");
            itemsRow.SelectItem();
            window.WaitById("OpenTableButton").InvokeButton();
            window.WaitById("ItemTabs", TimeSpan.FromSeconds(60));
            window.WaitById("ItemGoToIdBox").SetText("12");
            window.WaitById("ItemGoToIdButton").InvokeButton();
            Ui.WaitUntil(
                () => window.WaitById("ItemDetailHeader").ReadName().StartsWith("12:", StringComparison.Ordinal),
                "item 12 is selected",
                TimeSpan.FromSeconds(20));
            Thread.Sleep(600);
            CaptureMainWindow(app, Path.Combine(directory, "02-item-workspace.png"));

            var weaponsRow = ScenarioUi.FindTableRow(window, "Items.Weapons")
                ?? throw new InvalidOperationException("Items.Weapons was not found in the table list.");
            weaponsRow.SelectItem();
            window.WaitById("OpenTableButton").InvokeButton();
            window.WaitById("RowsGrid", TimeSpan.FromSeconds(60));
            Thread.Sleep(600);
            CaptureMainWindow(app, Path.Combine(directory, "03-table-editor.png"));
        }
        finally
        {
            app.Close();
        }

        return 0;
    }

    private static void CaptureMainWindow(UiApp app, string path)
    {
        app.RefreshWindow();
        app.Activate();
        Thread.Sleep(250);
        CaptureWindow(app.WindowHandle, path);
        Console.WriteLine($"Screenshot: {path}");
    }

    private static void CaptureWindow(IntPtr windowHandle, string path)
    {
        if (!GetWindowRect(windowHandle, out var bounds))
        {
            throw new InvalidOperationException("The window bounds could not be read.");
        }

        var width = bounds.Right - bounds.Left;
        var height = bounds.Bottom - bounds.Top;
        if (width <= 0 || height <= 0)
        {
            throw new InvalidOperationException("The window has no visible area.");
        }

        var screen = GetDC(IntPtr.Zero);
        var memory = CreateCompatibleDC(screen);
        var bitmap = CreateCompatibleBitmap(screen, width, height);
        var previous = SelectObject(memory, bitmap);
        try
        {
            if (!BitBlt(memory, 0, 0, width, height, screen, bounds.Left, bounds.Top, SourceCopy | CaptureBlt))
            {
                throw new InvalidOperationException("BitBlt failed.");
            }

            var stride = width * 4;
            var pixels = new byte[stride * height];
            var info = new BitmapInfoHeader
            {
                Size = (uint)Marshal.SizeOf<BitmapInfoHeader>(),
                Width = width,
                // A negative height asks GDI for top-down rows, which is what BitmapSource expects.
                Height = -height,
                Planes = 1,
                BitCount = 32,
                Compression = 0,
            };
            if (GetDIBits(memory, bitmap, 0, (uint)height, pixels, ref info, 0) == 0)
            {
                throw new InvalidOperationException("GetDIBits failed.");
            }

            var source = BitmapSource.Create(
                width,
                height,
                96,
                96,
                PixelFormats.Bgra32,
                null,
                pixels,
                stride);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(source));
            using var stream = File.Create(path);
            encoder.Save(stream);
        }
        finally
        {
            SelectObject(memory, previous);
            DeleteObject(bitmap);
            DeleteDC(memory);
            ReleaseDC(IntPtr.Zero, screen);
        }
    }

    private static void TryEnablePerMonitorDpi()
    {
        try
        {
            SetProcessDpiAwarenessContext(new IntPtr(-4));
        }
        catch (Exception exception) when (exception is EntryPointNotFoundException or DllNotFoundException)
        {
            // Older Windows versions keep the default awareness; the capture still works.
        }
    }

    private const uint SourceCopy = 0x00CC0020;
    private const uint CaptureBlt = 0x40000000;

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BitmapInfoHeader
    {
        public uint Size;
        public int Width;
        public int Height;
        public ushort Planes;
        public ushort BitCount;
        public uint Compression;
        public uint SizeImage;
        public int XPelsPerMeter;
        public int YPelsPerMeter;
        public uint ClrUsed;
        public uint ClrImportant;
    }

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr window, out NativeRect rectangle);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr window);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr window, IntPtr deviceContext);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetProcessDpiAwarenessContext(IntPtr context);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleDC(IntPtr deviceContext);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleBitmap(IntPtr deviceContext, int width, int height);

    [DllImport("gdi32.dll")]
    private static extern IntPtr SelectObject(IntPtr deviceContext, IntPtr handle);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr handle);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(IntPtr deviceContext);

    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(
        IntPtr target,
        int x,
        int y,
        int width,
        int height,
        IntPtr source,
        int sourceX,
        int sourceY,
        uint flags);

    [DllImport("gdi32.dll")]
    private static extern int GetDIBits(
        IntPtr deviceContext,
        IntPtr bitmap,
        uint start,
        uint lines,
        byte[] bits,
        ref BitmapInfoHeader info,
        uint usage);
}
