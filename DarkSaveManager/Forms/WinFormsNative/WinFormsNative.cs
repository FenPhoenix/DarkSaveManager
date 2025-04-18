﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace DarkSaveManager.Forms.WinFormsNative;

[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "CommentTypo")]
[SuppressMessage("ReSharper", "StringLiteralTypo")]
internal static partial class Native
{
    internal const int WM_SETREDRAW = 0x000B;
    internal const int WM_NCPAINT = 0x0085;
    internal const int WM_PAINT = 0x000F;
    internal const int WM_ERASEBKGND = 0x0014;
    internal const int WM_MOVE = 0x0003;
    internal const int WM_SIZE = 0x0005;
    internal const int WM_WINDOWPOSCHANGED = 0x0047;
    internal const int WM_ENABLE = 0x000A;
    internal const int WM_CONTEXTMENU = 0x007B;

    internal const uint WM_CTLCOLORLISTBOX = 0x0134;
    internal const int SWP_NOSIZE = 0x0001;

    internal const int STATE_SYSTEM_INVISIBLE = 0x00008000;
    internal const int STATE_SYSTEM_UNAVAILABLE = 0x00000001;

    internal const int WM_SETTINGCHANGE = 0x001A;

    internal const int WM_PRINT = 0x0317;
    internal const int WM_PRINTCLIENT = 0x0318;

    [PublicAPI]
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct RECT
    {
        public readonly int left;
        public readonly int top;
        public readonly int right;
        public readonly int bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        internal int Width => right - left;

        internal Rectangle ToRectangle() => Rectangle.FromLTRB(left, top, right, bottom);
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct POINT(int x, int y)
    {
        public readonly int X = x;
        public readonly int Y = y;
    }

    #region Cursor

    public static Point ClientCursorPos(this Control c) => c.PointToClient(Cursor.Position);

    #endregion

    #region SendMessageW/PostMessageW

    [LibraryImport("user32.dll")]
    internal static partial IntPtr PostMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    [LibraryImport("user32.dll")]
    internal static partial IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    [LibraryImport("user32.dll")]
    internal static partial int SendMessageW(IntPtr hWnd, int wMsg, [MarshalAs(UnmanagedType.Bool)] bool wParam, IntPtr lParam);

    [LibraryImport("user32.dll")]
    internal static partial void SendMessageW(IntPtr hWnd, int wMsg, IntPtr wParam, ref DATETIMEPICKERINFO lParam);

    #endregion

    #region Control-specific

    #region ListView

    private const int LVM_FIRST = 0x1000;
    internal const int LVM_SETITEMA = LVM_FIRST + 6;
    internal const int LVM_SETITEMW = LVM_FIRST + 76;
    internal const int LVM_SETITEMTEXTA = LVM_FIRST + 46;
    internal const int LVM_SETITEMTEXTW = LVM_FIRST + 116;
    internal const int LVM_INSERTITEMA = LVM_FIRST + 7;
    internal const int LVM_INSERTITEMW = LVM_FIRST + 77;
    internal const int LVM_DELETEITEM = LVM_FIRST + 8;
    internal const int LVM_DELETEALLITEMS = LVM_FIRST + 9;

    #endregion

    #region MessageBox/TaskDialog

    internal enum SHSTOCKICONID : uint
    {
        SIID_HELP = 23,
        SIID_WARNING = 78,
        SIID_INFO = 79,
        SIID_ERROR = 80,
    }

    internal const uint SHGSI_ICON = 0x000000100;

    [PublicAPI]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal unsafe struct SHSTOCKICONINFO
    {
        internal uint cbSize;
        internal IntPtr hIcon;
        internal int iSysIconIndex;
        internal int iIcon;
        internal fixed char szPath[MAX_PATH];
    }

    [LibraryImport("Shell32.dll", SetLastError = false)]
    internal static partial int SHGetStockIconInfo(SHSTOCKICONID siid, uint uFlags, ref SHSTOCKICONINFO psii);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool DestroyIcon(IntPtr hIcon);

    #endregion

    #endregion

    #region Device context

    [LibraryImport("user32.dll")]
    private static partial IntPtr GetWindowDC(IntPtr hWnd);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [StructLayout(LayoutKind.Auto)]
    public readonly ref struct GraphicsContext
    {
        private readonly IntPtr _hWnd;
        private readonly IntPtr _dc;
        public readonly Graphics G;

        public GraphicsContext(IntPtr hWnd)
        {
            _hWnd = hWnd;
            _dc = GetWindowDC(_hWnd);
            G = Graphics.FromHdc(_dc);
        }

        public void Dispose()
        {
            G.Dispose();
            ReleaseDC(_hWnd, _dc);
        }
    }

    #endregion

    #region Window

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [LibraryImport("user32.dll", EntryPoint = "WindowFromPoint")]
    private static partial IntPtr WindowFromPoint_Native(POINT pt);

    internal static IntPtr WindowFromPoint(Point pt) => WindowFromPoint_Native(new POINT(pt.X, pt.Y));

    #endregion

    #region lParam/wParam

    // This code is straight from the .NET source, so keep it exactly the same despite "unnecessary cast" warnings

    // ReSharper disable RedundantCast
#pragma warning disable IDE0004
    internal static IntPtr MAKELPARAM(int low, int high) => (IntPtr)((high << 16) | (low & 0xffff));
#if false
    internal static int MAKELONG(int low, int high) => (high << 16) | (low & 0xffff);
    internal static int HIWORD(int n) => (n >> 16) & 0xffff;
    internal static int HIWORD(IntPtr n) => HIWORD(unchecked((int)(long)n));
    internal static int LOWORD(IntPtr n) => LOWORD(unchecked((int)(long)n));
    internal static int LOWORD(int n) => n & 0xffff;
#endif
    internal static int SignedHIWORD(IntPtr n) => SignedHIWORD(unchecked((int)(long)n));
    internal static int SignedLOWORD(IntPtr n) => SignedLOWORD(unchecked((int)(long)n));
    internal static int SignedHIWORD(int n) => (int)(short)((n >> 16) & 0xffff);
    internal static int SignedLOWORD(int n) => (int)(short)(n & 0xFFFF);
#pragma warning restore IDE0004
    // ReSharper restore RedundantCast

    #endregion

    #region Hide focus rectangle

    internal const int WM_CHANGEUISTATE = 0x0127;

    private const int UIS_SET = 1;
    //internal const int UIS_CLEAR = 2;
    //internal const int UIS_INITIALIZE = 3;

    private const int UISF_HIDEFOCUS = 1;
    //internal const int UISF_HIDEACCEL = 2;
    //internal const int UISF_ACTIVE = 4;

    internal const int SetControlFocusToHidden = UISF_HIDEFOCUS + (UIS_SET << 16);

    #endregion

    #region Mouse

    // NC prefix means the mouse was over a non-client area

    internal const int WM_MOUSEWHEEL = 0x20A;
    internal const int WM_MOUSEHWHEEL = 0x020E; // Mousewheel tilt

    internal const int WM_MOUSEMOVE = 0x200;
    internal const int WM_NCMOUSEMOVE = 0xA0;

    internal const int WM_MOUSELEAVE = 0x02A3;

    internal const int WM_LBUTTONUP = 0x202;
    internal const int WM_NCLBUTTONUP = 0x00A2;
    internal const int WM_MBUTTONUP = 0x208;
    internal const int WM_NCMBUTTONUP = 0xA8;
    internal const int WM_RBUTTONUP = 0x205;
    internal const int WM_NCRBUTTONUP = 0xA5;

    internal const int WM_LBUTTONDOWN = 0x201;
    internal const int WM_NCLBUTTONDOWN = 0x00A1;
    internal const int WM_MBUTTONDOWN = 0x207;
    internal const int WM_NCMBUTTONDOWN = 0xA7;
    internal const int WM_RBUTTONDOWN = 0x204;
    internal const int WM_NCRBUTTONDOWN = 0xA4;

    internal const int WM_LBUTTONDBLCLK = 0x203;
    internal const int WM_NCLBUTTONDBLCLK = 0xA3;
    internal const int WM_MBUTTONDBLCLK = 0x209;
    internal const int WM_NCMBUTTONDBLCLK = 0xA9;
    internal const int WM_RBUTTONDBLCLK = 0x206;
    internal const int WM_NCRBUTTONDBLCLK = 0xA6;

    #endregion

    #region Keyboard

    internal const int WM_KEYDOWN = 0x100;
    internal const int WM_SYSKEYDOWN = 0x104;
    internal const int WM_SYSKEYUP = 0x105;
    internal const int WM_KEYUP = 0x101;

    // VK_ only to be used in keyboard messages
#if false
    internal const int VK_SHIFT = 0x10;
    internal const int VK_CONTROL = 0x11;
    internal const int VK_ALT = 0x12; // this is supposed to be called VK_MENU but screw that
    internal const int VK_ESCAPE = 0x1B;
    internal const int VK_PAGEUP = 0x21; // VK_PRIOR
    internal const int VK_PAGEDOWN = 0x22; // VK_NEXT
    internal const int VK_END = 0x23;
    internal const int VK_HOME = 0x24;
    internal const int VK_LEFT = 0x25;
    internal const int VK_UP = 0x26;
    internal const int VK_RIGHT = 0x27;
    internal const int VK_DOWN = 0x28;
#endif

    #endregion

    #region Scrolling / scroll bars

    internal const uint OBJID_HSCROLL = 0xFFFFFFFA;
    internal const uint OBJID_VSCROLL = 0xFFFFFFFB;

    internal const int SB_HORZ = 0;
    internal const int SB_VERT = 1;

    internal const int WM_VSCROLL = 0x115;
    internal const int WM_HSCROLL = 0x114;

#if false
    internal const int SB_LINEUP = 0;
    internal const int SB_LINEDOWN = 1;
    internal const int SB_PAGEUP = 2;
    internal const int SB_PAGELEFT = 2;
    internal const int SB_PAGEDOWN = 3;
    internal const int SB_PAGERIGHT = 3;
    internal const int SB_PAGETOP = 6;
    internal const int SB_LEFT = 6;
    internal const int SB_PAGEBOTTOM = 7;
    internal const int SB_RIGHT = 7;
    internal const int SB_ENDSCROLL = 8;
    internal const int SBM_GETPOS = 225;
    internal const int SB_HORZ = 0;
#endif
    internal const uint SB_THUMBTRACK = 5;

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [StructLayout(LayoutKind.Sequential)]
    internal struct SCROLLINFO
    {
        internal uint cbSize;
        internal uint fMask;
        internal int nMin;
        internal int nMax;
        internal uint nPage;
        internal int nPos;
        internal int nTrackPos;
    }

    [PublicAPI]
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct SCROLLBARINFO
    {
        internal int cbSize;
        internal RECT rcScrollBar;
        internal int dxyLineButton;
        internal int xyThumbTop;
        internal int xyThumbBottom;
        internal int reserved;
        internal fixed int rgstate[6];
    }

    [Flags]
    internal enum ScrollInfoMask
    {
        SIF_RANGE = 0x0001,
        SIF_PAGE = 0x0002,
        SIF_POS = 0x0004,
        //SIF_DISABLENOSCROLL = 0x0008,
        SIF_TRACKPOS = 0x0010,
        SIF_ALL = SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS,
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);

    [LibraryImport("user32.dll")]
    internal static partial int SetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi, [MarshalAs(UnmanagedType.Bool)] bool fRedraw);

    [LibraryImport("user32.dll", EntryPoint = "GetScrollBarInfo", SetLastError = true)]
    internal static partial int GetScrollBarInfo(IntPtr hWnd, uint idObject, ref SCROLLBARINFO psbi);

    #endregion

    #region Theming

    internal const int WM_THEMECHANGED = 0x031A;

    internal const int TMT_FILLCOLOR = 3802;
    internal const int TMT_TEXTCOLOR = 3803;

    #region Trackbar parts

    internal const int TKP_TRACK = 1;
    //internal const int TKP_TRACKVERT = 2;
    //internal const int TKP_THUMB = 3;
    internal const int TKP_THUMBBOTTOM = 4;
    //internal const int TKP_THUMBTOP = 5;
    //internal const int TKP_THUMBVERT = 6;
    //internal const int TKP_THUMBLEFT = 7;
    //internal const int TKP_THUMBRIGHT = 8;
    internal const int TKP_TICS = 9;
    //internal const int TKP_TICSVERT = 10;

#if false
    internal const int TKS_NORMAL = 1;
    internal const int TRS_NORMAL = 1;
    internal const int TRVS_NORMAL = 1;

    internal const int TUS_NORMAL = 1;
    internal const int TUS_HOT = 2;
    internal const int TUS_PRESSED = 3;
    internal const int TUS_FOCUSED = 4;
    internal const int TUS_DISABLED = 5;
#endif

    //internal const int TUBS_NORMAL = 1;
    internal const int TUBS_HOT = 2;
    internal const int TUBS_PRESSED = 3;
    //internal const int TUBS_FOCUSED = 4;
    internal const int TUBS_DISABLED = 5;

#if false
    internal const int TUTS_NORMAL = 1;
    internal const int TUTS_HOT = 2;
    internal const int TUTS_PRESSED = 3;
    internal const int TUTS_FOCUSED = 4;
    internal const int TUTS_DISABLED = 5;

    internal const int TUVS_NORMAL = 1;
    internal const int TUVS_HOT = 2;
    internal const int TUVS_PRESSED = 3;
    internal const int TUVS_FOCUSED = 4;
    internal const int TUVS_DISABLED = 5;

    internal const int TUVLS_NORMAL = 1;
    internal const int TUVLS_HOT = 2;
    internal const int TUVLS_PRESSED = 3;
    internal const int TUVLS_FOCUSED = 4;
    internal const int TUVLS_DISABLED = 5;

    internal const int TUVRS_NORMAL = 1;
    internal const int TUVRS_HOT = 2;
    internal const int TUVRS_PRESSED = 3;
    internal const int TUVRS_FOCUSED = 4;
    internal const int TUVRS_DISABLED = 5;
#endif

    internal const int TSS_NORMAL = 1;

#if false
    internal const int TSVS_NORMAL = 1;
#endif

    #endregion

    #region ToolTip parts

    internal const int TTP_STANDARD = 1;
    internal const int TTP_STANDARDTITLE = 2;
#if false
    internal const int TTP_BALLOON = 3;
    internal const int TTP_BALLOONTITLE = 4;
    internal const int TTP_CLOSE = 5;
    internal const int TTP_BALLOONSTEM = 6;
    internal const int TTP_WRENCH = 7;
#endif

    #endregion

    #region DateTimePicker

    internal const int DTM_GETDATETIMEPICKERINFO = 0x100E;

    //internal const int STATE_SYSTEM_HOTTRACKED = 0x00000080;
    internal const int STATE_SYSTEM_PRESSED = 0x00000008;

    [StructLayout(LayoutKind.Sequential)]
    [PublicAPI]
    internal struct DATETIMEPICKERINFO
    {
        internal int cbSize;
        internal RECT rcCheck;
        internal int stateCheck;
        internal RECT rcButton;
        internal int stateButton;
        internal IntPtr hwndEdit;
        internal IntPtr hwndUD;
        internal IntPtr hwndDropDown;
    }

    #endregion

    #region Spinner parts

    internal const int SPNP_UPHORZ = 3;
    internal const int SPNP_DOWNHORZ = 4;

    // Up and down states are the same set of values
    internal const int UP_OR_DOWN_HZS_NORMAL = 1;
    internal const int UP_OR_DOWN_HZS_HOT = 2;
    internal const int UP_OR_DOWN_HZS_PRESSED = 3;
    internal const int UP_OR_DOWN_HZS_DISABLED = 4;

    #endregion

    #region Scroll bar parts

    internal const int SBP_ARROWBTN = 1;
    internal const int SBP_THUMBBTNHORZ = 2;
    internal const int SBP_THUMBBTNVERT = 3;
#if false
    internal const int SBP_LOWERTRACKHORZ = 4;
    internal const int SBP_UPPERTRACKHORZ = 5;
    internal const int SBP_LOWERTRACKVERT = 6;
    internal const int SBP_UPPERTRACKVERT = 7;
#endif
    internal const int SBP_GRIPPERHORZ = 8;
    internal const int SBP_GRIPPERVERT = 9;
#if false
    internal const int SBP_SIZEBOX = 10;
#endif
    // Uh, this one isn't listed in vsstyle.h, but it works...?
    internal const int SBP_CORNER = 11;

    #endregion

    #region Scroll bar arrow button states

    internal const int ABS_UPNORMAL = 1;
    internal const int ABS_UPHOT = 2;
    internal const int ABS_UPPRESSED = 3;
    internal const int ABS_UPDISABLED = 4;
    //internal const int ABS_DOWNNORMAL = 5;
    internal const int ABS_DOWNHOT = 6;
    internal const int ABS_DOWNPRESSED = 7;
    internal const int ABS_DOWNDISABLED = 8;
    internal const int ABS_LEFTNORMAL = 9;
    internal const int ABS_LEFTHOT = 10;
    internal const int ABS_LEFTPRESSED = 11;
    internal const int ABS_LEFTDISABLED = 12;
    internal const int ABS_RIGHTNORMAL = 13;
    internal const int ABS_RIGHTHOT = 14;
    internal const int ABS_RIGHTPRESSED = 15;
    internal const int ABS_RIGHTDISABLED = 16;
    internal const int ABS_UPHOVER = 17;
    //internal const int ABS_DOWNHOVER = 18;
    internal const int ABS_LEFTHOVER = 19;
    internal const int ABS_RIGHTHOVER = 20;

    #endregion

    #region Scroll bar thumb states

    internal const int SCRBS_NORMAL = 1;
    internal const int SCRBS_HOT = 2;
    internal const int SCRBS_PRESSED = 3;
    //internal const int SCRBS_DISABLED = 4;
    internal const int SCRBS_HOVER = 5;

    #endregion

    #region TreeView parts

    //internal const int TVP_TREEITEM = 1;
    internal const int TVP_GLYPH = 2;
    //internal const int TVP_BRANCH = 3;
    internal const int TVP_HOTGLYPH = 4;

    #endregion

    #region TreeView glyph states

    internal const int GLPS_CLOSED = 1;
    //internal const int GLPS_OPENED = 2;
    internal const int HGLPS_CLOSED = 1;
    //internal const int HGLPS_OPENED = 2;

    #endregion

    [LibraryImport("uxtheme.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial int SetWindowTheme(IntPtr hWnd, string appname, string idlist);

    [LibraryImport("uxtheme.dll", StringMarshalling = StringMarshalling.Utf16)]
    internal static partial IntPtr OpenThemeData(IntPtr hWnd, string classList);

    [LibraryImport("uxtheme.dll")]
    public static partial int CloseThemeData(IntPtr hTheme);

    [LibraryImport("uxtheme.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool IsThemeActive();

    [LibraryImport("gdi32.dll", SetLastError = true)]
    internal static partial IntPtr CreateSolidBrush(int crColor);

    // Ridiculous Windows using a different value on different versions...
    internal const int DWMWA_USE_IMMERSIVE_DARK_MODE_OLD = 19;
    internal const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

    [LibraryImport("dwmapi.dll")]
    internal static partial int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    #endregion

    #region Enumerate window handles

    private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool EnumThreadWindows(int dwThreadId, IntPtr lpfn, IntPtr lParam);

    internal static List<IntPtr> GetProcessWindowHandles()
    {
        List<IntPtr> handles = new();

        using Process currentProcess = Process.GetCurrentProcess();
        IntPtr callback = Marshal.GetFunctionPointerForDelegate<EnumThreadDelegate>(Callback);
        foreach (ProcessThread thread in currentProcess.Threads)
        {
            EnumThreadWindows(thread.Id, callback, IntPtr.Zero);
        }

        return handles;

        bool Callback(IntPtr hWnd, IntPtr _)
        {
            handles.Add(hWnd);
            return true;
        }
    }

    #endregion

    #region High contrast code from .NET latest runtime

    // Licensed to the .NET Foundation under one or more agreements.
    // The .NET Foundation licenses this file to you under the MIT license.

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SystemParametersInfoW(SystemParametersAction uiAction, uint uiParam, ref HIGHCONTRASTW pvParam, uint fWinIni);

    [Flags]
    private enum HIGHCONTRASTW_FLAGS : uint
    {
        HCF_HIGHCONTRASTON = 0x00000001,
        HCF_AVAILABLE = 0x00000002,
        HCF_HOTKEYACTIVE = 0x00000004,
        HCF_CONFIRMHOTKEY = 0x00000008,
        HCF_HOTKEYSOUND = 0x00000010,
        HCF_INDICATOR = 0x00000020,
        HCF_HOTKEYAVAILABLE = 0x00000040,
        HCF_OPTION_NOTHEMECHANGE = 0x00001000,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private unsafe struct HIGHCONTRASTW
    {
        internal uint cbSize;
        internal HIGHCONTRASTW_FLAGS dwFlags;
        internal void* lpszDefaultScheme;
    }

    private enum SystemParametersAction : uint
    {
        SPI_GETICONTITLELOGFONT = 0x1F,
        SPI_GETNONCLIENTMETRICS = 0x29,
        SPI_GETHIGHCONTRAST = 0x42,
    }

    internal static unsafe bool HighContrastEnabled()
    {
        HIGHCONTRASTW highContrast = default;

        // Note that the documentation for HIGHCONTRASTW says that the lpszDefaultScheme member needs to be
        // freed, but this is incorrect. No internal users ever free the pointer and the pointer never changes.
        highContrast.cbSize = (uint)sizeof(HIGHCONTRASTW);
        bool success = SystemParametersInfoW(
            SystemParametersAction.SPI_GETHIGHCONTRAST,
            highContrast.cbSize,
            ref highContrast,
            0); // This has no meaning when getting values

        return success && (highContrast.dwFlags & HIGHCONTRASTW_FLAGS.HCF_HIGHCONTRASTON) != 0;
    }

    #endregion
}
