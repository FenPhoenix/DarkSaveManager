﻿using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using DarkSaveManager.Forms.CustomControls;
using DarkSaveManager.Forms.WinFormsNative;

namespace DarkSaveManager.Forms;

internal static class ControlUtils
{
    #region Suspend/resume drawing

    internal static void SuspendDrawing(this Control control)
    {
        if (!control.IsHandleCreated || !control.Visible) return;
        Native.SendMessageW(control.Handle, Native.WM_SETREDRAW, false, IntPtr.Zero);
    }

    internal static void ResumeDrawing(this Control control, bool invalidateInsteadOfRefresh = false)
    {
        if (!control.IsHandleCreated || !control.Visible) return;
        Native.SendMessageW(control.Handle, Native.WM_SETREDRAW, true, IntPtr.Zero);
        if (invalidateInsteadOfRefresh)
        {
            control.Invalidate();
        }
        else
        {
            control.Refresh();
        }
    }

    #endregion

    #region Centering

    internal static void CenterH(this Control control, Control parent, bool clientSize = false)
    {
        int pWidth = clientSize ? parent.ClientSize.Width : parent.Width;
        control.Location = control.Location with { X = (pWidth / 2) - (control.Width / 2) };
    }

#if false
    internal static void CenterV(this Control control, Control parent)
    {
        control.Location = control.Location with { Y = (parent.Height / 2) - (control.Height / 2) };
    }
#endif

    internal static void CenterHV(this Control control, Control parent, bool clientSize = false)
    {
        int pWidth = clientSize ? parent.ClientSize.Width : parent.Width;
        int pHeight = clientSize ? parent.ClientSize.Height : parent.Height;
        control.Location = new Point((pWidth / 2) - (control.Width / 2), (pHeight / 2) - (control.Height / 2));
    }

    #endregion

    #region Autosizing

    /// <summary>
    /// Special case for buttons that need to be autosized but then have their width adjusted after the fact,
    /// and so are unable to have GrowAndShrink set.
    /// </summary>
    /// <param name="button"></param>
    /// <param name="text"></param>
    internal static void SetTextAutoSize(this Button button, string text)
    {
        button.Text = "";
        button.Width = 2;
        button.Text = text;
    }

    /// <summary>
    /// Sets a <see cref="Button"/>'s text, and autosizes and repositions the <see cref="Button"/> and a
    /// <see cref="TextBox"/> horizontally together to accommodate it.
    /// </summary>
    /// <param name="button"></param>
    /// <param name="textBox"></param>
    /// <param name="text"></param>
    internal static void SetTextForTextBoxButtonCombo(this Button button, TextBox textBox, string text)
    {
        AnchorStyles oldAnchor = button.Anchor;
        button.Anchor = AnchorStyles.Top | AnchorStyles.Left;

        int oldWidth = button.Width;

        button.Text = text;

        int diff =
            button.Width > oldWidth ? -(button.Width - oldWidth) :
            button.Width < oldWidth ? oldWidth - button.Width : 0;

        button.Left += diff;
        // For some reason the diff doesn't work when scaling is > 100% so, yeah
        textBox.Width = button.Left > textBox.Left ? (button.Left - textBox.Left) - 1 : 0;

        button.Anchor = oldAnchor;
    }

    #endregion

    internal static void HideFocusRectangle(this Control control) => Native.SendMessageW(
        control.Handle,
        Native.WM_CHANGEUISTATE,
        new IntPtr(Native.SetControlFocusToHidden),
        IntPtr.Zero);

    #region Text alignment flags

    internal static TextFormatFlags GetTextAlignmentFlags(ContentAlignment textAlign) => textAlign switch
    {
        ContentAlignment.TopLeft => TextFormatFlags.Top | TextFormatFlags.Left,
        ContentAlignment.TopCenter => TextFormatFlags.Top | TextFormatFlags.HorizontalCenter,
        ContentAlignment.TopRight => TextFormatFlags.Top | TextFormatFlags.Right,
        ContentAlignment.MiddleLeft => TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
        ContentAlignment.MiddleCenter => TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
        ContentAlignment.MiddleRight => TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
        ContentAlignment.BottomLeft => TextFormatFlags.Bottom | TextFormatFlags.Left,
        ContentAlignment.BottomCenter => TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter,
        ContentAlignment.BottomRight => TextFormatFlags.Bottom | TextFormatFlags.Right,
        _ => TextFormatFlags.Top | TextFormatFlags.Left,
    };

    internal static TextFormatFlags GetTextAlignmentFlags(DataGridViewContentAlignment align) => align switch
    {
        DataGridViewContentAlignment.TopLeft => TextFormatFlags.Top | TextFormatFlags.Left,
        DataGridViewContentAlignment.TopCenter => TextFormatFlags.Top | TextFormatFlags.HorizontalCenter,
        DataGridViewContentAlignment.TopRight => TextFormatFlags.Top | TextFormatFlags.Right,
        DataGridViewContentAlignment.MiddleLeft => TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
        DataGridViewContentAlignment.MiddleCenter => TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
        DataGridViewContentAlignment.MiddleRight => TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
        DataGridViewContentAlignment.BottomLeft => TextFormatFlags.Bottom | TextFormatFlags.Left,
        DataGridViewContentAlignment.BottomCenter => TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter,
        DataGridViewContentAlignment.BottomRight => TextFormatFlags.Bottom | TextFormatFlags.Right,
        _ => TextFormatFlags.Top | TextFormatFlags.Left,
    };

    #endregion

    #region Scrolling

    internal static Native.SCROLLINFO GetCurrentScrollInfo(IntPtr handle, int direction)
    {
        Native.SCROLLINFO si = new();
        si.cbSize = (uint)Marshal.SizeOf(si);
        si.fMask = (uint)Native.ScrollInfoMask.SIF_ALL;
        Native.GetScrollInfo(handle, direction, ref si);
        return si;
    }

    internal static void RepositionScroll(IntPtr handle, Native.SCROLLINFO si, int direction)
    {
        Native.SetScrollInfo(handle, direction, ref si, true);

        // Send a WM_*SCROLL scroll message using SB_THUMBTRACK as wParam
        // SB_THUMBTRACK: low-order word of wParam, si.nPos high-order word of wParam
        IntPtr ptrWParam = new(Native.SB_THUMBTRACK + (0x10000 * si.nPos));

        IntPtr wp = (long)ptrWParam >= 0 ? ptrWParam : (IntPtr)Native.SB_THUMBTRACK;
        Native.SendMessageW(handle, direction == Native.SB_VERT ? Native.WM_VSCROLL : Native.WM_HSCROLL, wp, IntPtr.Zero);
    }

    #endregion

    #region Dialogs

    internal static MessageBoxIcon GetIcon(MBoxIcon icon) => icon switch
    {
        MBoxIcon.Information => MessageBoxIcon.Information,
        MBoxIcon.Warning => MessageBoxIcon.Warning,
        MBoxIcon.Error => MessageBoxIcon.Error,
        _ => MessageBoxIcon.None,
    };

    internal static MessageBoxButtons GetButtons(MBoxButtons buttons) => buttons switch
    {
        MBoxButtons.OKCancel => MessageBoxButtons.OKCancel,
        MBoxButtons.YesNo => MessageBoxButtons.YesNo,
        MBoxButtons.YesNoCancel => MessageBoxButtons.YesNoCancel,
        _ => MessageBoxButtons.OK,
    };

    internal static MBoxButton DialogResultToMBoxButton(DialogResult dialogResult) => dialogResult switch
    {
        DialogResult.Yes => MBoxButton.Yes,
        DialogResult.No => MBoxButton.No,
        _ => MBoxButton.Cancel,
    };

    internal static void SetMessageBoxIcon(PictureBox pictureBox, MessageBoxIcon icon)
    {
        Native.SHSTOCKICONINFO sii = new();
        try
        {
            Native.SHSTOCKICONID sysIcon = icon switch
            {
                MessageBoxIcon.Error or
                MessageBoxIcon.Hand or
                MessageBoxIcon.Stop
                    => Native.SHSTOCKICONID.SIID_ERROR,
                MessageBoxIcon.Question
                    => Native.SHSTOCKICONID.SIID_HELP,
                MessageBoxIcon.Exclamation or
                MessageBoxIcon.Warning
                    => Native.SHSTOCKICONID.SIID_WARNING,
                MessageBoxIcon.Asterisk or
                MessageBoxIcon.Information
                    => Native.SHSTOCKICONID.SIID_INFO,
                _
                    => throw new ArgumentOutOfRangeException(),
            };

            sii.cbSize = (uint)Marshal.SizeOf(typeof(Native.SHSTOCKICONINFO));

            int result = Native.SHGetStockIconInfo(sysIcon, Native.SHGSI_ICON, ref sii);
            Marshal.ThrowExceptionForHR(result, new IntPtr(-1));

            pictureBox.Image = Icon.FromHandle(sii.hIcon).ToBitmap();
        }
        catch
        {
            // "Wrong style" image (different style from the MessageBox one) but better than nothing if the
            // above fails
            pictureBox.Image = icon switch
            {
                MessageBoxIcon.Error or
                MessageBoxIcon.Hand or
                MessageBoxIcon.Stop
                    => SystemIcons.Error.ToBitmap(),
                MessageBoxIcon.Question
                    => SystemIcons.Question.ToBitmap(),
                MessageBoxIcon.Exclamation or
                MessageBoxIcon.Warning
                    => SystemIcons.Warning.ToBitmap(),
                MessageBoxIcon.Asterisk or
                MessageBoxIcon.Information
                    => SystemIcons.Information.ToBitmap(),
                _
                    => null,
            };
        }
        finally
        {
            Native.DestroyIcon(sii.hIcon);
        }
    }

    internal static int GetFlowLayoutPanelControlsWidthAll(FlowLayoutPanel flp)
    {
        int ret = 0;
        for (int i = 0; i < flp.Controls.Count; i++)
        {
            Control c = flp.Controls[i];
            ret += c.Margin.Horizontal + c.Width;
        }
        ret += flp.Padding.Horizontal;

        return ret;
    }

    /// <summary>
    /// Shows a dialog with hooked theming disabled, to prevent themed elements being drawn in the unthemed
    /// dialog (half-dark tooltips, dark scrollbars, etc.)
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    internal static DialogResult ShowDialogDark(this CommonDialog dialog, IWin32Window? owner)
    {
        using (new Win32ThemeHooks.DialogScope(active: Config.DarkMode))
        {
            return dialog.ShowDialog(owner);
        }
    }

    /// <summary>
    /// Just redirects to ShowDialog(), but it's so I can make every call ShowDialogDark() so if I find any
    /// ShowDialog() call I'll know it could be a bug (if it's calling a built-in dialog).
    /// </summary>
    /// <param name="dialog"></param>
    /// <returns></returns>
    internal static DialogResult ShowDialogDark(this Form dialog) => dialog.ShowDialog();

    /// <summary>
    /// Just redirects to ShowDialog(), but it's so I can make every call ShowDialogDark() so if I find any
    /// ShowDialog() call I'll know it could be a bug (if it's calling a built-in dialog).
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="owner"></param>
    /// <returns></returns>
    internal static DialogResult ShowDialogDark(this Form dialog, IWin32Window? owner) => dialog.ShowDialog(owner);

    #endregion

    #region ToolTips

    // Make ABSOLUTELY SURE we're able to do every single reflection thing we need to do to make this work.
    // Any failure whatsoever should result in falling back to classic-mode tooltips so we don't end up with
    // unreadable text!

    private static MethodInfo? _toolTipRecreateHandleMethod;
    private static FieldInfo? _toolTipNativeWindowControlField;
    private static Type? _toolTipNativeWindowClass;

    private static bool? _toolTipsReflectable;

    [MemberNotNullWhen(true,
        nameof(_toolTipRecreateHandleMethod),
        nameof(_toolTipNativeWindowControlField),
        nameof(_toolTipNativeWindowClass))]
    internal static bool ToolTipsReflectable
    {
        get
        {
            if (_toolTipsReflectable == null)
            {
                using ToolTip testToolTip = new();

                const BindingFlags bindingFlags =
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance;

                #region Check for RecreateHandle method

                try
                {
                    _toolTipRecreateHandleMethod = typeof(ToolTip)
                        .GetMethod(
                            "RecreateHandle",
                            bindingFlags);

                    if (_toolTipRecreateHandleMethod == null)
                    {
                        return SetFalse();
                    }

                    // Make sure we can invoke the method and it has the right param count (none in this case)
                    _toolTipRecreateHandleMethod.Invoke(testToolTip, null);
                }
                catch
                {
                    return SetFalse();
                }

                #endregion

                #region Check for nested ToolTipNativeWindow class

                try
                {
                    _toolTipNativeWindowClass = typeof(ToolTip)
                        .GetNestedType(
                            "ToolTipNativeWindow",
                            bindingFlags);

                    if (_toolTipNativeWindowClass == null)
                    {
                        return SetFalse();
                    }
                }
                catch
                {
                    return SetFalse();
                }

                #endregion

                #region Check for ToolTip field in ToolTipNativeWindow class

                try
                {
                    _toolTipNativeWindowControlField = _toolTipNativeWindowClass
                        .GetField(WinFormsReflection.ToolTipNativeWindow_ToolTipFieldName,
                            bindingFlags);

                    if (_toolTipNativeWindowControlField == null ||
                        _toolTipNativeWindowControlField.FieldType != typeof(ToolTip))
                    {
                        return SetFalse();
                    }
                }
                catch
                {
                    return SetFalse();
                }

                #endregion

                #region Check for the ToolTip field to be initialized

                object? tsNativeWindow = null;
                try
                {
                    ConstructorInfo[] constructors = _toolTipNativeWindowClass
                        .GetConstructors(
                            bindingFlags);

                    if (constructors.Length != 1)
                    {
                        return SetFalse();
                    }

                    ConstructorInfo? cons = constructors[0];

                    if (cons == null!)
                    {
                        return SetFalse();
                    }

                    tsNativeWindow = Activator.CreateInstance(
                        type: _toolTipNativeWindowClass,
                        bindingAttr: bindingFlags,
                        binder: null,
                        args: new object[] { testToolTip },
                        culture: CultureInfo.InvariantCulture);

                    if (tsNativeWindow == null)
                    {
                        return SetFalse();
                    }

                    // At this point, we know we have only one instance constructor, that it takes one param
                    // of type ToolTip, and that there is a ToolTip field in the class. If getting the ToolTip
                    // field's value succeeds and its value is not null, then we know this field is guaranteed
                    // to be initialized by the time the ToolTipNativeWindow class is constructed.
                    if (_toolTipNativeWindowControlField.GetValue(tsNativeWindow) == null)
                    {
                        return SetFalse();
                    }

                    _toolTipsReflectable = true;
                }
                catch
                {
                    return SetFalse();
                }
                finally
                {
                    // Ultra paranoid cleanup - this isn't disposable in .NET Framework 4.7.2 at the very least,
                    // but in theory it could be, so dispose it if so!
                    if (tsNativeWindow is IDisposable tsNativeWindowDisposable)
                    {
                        tsNativeWindowDisposable.Dispose();
                    }
                }

                #endregion
            }

            return _toolTipsReflectable == true;

            static bool SetFalse()
            {
                _toolTipRecreateHandleMethod = null;
                _toolTipNativeWindowControlField = null;
                _toolTipNativeWindowClass = null;
                _toolTipsReflectable = false;
                return false;
            }
        }
    }

    // Tooltips cache their text color (though not their other colors), so they only call the GetThemeColor()
    // method once. That means we can't change their text color at will. Recreating their handles causes them
    // to call GetThemeColor() again, so we can set their text color again.
    // We also can't try to do clever subclassing stuff, because some controls have internal tooltips that we
    // can't set to our subclassed version.
    internal static void RecreateAllToolTipHandles()
    {
        if (!ToolTipsReflectable) return;

        try
        {
            List<IntPtr> handles = Native.GetProcessWindowHandles();
            foreach (IntPtr handle in handles)
            {
                NativeWindow? nw = NativeWindow.FromHandle(handle);
                if (nw == null || nw.GetType() != _toolTipNativeWindowClass) continue;

                _toolTipRecreateHandleMethod.Invoke((ToolTip)_toolTipNativeWindowControlField.GetValue(nw)!, null);
            }
        }
        catch (Exception ex)
        {
            // Because of our exhaustive reflection checks, there should be no exceptions here unless something
            // very, very weird happens. If we do get an exception here, then we'll have wrong-colored (and
            // possibly unreadable) text in tooltips until a restart.
            Logger.Log(
                "Unable to recreate tooltip handles, meaning their text color has not changed with dark/light theme.",
                ex);
        }
    }

    #endregion

    internal static void DisposeAndClear(this Control.ControlCollection controlCollection)
    {
        foreach (Control? control in controlCollection)
        {
            control?.Dispose();
        }
        controlCollection.Clear();
    }

    internal static void SetFontStyle(this Control control, FontStyle fontStyle)
    {
        Font f = control.Font;
        control.Font = new Font(
            f.FontFamily,
            f.Size,
            fontStyle,
            f.Unit,
            f.GdiCharSet,
            f.GdiVerticalFont);
    }

    internal static bool IsMenuKey(KeyEventArgs e) => e.KeyCode == Keys.Apps || (e.Shift && e.KeyCode == Keys.F10);

    #region Theming

    internal sealed class ControlOriginalColors
    {
        internal readonly Color ForeColor;
        internal readonly Color BackColor;

        internal ControlOriginalColors(Color foreColor, Color backColor)
        {
            ForeColor = foreColor;
            BackColor = backColor;
        }
    }

    private static void FillControlColorList(
        Control control,
        List<KeyValuePair<Control, ControlOriginalColors?>>? controlColors,
        bool createControlHandles,
        Func<Control, bool>? createHandlePredicate = null)
    {
        if (controlColors != null && control.Tag is not LoadType.Lazy)
        {
            ControlOriginalColors? origColors = control is IDarkable
                ? null
                : new ControlOriginalColors(control.ForeColor, control.BackColor);
            controlColors.Add(new KeyValuePair<Control, ControlOriginalColors?>(control, origColors));
        }

        if (createControlHandles &&
            createHandlePredicate?.Invoke(control) != false &&
            !control.IsHandleCreated)
        {
            _ = control.Handle;
        }

        Control.ControlCollection controls = control.Controls;
        int count = controls.Count;
        for (int i = 0; i < count; i++)
        {
            FillControlColorList(
                control: controls[i],
                controlColors: controlColors,
                createControlHandles: createControlHandles,
                createHandlePredicate: createHandlePredicate
            );
        }
    }

    internal static void CreateAllControlsHandles(Control control, Func<Control, bool> createHandlePredicate)
    {
        FillControlColorList(
            control: control,
            controlColors: null,
            createControlHandles: true,
            createHandlePredicate: createHandlePredicate);
    }

    internal static void SetTheme(
        Control baseControl,
        List<KeyValuePair<Control, ControlOriginalColors?>> controlColors,
        VisualTheme theme,
        Func<Component, bool>? excludePredicate = null,
        bool createControlHandles = false,
        Func<Control, bool>? createHandlePredicate = null,
        int capacity = -1)
    {
        bool darkMode = theme == VisualTheme.Dark;

        // @DarkModeNote(FillControlColorList): Controls might change their colors after construct
        // Remember to handle this if new controls are added that this applies to.
        if (controlColors.Count == 0)
        {
            if (capacity >= 0) controlColors.Capacity = capacity;
            FillControlColorList(
                control: baseControl,
                controlColors: (List<KeyValuePair<Control, ControlOriginalColors?>>?)controlColors,
                createControlHandles: createControlHandles,
                createHandlePredicate: createHandlePredicate);
        }

        foreach (var item in controlColors)
        {
            Control control = item.Key;

            // Separate if because a control could be IDarkable AND be a ToolStrip
            if (control is ToolStrip ts)
            {
                foreach (ToolStripItem tsItem in ts.Items)
                {
                    if (tsItem is IDarkable darkableTSItem && (excludePredicate == null || !excludePredicate(tsItem)))
                    {
                        darkableTSItem.DarkModeEnabled = darkMode;
                    }
                }
            }

            // We might want to exclude a ToolStrip but not its subcomponents, so we put this check after the
            // ToolStrip component check
            if (excludePredicate?.Invoke(control) == true) continue;

            if (control is IDarkable darkableControl)
            {
                darkableControl.DarkModeEnabled = darkMode;
            }
            else
            {
                (control.ForeColor, control.BackColor) =
                    darkMode
                        ? (DarkColors.LightText, DarkColors.Fen_ControlBackground)
                        : (item.Value!.ForeColor, item.Value!.BackColor);
            }
        }
    }

    #endregion

    internal static void DrawCheckMark(Graphics g, Pen pen, RectangleF rect)
    {
        SmoothingMode oldSmoothingMode = g.SmoothingMode;
        g.SmoothingMode = SmoothingMode.HighQuality;

        // First half of checkmark
        g.DrawLine(
            pen,
            rect.Left + 1.5f,
            rect.Top + 6,
            rect.Left + 4.5f,
            rect.Top + 9);

        // Second half of checkmark
        g.DrawLine(
            pen,
            rect.Left + 4.5f,
            rect.Top + 9,
            rect.Left + 10.5f,
            rect.Top + 3);

        g.SmoothingMode = oldSmoothingMode;
    }

    /// <summary>
    /// For text that goes in menus: "&" is a reserved character, so escape "&" to "&&"
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static string EscapeAmpersands(this string value) => value.Replace("&", "&&");

    internal static string ToStringOrEmpty(this object? obj) => obj?.ToString() ?? "";

    internal static bool SystemThemeHasChanged(ref Message m, out VisualTheme newTheme)
    {
        if (m.Msg == Native.WM_SETTINGCHANGE &&
            Config.FollowSystemTheme &&
            Marshal.PtrToStringUni(m.LParam) is "ImmersiveColorSet")
        {
            newTheme = Core.GetSystemTheme();
            if (newTheme != Config.VisualTheme)
            {
                return true;
            }
        }

        newTheme = default;
        return false;
    }

    /*
    The docs are confusing about this. They mention the value "5000" several times, and then they say this:
    "The maximum time you can delay a popup is 5000 milliseconds." If you're just breezing over that line, you
    might read that as "the maximum time you can display a popup for is 5000 milliseconds". But that's false,
    the maximum display time is 32767 milliseconds. But it doesn't say that anywhere on the page, and the fact
    that this is the help page for AutoPopDelay and NOT InitialDelay means you're expecting any line vaguely
    saying "maximum" and then a number will be the maximum value for the property the page is about. But nope.
    */
    internal static void SetMaxDelay(this ToolTip toolTip)
    {
        if (!WinVersion.SupportsPersistentToolTips)
        {
            toolTip.AutoPopDelay = 32767;
        }
    }

    internal static void DrawFocusRectangle(Control c, Graphics g, Rectangle rect, Color color)
    {
        // This draws around the entire control rather than just around the text, but meh. It's good enough.
        // I don't know if there's any way to get the text rectangle reliably, so let's just leave it like
        // this.
        ControlPaint.DrawFocusRectangle(
            g,
            rect,
            // This method doesn't even use the foreColor value. Straight up it just ignores it and passes
            // only backColor. Meh?
            c.ForeColor,
            color);
    }
}
