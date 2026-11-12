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
    private const uint MouseEventRightDown = 0x0008;
    private const uint MouseEventRightUp = 0x0010;

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

        /// <summary>Page Up key.</summary>
        PageUp = 0x21,

        /// <summary>Page Down key.</summary>
        PageDown = 0x22,

        /// <summary>Home key.</summary>
        Home = 0x24,

        /// <summary>F1 key.</summary>
        F1 = 0x70,

        /// <summary>F2 key.</summary>
        F2 = 0x71,

        /// <summary>A key.</summary>
        A = 0x41,

        /// <summary>D key.</summary>
        D = 0x44,

        /// <summary>F key.</summary>
        F = 0x46,

        /// <summary>G key.</summary>
        G = 0x47,

        /// <summary>L key.</summary>
        L = 0x4C,

        /// <summary>S key.</summary>
        S = 0x53,

        /// <summary>U key.</summary>
        U = 0x55,

        /// <summary>V key.</summary>
        V = 0x56,

        /// <summary>X key.</summary>
        X = 0x58,

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

        // Raise the window above other windows. The smoke run may execute while another window covers
        // the editor, and a covered window receives neither synthetic clicks nor the foreground. The
        // topmost state is preserved: HWND_TOP would move a topmost window into the normal band and put
        // it below every topmost window of other applications.
        Raise(windowHandle);

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
            SwitchToThisWindow(windowHandle, altTab: true);
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
        if (GetForegroundWindow() == windowHandle)
        {
            return;
        }

        // The foreground lock also refuses the activation when another application owns the input focus.
        // Lowering the lock timeout for the duration of the call is the documented way for a background
        // process to take the foreground; the previous value is restored immediately afterwards.
        if (TryGetForegroundLockTimeout(out var previousTimeout))
        {
            SetForegroundLockTimeout(0);
            try
            {
                SetForegroundWindow(windowHandle);
                Thread.Sleep(120);
            }
            finally
            {
                SetForegroundLockTimeout(previousTimeout);
            }
        }
    }

    /// <summary>
    /// Reads the current foreground lock timeout.
    /// </summary>
    /// <param name="timeout">Timeout in milliseconds.</param>
    /// <returns><see langword="true"/> when the value was read.</returns>
    private static bool TryGetForegroundLockTimeout(out uint timeout)
    {
        timeout = 0;
        return SystemParametersInfo(SpiGetForegroundLockTimeout, 0, ref timeout, 0);
    }

    /// <summary>
    /// Writes the foreground lock timeout.
    /// </summary>
    /// <param name="timeout">Timeout in milliseconds.</param>
    private static void SetForegroundLockTimeout(uint timeout) =>
        SystemParametersInfo(SpiSetForegroundLockTimeout, 0, ref timeout, SpifSendChange);

    /// <summary>
    /// Moves and resizes a top-level window without changing its z-order or activation. Used by the
    /// geometry scenario, where the smoke runner must not override the placement the application will
    /// persist.
    /// </summary>
    /// <param name="windowHandle">Native window handle.</param>
    /// <param name="x">Left edge in screen coordinates.</param>
    /// <param name="y">Top edge in screen coordinates.</param>
    /// <param name="width">Window width.</param>
    /// <param name="height">Window height.</param>
    public static void SetWindowBounds(IntPtr windowHandle, int x, int y, int width, int height)
    {
        if (windowHandle == IntPtr.Zero)
        {
            return;
        }

        SetWindowPos(windowHandle, HwndTop, x, y, width, height, SwpNoZOrder | SwpNoActivate);
        Thread.Sleep(200);
    }

    /// <summary>
    /// Reads the screen rectangle of a window in the same physical coordinate space as
    /// <see cref="VirtualScreenBounds"/>.
    /// </summary>
    /// <param name="windowHandle">Native window handle.</param>
    /// <returns>The window rectangle, or an empty rectangle when the handle is invalid.</returns>
    public static System.Windows.Rect WindowBounds(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero || !GetWindowRect(windowHandle, out var bounds))
        {
            return System.Windows.Rect.Empty;
        }

        return new System.Windows.Rect(
            bounds.Left,
            bounds.Top,
            bounds.Right - bounds.Left,
            bounds.Bottom - bounds.Top);
    }

    /// <summary>
    /// Reads the rectangle of the whole virtual screen, including secondary monitors with negative
    /// coordinates.
    /// </summary>
    /// <returns>The virtual-screen rectangle in physical pixels.</returns>
    public static System.Windows.Rect VirtualScreenBounds() => new(
        GetSystemMetrics(SmXVirtualScreen),
        GetSystemMetrics(SmYVirtualScreen),
        GetSystemMetrics(SmCxVirtualScreen),
        GetSystemMetrics(SmCyVirtualScreen));

    /// <summary>
    /// Raises a window to the top of the band it belongs to without changing its topmost state.
    /// </summary>
    /// <param name="windowHandle">Native window handle.</param>
    private static void Raise(IntPtr windowHandle)
    {
        var extendedStyle = GetWindowLongPtr(windowHandle, GwlExStyle).ToInt64();
        var insertAfter = (extendedStyle & WsExTopMost) != 0 ? HwndTopMost : HwndTop;
        SetWindowPos(windowHandle, insertAfter, 0, 0, 0, 0, SwpNoMove | SwpNoSize | SwpShowWindow);
    }

    /// <summary>
    /// Moves and resizes a window into the work area of the primary display when it was opened on another
    /// monitor. WPF centres a window without an owner on the monitor that holds the mouse cursor, so a
    /// secondary or virtual display can capture the window of the run; synthetic input does not reach
    /// every such display, and the run then fails with swallowed clicks instead of a real defect.
    /// </summary>
    /// <param name="windowHandle">Native window handle.</param>
    public static void PlaceOnPrimaryScreen(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero || !GetWindowRect(windowHandle, out var bounds))
        {
            return;
        }

        // The primary display always starts at the virtual origin, while secondary displays can lie above
        // or to the left of it. Resolving the monitor of the window would return that secondary display
        // and keep the window there, so the primary monitor is resolved from the origin instead.
        var monitor = MonitorFromPoint(new Point(0, 0), MonitorDefaultToPrimary);
        if (monitor == IntPtr.Zero)
        {
            return;
        }

        var info = new MonitorInfo { Size = Marshal.SizeOf<MonitorInfo>() };
        if (!GetMonitorInfo(monitor, ref info))
        {
            return;
        }

        var work = info.Work;
        var width = bounds.Right - bounds.Left;
        var height = bounds.Bottom - bounds.Top;
        if (bounds.Left >= work.Left
            && bounds.Top >= work.Top
            && bounds.Right <= work.Right
            && bounds.Bottom <= work.Bottom)
        {
            return;
        }

        var restore = IsZoomed(windowHandle);
        if (restore)
        {
            ShowWindow(windowHandle, RestoreWindow);
        }

        width = Math.Min(width, work.Right - work.Left);
        height = Math.Min(height, work.Bottom - work.Top);
        var x = work.Left + Math.Max(0, (work.Right - work.Left - width) / 2);
        var y = work.Top + Math.Max(0, (work.Bottom - work.Top - height) / 2);
        SetWindowPos(windowHandle, HwndTop, x, y, width, height, SwpNoZOrder | SwpNoActivate);
        Thread.Sleep(150);
        if (restore)
        {
            ShowWindow(windowHandle, MaximizeWindow);
            Thread.Sleep(150);
        }
    }

    /// <summary>
    /// Keeps a window above other applications for the rest of the run. Synthetic input otherwise lands
    /// in whatever window is on top, and an application that raises itself, such as a chat client, can
    /// also take the keyboard focus away between two clicks.
    /// </summary>
    /// <param name="windowHandle">Native window handle.</param>
    public static void KeepOnTop(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
        {
            return;
        }

        SetWindowPos(windowHandle, HwndTopMost, 0, 0, 0, 0, SwpNoMove | SwpNoSize | SwpNoActivate);
        Thread.Sleep(60);
    }

    /// <summary>
    /// Keeps the display and the system awake for the duration of the run. A blanked or sleeping display
    /// swallows synthetic input before the application can receive it.
    /// </summary>
    public static void KeepDisplayOn() =>
        SetThreadExecutionState(EsContinuous | EsDisplayRequired | EsSystemRequired);

    /// <summary>
    /// Restores the default power behaviour after the run.
    /// </summary>
    public static void AllowDisplaySleep() => SetThreadExecutionState(EsContinuous);

    /// <summary>
    /// Brings the owning window of an element to the foreground and verifies that it really received the
    /// focus, retrying while another application holds it.
    /// </summary>
    /// <param name="element">Element inside the window that must be active.</param>
    public static void EnsureForeground(System.Windows.Automation.AutomationElement element)
    {
        var windowHandle = TopLevelWindow(element);
        if (windowHandle == IntPtr.Zero)
        {
            return;
        }

        for (var attempt = 0; attempt < 10; attempt++)
        {
            if (GetForegroundWindow() == windowHandle)
            {
                return;
            }

            Activate(windowHandle);
            Thread.Sleep(60);
        }

        // SetForegroundWindow is refused while another application owns the foreground. A real click on
        // the caption still activates the window under the cursor regardless of the foreground lock.
        for (var attempt = 0; attempt < 3; attempt++)
        {
            ClickCaption(windowHandle);
            if (GetForegroundWindow() == windowHandle)
            {
                return;
            }
        }
    }

    private static void ClickCaption(IntPtr windowHandle)
    {
        if (!GetWindowRect(windowHandle, out var bounds))
        {
            return;
        }

        var x = bounds.Left + ((bounds.Right - bounds.Left) / 2);
        var y = bounds.Top + 12;
        SetCursorPos(x, y);
        Thread.Sleep(40);
        Send([MouseEventInput(MouseEventLeftDown), MouseEventInput(MouseEventLeftUp)]);
        Thread.Sleep(200);
    }

    /// <summary>
    /// Sends a single virtual key to the window that owns the given element, bringing that window to the
    /// foreground first.
    /// </summary>
    /// <param name="owner">Element inside the window that must receive the key.</param>
    /// <param name="key">Virtual key to press.</param>
    /// <param name="control">Whether Control is held.</param>
    public static void PressIn(
        System.Windows.Automation.AutomationElement owner,
        VirtualKey key,
        bool control = false)
    {
        EnsureForeground(owner);
        Press(key, control);
    }

    /// <summary>
    /// Types text into the window that owns the given element, bringing that window to the foreground
    /// first.
    /// </summary>
    /// <param name="owner">Element inside the window that must receive the text.</param>
    /// <param name="text">Text to type.</param>
    public static void TypeTextIn(System.Windows.Automation.AutomationElement owner, string text)
    {
        EnsureForeground(owner);
        TypeText(text);
    }

    /// <summary>
    /// Sends a single virtual key directly to the top-level window through <c>WM_KEYDOWN</c> and
    /// <c>WM_KEYUP</c>. Use this when a global hotkey or a foreground race prevents <c>SendInput</c>
    /// from reaching the application under test; the application still receives a normal WPF key event.
    /// </summary>
    /// <param name="owner">Element inside the window that must receive the key.</param>
    /// <param name="key">Virtual key to press.</param>
    public static void PressWindowMessage(
        System.Windows.Automation.AutomationElement owner,
        VirtualKey key)
    {
        var windowHandle = TopLevelWindow(owner);
        if (windowHandle == IntPtr.Zero)
        {
            PressIn(owner, key);
            return;
        }

        Activate(windowHandle);
        PostMessage(windowHandle, 0x0100, (IntPtr)(int)key, IntPtr.Zero);
        Thread.Sleep(30);
        PostMessage(windowHandle, 0x0101, (IntPtr)(int)key, IntPtr.Zero);
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
    /// Moves the mouse pointer to the center of an element without clicking, which shows its tooltip.
    /// </summary>
    /// <param name="element">Target element.</param>
    public static void MoveTo(System.Windows.Automation.AutomationElement element)
    {
        var point = Center(element);
        SetCursorPos(point.X, point.Y);
    }

    /// <summary>
    /// Moves the mouse pointer to the top-left corner of the screen so an open tooltip closes.
    /// </summary>
    public static void MoveAway() => SetCursorPos(0, 0);

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
    /// Opens the context menu of an element by posting right-button window messages with client
    /// coordinates. This path does not depend on the foreground window owning the physical mouse
    /// cursor, which makes context-menu scenarios repeatable on a busy desktop.
    /// </summary>
    /// <param name="element">Element whose context menu is opened.</param>
    public static void RightClickWindowMessage(System.Windows.Automation.AutomationElement element)
    {
        var windowHandle = TopLevelWindow(element);
        if (windowHandle == IntPtr.Zero)
        {
            RightClick(element);
            return;
        }

        var point = Center(element);
        if (!ScreenToClient(windowHandle, ref point))
        {
            RightClick(element);
            return;
        }

        var packed = (IntPtr)((point.Y << 16) | (point.X & 0xFFFF));
        PostMessage(windowHandle, 0x0204, (IntPtr)0x0002, packed);
        Thread.Sleep(60);
        PostMessage(windowHandle, 0x0205, IntPtr.Zero, packed);
        Thread.Sleep(300);
    }

    /// <summary>
    /// Right-clicks the center of the element, which opens its context menu.
    /// </summary>
    /// <param name="element">Target element.</param>
    public static void RightClick(System.Windows.Automation.AutomationElement element)
    {
        Activate(TopLevelWindow(element));
        var point = Center(element);
        SetCursorPos(point.X, point.Y);
        Thread.Sleep(40);
        Send([MouseEventInput(MouseEventRightDown), MouseEventInput(MouseEventRightUp)]);
        Thread.Sleep(200);
    }

    /// <summary>
    /// Drags from the center of one element to a point inside another, for example to reorder a grid
    /// column. The drop point matters: a column header insert is chosen from the half of the target header
    /// that receives the pointer, so the center of the target would keep a column that already stands next
    /// to it in place.
    /// </summary>
    /// <param name="source">Element to start the drag on.</param>
    /// <param name="target">Element that receives the drop.</param>
    /// <param name="targetFraction">Horizontal drop position inside the target, from 0 (left) to 1 (right).</param>
    public static void Drag(
        System.Windows.Automation.AutomationElement source,
        System.Windows.Automation.AutomationElement target,
        double targetFraction = 0.5)
    {
        Activate(TopLevelWindow(source));
        var rectangle = target.Current.BoundingRectangle;
        var drop = new Point(
            (int)Math.Round(rectangle.Left + (rectangle.Width * targetFraction)),
            (int)Math.Round(rectangle.Top + (rectangle.Height / 2)));
        DragCore(Center(source), drop);
    }

    /// <summary>
    /// Drags the right edge of an element horizontally. Used to resize a grid column through its
    /// header gripper.
    /// </summary>
    /// <param name="element">Element whose right edge is dragged.</param>
    /// <param name="deltaX">Horizontal distance in pixels; positive widens the element.</param>
    public static void DragRightEdge(System.Windows.Automation.AutomationElement element, int deltaX)
    {
        Activate(TopLevelWindow(element));
        var rectangle = element.Current.BoundingRectangle;
        var from = new Point(
            (int)Math.Round(rectangle.Right) - 2,
            (int)Math.Round(rectangle.Top + (rectangle.Height / 2)));
        DragCore(from, new Point(from.X + deltaX, from.Y));
    }

    private static void DragCore(Point from, Point to)
    {
        SetCursorPos(from.X, from.Y);
        Thread.Sleep(120);
        Send([MouseEventInput(MouseEventLeftDown)]);
        Thread.Sleep(120);
        const int Steps = 12;
        for (var step = 1; step <= Steps; step++)
        {
            var x = from.X + ((to.X - from.X) * step / Steps);
            var y = from.Y + ((to.Y - from.Y) * step / Steps);
            SetCursorPos(x, y);
            Thread.Sleep(30);
        }

        Thread.Sleep(120);
        Send([MouseEventInput(MouseEventLeftUp)]);
        Thread.Sleep(250);
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
    /// <remarks>
    /// The walk stops at the first ancestor that owns a native window instead of at the outermost
    /// element of the control view. The control view of a top-level window continues with the desktop
    /// pane, whose handle belongs to the desktop and must never be activated.
    /// </remarks>
    /// <param name="element">Element inside the window.</param>
    /// <returns>Window handle, or <see cref="IntPtr.Zero"/> when it cannot be resolved.</returns>
    private static IntPtr TopLevelWindow(System.Windows.Automation.AutomationElement element)
    {
        var walker = System.Windows.Automation.TreeWalker.ControlViewWalker;
        var current = element;
        for (var depth = 0; depth < 32; depth++)
        {
            try
            {
                var handle = current.Current.NativeWindowHandle;
                if (handle != 0)
                {
                    return new IntPtr(handle);
                }
            }
            catch (System.Windows.Automation.ElementNotAvailableException)
            {
                return IntPtr.Zero;
            }

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
                return IntPtr.Zero;
            }

            current = parent;
        }

        return IntPtr.Zero;
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
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MonitorInfo
    {
        public int Size;
        public Rect Monitor;
        public Rect Work;
        public uint Flags;
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

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool PostMessage(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetCursorPos(int x, int y);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(IntPtr window);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(
        IntPtr window,
        IntPtr insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool ScreenToClient(IntPtr window, ref Point point);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    private static extern IntPtr GetWindowLongPtr(IntPtr window, int index);

    [DllImport("user32.dll")]
    private static extern void SwitchToThisWindow(IntPtr window, [MarshalAs(UnmanagedType.Bool)] bool altTab);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SystemParametersInfo(uint action, uint parameter, ref uint value, uint winIni);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr window, out Rect rectangle);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int index);

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromPoint(Point point, uint flags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(IntPtr monitor, ref MonitorInfo info);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsZoomed(IntPtr window);

    [DllImport("kernel32.dll")]
    private static extern uint SetThreadExecutionState(uint flags);

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
    private const int MaximizeWindow = 3;
    private const int SmXVirtualScreen = 76;
    private const int SmYVirtualScreen = 77;
    private const int SmCxVirtualScreen = 78;
    private const int SmCyVirtualScreen = 79;
    private const uint MonitorDefaultToPrimary = 0x00000001;
    private const int GwlExStyle = -20;
    private const long WsExTopMost = 0x00000008;
    private const uint EsSystemRequired = 0x00000001;
    private const uint EsDisplayRequired = 0x00000002;
    private const uint EsContinuous = 0x80000000;
    private const uint SpiGetForegroundLockTimeout = 0x2000;
    private const uint SpiSetForegroundLockTimeout = 0x2001;
    private const uint SpifSendChange = 0x0002;
    private static readonly IntPtr HwndTop = IntPtr.Zero;
    private static readonly IntPtr HwndTopMost = new(-1);
    private const uint SwpNoSize = 0x0001;
    private const uint SwpNoMove = 0x0002;
    private const uint SwpNoZOrder = 0x0004;
    private const uint SwpNoActivate = 0x0010;
    private const uint SwpShowWindow = 0x0040;
}

