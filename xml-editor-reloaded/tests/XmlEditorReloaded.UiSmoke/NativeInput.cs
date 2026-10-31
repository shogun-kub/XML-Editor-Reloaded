using System.Runtime.InteropServices;

namespace XmlEditorReloaded.UiSmoke;

/// <summary>
/// Keyboard and mouse input used where UI Automation patterns are not available, for example inline
/// cell editing in a WPF <c>DataGrid</c>.
/// </summary>
internal static class NativeInput
{
    private const uint InputKeyboard = 1;
    private const uint InputMouse = 0;
    private const uint KeyEventKeyUp = 0x0002;
    private const uint KeyEventUnicode = 0x0004;
    private const uint MouseEventLeftDown = 0x0002;
    private const uint MouseEventLeftUp = 0x0004;

    /// <summary>
    /// Virtual-key codes used by the runner.
    /// </summary>
    public enum VirtualKey : ushort
    {
        /// <summary>Enter key.</summary>
        Return = 0x0D,

        /// <summary>Escape key.</summary>
        Escape = 0x1B,

        /// <summary>Tab key.</summary>
        Tab = 0x09,

        /// <summary>End key.</summary>
        End = 0x23,

        /// <summary>Home key.</summary>
        Home = 0x24,

        /// <summary>F2 key.</summary>
        F2 = 0x71,

        /// <summary>A key.</summary>
        A = 0x41,

        /// <summary>S key.</summary>
        S = 0x53,

        /// <summary>Z key.</summary>
        Z = 0x5A,

        /// <summary>Y key.</summary>
        Y = 0x59,

        /// <summary>Insert key.</summary>
        Insert = 0x2D,
    }

    /// <summary>
    /// Brings a window to the foreground so synthetic mouse and keyboard input reaches it.
    /// </summary>
    /// <remarks>
    /// A background process is normally not allowed to change the foreground window, so the input
    /// queues of the current foreground thread and the target thread are attached for the call.
    /// Without this, synthetic clicks land on whatever window happens to be on top, which may be the
    /// console that started the runner.
    /// </remarks>
    /// <param name="windowHandle">Native window handle.</param>
    public static void Activate(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
        {
            return;
        }

        if (GetForegroundWindow() == windowHandle)
        {
            return;
        }

        if (IsIconic(windowHandle))
        {
            ShowWindow(windowHandle, RestoreWindow);
        }

        var foregroundWindow = GetForegroundWindow();
        var foregroundThread = foregroundWindow == IntPtr.Zero
            ? 0
            : GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
        var targetThread = GetWindowThreadProcessId(windowHandle, IntPtr.Zero);
        var attached = false;
        try
        {
            if (foregroundThread != 0 && foregroundThread != targetThread)
            {
                attached = AttachThreadInput(foregroundThread, targetThread, attach: true);
            }

            BringWindowToTop(windowHandle);
            SetForegroundWindow(windowHandle);
            SetActiveWindow(windowHandle);
        }
        finally
        {
            if (attached)
            {
                AttachThreadInput(foregroundThread, targetThread, attach: false);
            }
        }

        Thread.Sleep(120);
    }

    /// <summary>
    /// Sends a single virtual key, optionally with Control and/or Shift held.
    /// </summary>
    /// <param name="key">Virtual key to press.</param>
    /// <param name="control">Whether Control is held.</param>
    /// <param name="shift">Whether Shift is held.</param>
    public static void Press(VirtualKey key, bool control = false, bool shift = false)
    {
        var inputs = new List<Input>();
        if (control)
        {
            inputs.Add(KeyInput(0x11, down: true));
        }

        if (shift)
        {
            inputs.Add(KeyInput(0x10, down: true));
        }

        inputs.Add(KeyInput((ushort)key, down: true));
        inputs.Add(KeyInput((ushort)key, down: false));
        if (shift)
        {
            inputs.Add(KeyInput(0x10, down: false));
        }

        if (control)
        {
            inputs.Add(KeyInput(0x11, down: false));
        }

        Send(inputs);
        Thread.Sleep(30);
    }

    /// <summary>
    /// Types text as Unicode characters.
    /// </summary>
    /// <param name="text">Text to type.</param>
    public static void TypeText(string text)
    {
        var inputs = new List<Input>();
        foreach (var character in text)
        {
            inputs.Add(UnicodeInput(character, KeyEventUnicode));
            inputs.Add(UnicodeInput(character, KeyEventUnicode | KeyEventKeyUp));
        }

        Send(inputs);
        Thread.Sleep(40);
    }

    /// <summary>
    /// Clicks the center of the element once.
    /// </summary>
    /// <param name="element">Target element.</param>
    public static void Click(System.Windows.Automation.AutomationElement element)
    {
        Activate(TopLevelWindow(element));
        var point = Center(element);
        SetCursorPos(point.X, point.Y);
        Thread.Sleep(40);
        Send([MouseEventInput(MouseEventLeftDown), MouseEventInput(MouseEventLeftUp)]);
        Thread.Sleep(80);
    }

    /// <summary>
    /// Double-clicks the center of the element, entering inline edit for a grid cell.
    /// </summary>
    /// <param name="element">Target element.</param>
    public static void DoubleClick(System.Windows.Automation.AutomationElement element)
    {
        Activate(TopLevelWindow(element));
        var point = Center(element);
        SetCursorPos(point.X, point.Y);
        Thread.Sleep(40);
        Send(
        [
            MouseEventInput(MouseEventLeftDown),
            MouseEventInput(MouseEventLeftUp),
            MouseEventInput(MouseEventLeftDown),
            MouseEventInput(MouseEventLeftUp),
        ]);
        Thread.Sleep(120);
    }

    private static Point Center(System.Windows.Automation.AutomationElement element)
    {
        var rectangle = element.Current.BoundingRectangle;
        return new Point(
            (int)Math.Round(rectangle.Left + (rectangle.Width / 2)),
            (int)Math.Round(rectangle.Top + (rectangle.Height / 2)));
    }

    /// <summary>
    /// Returns the native handle of the top-level window that owns an element.
    /// </summary>
    /// <param name="element">Element inside the window.</param>
    /// <returns>Window handle, or <see cref="IntPtr.Zero"/> when it cannot be resolved.</returns>
    private static IntPtr TopLevelWindow(System.Windows.Automation.AutomationElement element)
    {
        var walker = System.Windows.Automation.TreeWalker.ControlViewWalker;
        var current = element;
        while (true)
        {
            System.Windows.Automation.AutomationElement? parent;
            try
            {
                parent = walker.GetParent(current);
            }
            catch (System.Windows.Automation.ElementNotAvailableException)
            {
                return IntPtr.Zero;
            }

            if (parent is null
                || ReferenceEquals(parent, System.Windows.Automation.AutomationElement.RootElement))
            {
                break;
            }

            current = parent;
        }

        try
        {
            return new IntPtr(current.Current.NativeWindowHandle);
        }
        catch (System.Windows.Automation.ElementNotAvailableException)
        {
            return IntPtr.Zero;
        }
    }

    private static Input KeyInput(ushort virtualKey, bool down) => new()
    {
        Type = InputKeyboard,
        Data = new InputUnion
        {
            Keyboard = new KeyboardInputData
            {
                VirtualKey = virtualKey,
                Flags = down ? 0 : KeyEventKeyUp,
            },
        },
    };

    private static Input UnicodeInput(char character, uint flags) => new()
    {
        Type = InputKeyboard,
        Data = new InputUnion
        {
            Keyboard = new KeyboardInputData
            {
                ScanCode = character,
                Flags = flags,
            },
        },
    };

    private static Input MouseEventInput(uint flags) => new()
    {
        Type = InputMouse,
        Data = new InputUnion
        {
            Mouse = new MouseInputData { Flags = flags },
        },
    };

    private static void Send(IReadOnlyList<Input> inputs)
    {
        if (inputs.Count == 0)
        {
            return;
        }

        var array = inputs.ToArray();
        SendInput((uint)array.Length, array, Marshal.SizeOf<Input>());
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Input
    {
        public uint Type;
        public InputUnion Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)]
        public MouseInputData Mouse;

        [FieldOffset(0)]
        public KeyboardInputData Keyboard;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MouseInputData
    {
        public int X;
        public int Y;
        public uint MouseData;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KeyboardInputData
    {
        public ushort VirtualKey;
        public ushort ScanCode;
        public uint Flags;
        public uint Time;
        public IntPtr ExtraInfo;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint count, [In] Input[] inputs, int size);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(IntPtr window);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsIconic(IntPtr window);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(IntPtr window, int command);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr window, IntPtr processId);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool attach);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool BringWindowToTop(IntPtr window);

    [DllImport("user32.dll")]
    private static extern IntPtr SetActiveWindow(IntPtr window);

    private const int RestoreWindow = 9;
}

