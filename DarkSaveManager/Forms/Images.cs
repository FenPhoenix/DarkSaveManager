using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DarkSaveManager.Forms.CustomControls;
using DarkSaveManager.Properties;

namespace DarkSaveManager.Forms;

// This acts as both a cache for images that might be loaded more than once, and also a dark mode image
// switcher. Images are here because they need caching, because they have a dark mode version, or both.

// NOTE: Anything in here that a designer needs access to has to be public.

// Also performance hack for the splash screen as above.
file static class DarkModeImageConversion
{
    internal enum Matrix
    {
        Dark,
        DarkDim,
        DarkDisabled,
    }

    private static readonly ColorMatrix DarkColorMatrix = new(new[]
    {
        new[] { 0.2125f, 0.2125f, 0.2125f, 0, 0 },
        new[] { 0.2577f, 0.2577f, 0.2577f, 0, 0 },
        new[] { 0.0361f, 0.0361f, 0.0361f, 0, 0 },
        new[] { 0, 0, 0, /* The value: */ 0.8425f, 0 },
        new[] { 0.99f, 0.99f, 0.99f, 0, 0 },
    });

    private static readonly ColorMatrix DarkDimColorMatrix = new(new[]
    {
        new[] { 0.2125f, 0.2125f, 0.2125f, 0, 0 },
        new[] { 0.2577f, 0.2577f, 0.2577f, 0, 0 },
        new[] { 0.0361f, 0.0361f, 0.0361f, 0, 0 },
        new[] { 0, 0, 0, /* The value: */ 0.7425f, 0 },
        new[] { 0.99f, 0.99f, 0.99f, 0, 0 },
    });

    private static readonly ColorMatrix DarkDisabledColorMatrix = new(new[]
    {
        new[] { 0.2125f, 0.2125f, 0.2125f, 0, 0 },
        new[] { 0.2577f, 0.2577f, 0.2577f, 0, 0 },
        new[] { 0.0361f, 0.0361f, 0.0361f, 0, 0 },
        new[] { 0, 0, 0, /* The value: */ 0.273f, 0 },
        new[] { 0.99f, 0.99f, 0.99f, 0, 0 },
    });

    public static Bitmap CreateDarkModeVersion(Bitmap normalImage, Matrix matrix = Matrix.Dark)
    {
        using ImageAttributes imgAttrib = new();

        imgAttrib.ClearColorKey();

        imgAttrib.SetColorMatrix(
            matrix switch
            {
                Matrix.DarkDisabled => DarkDisabledColorMatrix,
                Matrix.DarkDim => DarkDimColorMatrix,
                _ => DarkColorMatrix,
            });

        Size size = normalImage.Size;

        Bitmap darkModeImage = new(size.Width, size.Height);

        using Graphics graphics = Graphics.FromImage(darkModeImage);
        graphics.DrawImage(
            normalImage,
            new Rectangle(0, 0, size.Width, size.Height),
            0,
            0,
            size.Width,
            size.Height,
            GraphicsUnit.Pixel,
            imgAttrib);

        return darkModeImage;
    }
}

public static class Images
{
    #region Path points and types

    #region Update arrow

    private static readonly float[] _updateArrowPoints =
    {
        0,5, 5,0, 10,5, 6.5f,5, 6.5f,9, 3.5f,9, 3.5f,5,
    };

    private static readonly byte[] _updateArrowTypes = MakeTypeArray((1, 5, 0, 129));

    private static GraphicsPath? _updateArrowGPath;
    private static GraphicsPath UpdateArrowGPath => _updateArrowGPath ??= MakeGraphicsPath(_updateArrowPoints, _updateArrowTypes);

    #endregion

    #region X symbol

    private static readonly float[] _xPoints =
    {
        8, 32,
        32, 8,
        80, 58,
        128, 8,
        152, 32,
        102, 80,
        152, 128,
        128, 152,
        80, 102,
        32, 152,
        8, 128,
        58, 80,
        8, 32,
    };

    private static readonly byte[] _xTypes = MakeTypeArray((1, 11, 0, 129));

    private static GraphicsPath? _xGPath;
    private static GraphicsPath XGPath => _xGPath ??= MakeGraphicsPath(_xPoints, _xTypes);

    #endregion

    #region Vector points

    #region Plus / ex

    private static readonly Rectangle[] _plusRects = new Rectangle[2];

    #endregion

    #region Hamburger

    private static readonly Rectangle[] _hamRects24 =
    {
        new Rectangle(5, 5, 14, 2),
        new Rectangle(5, 11, 14, 2),
        new Rectangle(5, 17, 14, 2),
    };

    #endregion

    #endregion

    #endregion

    #region Colors / brushes / pens

    private sealed class ThemedPen
    {
        private readonly Color _color;
        private readonly Color _colorDark;

        private readonly float _width;

        private Pen? _pen;
        private Pen? _penDark;
        internal Pen Pen =>
            Config.DarkMode
                ? _penDark ??= new Pen(_colorDark, _width)
                : _pen ??= new Pen(_color, _width);

        internal ThemedPen(Color color, Color colorDark, float width = 1f)
        {
            _color = color;
            _colorDark = colorDark;
            _width = width;
        }

#if false
        public ThemedPen(Color color, Pen penDark, float width = 1f)
        {
            _color = color;
            _penDark = penDark;
            _width = width;
        }
#endif

        public ThemedPen(Pen pen, Color colorDark, float width = 1f)
        {
            _pen = pen;
            _colorDark = colorDark;
            _width = width;
        }

#if false
        public ThemedPen(Pen pen, Pen penDark, float width = 1f)
        {
            _pen = pen;
            _penDark = penDark;
            _width = width;
        }
#endif
    }

    private sealed class ThemedBrush
    {
        private readonly Color _color;
        private readonly Color _colorDark;

        private Brush? _brush;
        private Brush? _brushDark;
        internal Brush Brush =>
            Config.DarkMode
                ? _brushDark ??= new SolidBrush(_colorDark)
                : _brush ??= new SolidBrush(_color);

        internal ThemedBrush(Color color, Color colorDark)
        {
            _color = color;
            _colorDark = colorDark;
        }

#if false
        public ThemedBrush(Color color, Brush brushDark)
        {
            _color = color;
            _brushDark = brushDark;
        }
#endif

        public ThemedBrush(Brush brush, Color colorDark)
        {
            _brush = brush;
            _colorDark = colorDark;
        }

        public ThemedBrush(Brush brush, Brush brushDark)
        {
            _brush = brush;
            _brushDark = brushDark;
        }
    }

    private sealed class FillAndOutlineBrushes
    {
        private readonly Color _outlineColor;
        private readonly Color _outlineColorDark;
        private readonly Color _fillColor;
        private readonly Color _fillColorDark;

        private SolidBrush? _outline;
        private SolidBrush? _outlineDark;
        private SolidBrush? _fill;
        private SolidBrush? _fillDark;

        private readonly int _width;

        internal FillAndOutlineBrushes(Color outline, Color outlineDark, Color fill, Color fillDark, int width)
        {
            _outlineColor = outline;
            _outlineColorDark = outlineDark;
            _fillColor = fill;
            _fillColorDark = fillDark;
            _width = width;
        }

        internal FillAndOutlineBrushes(Color outline, Color outlineDark, Color fill, Color fillDark)
        {
            _outlineColor = outline;
            _outlineColorDark = outlineDark;
            _fillColor = fill;
            _fillColorDark = fillDark;
        }

        internal FillAndOutlineBrushes(Color outline, Color fill)
        {
            _outlineColor = outline;
            _outlineColorDark = outline;
            _fillColor = fill;
            _fillColorDark = fill;
        }

        internal (SolidBrush Outline, SolidBrush Fill, int Width) GetData()
        {
            return Config.DarkMode
                ? (_outlineDark ??= new SolidBrush(_outlineColorDark), _fillDark ??= new SolidBrush(_fillColorDark), Width: _width)
                : (_outline ??= new SolidBrush(_outlineColor), _fill ??= new SolidBrush(_fillColor), Width: _width);
        }
    }

    private static readonly ThemedBrush _deleteFromDBBrush = new(
        color: Color.FromArgb(135, 0, 0),
        colorDark: Color.FromArgb(209, 70, 70));

    #region Separators

    private static readonly Pen _sep1Pen = new Pen(Color.FromArgb(189, 189, 189));
    private static Pen? _sep1PenC;
    private static readonly Pen Sep2Pen = new Pen(Color.FromArgb(255, 255, 255));

    internal static Pen Sep1Pen =>
        Config.DarkMode
            ? DarkColors.GreySelectionPen
            : Application.RenderWithVisualStyles
                ? _sep1Pen
                : _sep1PenC ??= new Pen(Color.FromArgb(166, 166, 166));

    #endregion

    #region AL Blue

    private static readonly Color _al_LightBlue = Color.FromArgb(4, 125, 202);
    private static readonly Color _al_LightBlueDark = Color.FromArgb(54, 146, 204);

    private static readonly ThemedBrush _al_LightBlueBrush = new(
        color: _al_LightBlue,
        colorDark: _al_LightBlueDark
    );

    #endregion

    private static Brush BlackForegroundBrush => Config.DarkMode ? DarkColors.Fen_DarkForegroundBrush : Brushes.Black;
    private static Pen BlackForegroundPen => Config.DarkMode ? DarkColors.Fen_DarkForegroundPen : Pens.Black;

    #region Arrows

    private static Pen ArrowButtonEnabledPen => Config.DarkMode ? DarkColors.ArrowEnabledPen : SystemPens.ControlText;

    #endregion

    #endregion

    #region Raster

    #region Image arrays

    // Load this only once, as it's transparent and so doesn't have to change with the theme
    internal static readonly Bitmap Blank = new(1, 1, PixelFormat.Format32bppPArgb);

    #endregion

    #region Image properties

    private static readonly Dictionary<Image, Image> _disabledImages = new(25);
    internal static Image GetDisabledImage(Image image)
    {
        return _disabledImages.TryGetValue(image, out Image? result)
            ? result
            : _disabledImages.AddAndReturn(image, ToolStripRenderer.CreateDisabledImage(image));
    }

    private static Bitmap? _redExclCircle;
    private static Bitmap? _redExclCircle_Dark;
    public static Bitmap RedExclCircle =>
        Config.DarkMode
            ? _redExclCircle_Dark ??= Resources.RedExclCircle_Dark
            : _redExclCircle ??= Resources.RedExclCircle;

    #endregion

    #region Methods

    #region Vector helpers

    // Believe it or not, I actually save space by having this massive complicated method rather than a few
    // very small byte arrays. I guess byte arrays must take up more space than you might think, or something.
    private static byte[] MakeTypeArray(params (byte FillValue, int FillCount, int Prefix, int Suffix)[] sets)
    {
        int totalArrayLen = 0;
        foreach ((_, int fillCount, int prefix, int suffix) in sets)
        {
            totalArrayLen += fillCount + (prefix > -1 ? 1 : 0) + (suffix > -1 ? 1 : 0);
        }
        byte[] ret = new byte[totalArrayLen];

        int pos = 0;
        foreach ((byte fillValue, int fillCount, int prefix, int suffix) in sets)
        {
            if (prefix > -1) ret[pos++] = (byte)prefix;

            int j;
            for (j = pos; j < pos + fillCount; j++)
            {
                ret[j] = fillValue;
            }
            pos = j;

            if (suffix > -1) ret[pos++] = (byte)suffix;
        }

        return ret;
    }

    private static GraphicsPath MakeGraphicsPath(float[] points, byte[] types)
    {
        int pointsCount = points.Length;
        PointF[] rawPoints = new PointF[pointsCount / 2];
        for (int i = 0, j = 0; i < pointsCount; i += 2, j++)
        {
            rawPoints[j] = new PointF(points[i], points[i + 1]);
        }
        return new GraphicsPath(rawPoints, types);
    }

    internal static void FitRectInBounds(Graphics g, RectangleF drawRect, RectangleF boundsRect)
    {
        if (boundsRect.Width < 1 || boundsRect.Height < 1) return;

        g.ResetTransform();

        // Set scale origin
        float drawRectCenterX = drawRect.Left + (drawRect.Width / 2);
        float drawRectCenterY = drawRect.Top + (drawRect.Height / 2);
        // Er, yeah, I don't actually know why these have to be negated... but it works, so oh well...?
        g.TranslateTransform(-drawRectCenterX, -drawRectCenterY, MatrixOrder.Append);

        // Scale graphic
        float scaleBothAxes = Math.Min(boundsRect.Width / drawRect.Width, boundsRect.Height / drawRect.Height);
        g.ScaleTransform(scaleBothAxes, scaleBothAxes, MatrixOrder.Append);

        // Center graphic in bounding rectangle
        float boundsRectCenterX = boundsRect.Left + (boundsRect.Width / 2);
        float boundsRectCenterY = boundsRect.Top + (boundsRect.Height / 2);
        g.TranslateTransform(boundsRectCenterX, boundsRectCenterY, MatrixOrder.Append);
    }

    #endregion

    #endregion

    #endregion

    #region Vector

    // Normally you would use images pulled from Resources for this. But to avoid bloating up our executable
    // and bogging down startup time, we just draw images ourselves where it's reasonable to do so.

    #region Buttons

    internal static void PaintBitmapButton(
        PaintEventArgs e,
        Image img,
        RectangleF scaledRect)
    {
        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        e.Graphics.DrawImage(img, scaledRect);
    }

    internal static void PaintBitmapButton(
        Button button,
        PaintEventArgs e,
        Image img,
        int x = 0,
        int? y = null)
    {
        y ??= (button.Height - img.Height) / 2;
        e.Graphics.DrawImage(img, x, (int)y);
    }

    internal static void PaintPlusButton(Button button, PaintEventArgs e)
    {
        Rectangle hRect = new((button.ClientRectangle.Width / 2) - 4, button.ClientRectangle.Height / 2, 10, 2);
        Rectangle vRect = new(button.ClientRectangle.Width / 2, (button.ClientRectangle.Height / 2) - 4, 2, 10);
        (_plusRects[0], _plusRects[1]) = (hRect, vRect);
        e.Graphics.FillRectangles(button.Enabled ? BlackForegroundBrush : SystemBrushes.ControlDark, _plusRects);
    }

    internal static void PaintMinusButton(Button button, PaintEventArgs e)
    {
        Rectangle hRect = new((button.ClientRectangle.Width / 2) - 4, button.ClientRectangle.Height / 2, 10, 2);
        e.Graphics.FillRectangle(button.Enabled ? BlackForegroundBrush : SystemBrushes.ControlDark, hRect);
    }

    internal static void PaintXButton(Button button, PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        int minDimension = (Math.Min(button.Width, button.Height) - 1).ClampToZero();
        float leftAndTop = Utils.GetValueFromPercent_Float(30.43f, minDimension);
        float widthAndHeight = minDimension - (leftAndTop * 2);

        float x = leftAndTop, y = leftAndTop;

        if (button.Width > button.Height)
        {
            x = ((float)button.Width / 2) - (widthAndHeight / 2);
        }
        else if (button.Height > button.Width)
        {
            y = ((float)button.Height / 2) - (widthAndHeight / 2);
        }

        FitRectInBounds(e.Graphics, XGPath.GetBounds(), new RectangleF(x, y, widthAndHeight, widthAndHeight));

        e.Graphics.FillPath(button.Enabled ? BlackForegroundBrush : SystemBrushes.ControlDark, XGPath);
    }

    internal static void PaintHamburgerMenuButton24(Button button, PaintEventArgs e)
    {
        e.Graphics.FillRectangles(button.Enabled ? BlackForegroundBrush : SystemBrushes.ControlDark, _hamRects24);
    }

    #endregion

    #region Separators

    // It's ridiculous to instantiate two controls (a ToolStrip and a ToolStripSeparator contained within it)
    // just to draw two one-pixel-wide lines. Especially when there's a ton of them on the UI. For startup
    // perf and lightness of weight, we just draw them ourselves.

    internal static void PaintToolStripSeparators(
        PaintEventArgs e,
        int pixelsFromVerticalEdges,
        ToolStripItem[] items)
    {
        Rectangle sizeBounds = items[0].Bounds;

        int y1 = sizeBounds.Top + pixelsFromVerticalEdges;
        int y2 = sizeBounds.Bottom - pixelsFromVerticalEdges;

        foreach (ToolStripItem item in items)
        {
            if (!item.Visible) continue;
            int l1s = (int)Math.Ceiling((double)item.Margin.Left / 2);
            DrawSeparator(e, Sep1Pen, l1s, y1, y2, item.Bounds.Location.X);
        }
    }

    internal static void PaintControlSeparators(
        PaintEventArgs e,
        int pixelsFromVerticalEdges,
        Control?[] items,
        int topOverride = -1,
        int bottomOverride = -1)
    {
        Rectangle sizeBounds = Rectangle.Empty;
        foreach (Control? item in items)
        {
            if (item != null)
            {
                sizeBounds = item.Bounds;
                break;
            }
        }

        int y1 = topOverride > -1 ? topOverride : sizeBounds.Top + pixelsFromVerticalEdges;
        int y2 = bottomOverride > -1 ? bottomOverride : (sizeBounds.Bottom - pixelsFromVerticalEdges) - 1;

        foreach (Control? item in items)
        {
            if (item is not { Visible: true }) continue;
            int l1s = (int)Math.Ceiling((double)item.Margin.Left / 2);
            DrawSeparator(e, Sep1Pen, l1s, y1, y2, item.Bounds.Location.X);
        }
    }

    internal static void DrawSeparator(
        PaintEventArgs e,
        Pen line1Pen,
        int line1DistanceBackFromLoc,
        int line1Top,
        int line1Bottom,
        int x)
    {
        int sep1x = x - line1DistanceBackFromLoc;
        e.Graphics.DrawLine(line1Pen, sep1x, line1Top, sep1x, line1Bottom);
        if (!Config.DarkMode)
        {
            int sep2x = (x - line1DistanceBackFromLoc) + 1;
            e.Graphics.DrawLine(Sep2Pen, sep2x, line1Top + 1, sep2x, line1Bottom + 1);
        }
    }

    #endregion

    #region Arrows

    // @PERF_TODO(Paint arrows): Maybe we should cache the drawn lines onto bitmaps?
    internal static void PaintArrow7x4(
        Graphics g,
        Direction direction,
        Rectangle area,
        bool? controlEnabled = null,
        Pen? pen = null)
    {
        g.SmoothingMode = SmoothingMode.None;

        int x = area.X + (area.Width / 2);
        int y = area.Y + (area.Height / 2);

        pen ??= controlEnabled == true ? ArrowButtonEnabledPen : SystemPens.ControlDark;

        switch (direction)
        {
            case Direction.Left:
                x -= 2;
                y -= 3;

                // Arrow tip
                g.DrawLine(pen, x, y + 3, x + 1, y + 3);

                g.DrawLine(pen, x + 1, y + 2, x + 1, y + 4);
                g.DrawLine(pen, x + 2, y + 1, x + 2, y + 5);
                g.DrawLine(pen, x + 3, y, x + 3, y + 6);

                break;
            case Direction.Right:
                x -= 1;
                y -= 3;

                g.DrawLine(pen, x, y, x, y + 6);
                g.DrawLine(pen, x + 1, y + 1, x + 1, y + 5);
                g.DrawLine(pen, x + 2, y + 2, x + 2, y + 4);

                // Arrow tip
                g.DrawLine(pen, x + 2, y + 3, x + 3, y + 3);

                break;
            case Direction.Up:
                x -= 3;
                y -= 2;

                // Arrow tip
                g.DrawLine(pen, x + 3, y, x + 3, y + 1);

                g.DrawLine(pen, x + 2, y + 1, x + 4, y + 1);
                g.DrawLine(pen, x + 1, y + 2, x + 5, y + 2);
                g.DrawLine(pen, x, y + 3, x + 6, y + 3);

                break;
            case Direction.Down:
            default:
                x -= 3;
                y -= 1;

                g.DrawLine(pen, x, y, x + 6, y);
                g.DrawLine(pen, x + 1, y + 1, x + 5, y + 1);
                g.DrawLine(pen, x + 2, y + 2, x + 4, y + 2);

                // Arrow tip
                g.DrawLine(pen, x + 3, y + 2, x + 3, y + 3);

                break;
        }
    }

    internal static void PaintArrow9x5(
        Graphics g,
        Direction direction,
        Rectangle area,
        bool? controlEnabled = null,
        Pen? pen = null)
    {
        g.SmoothingMode = SmoothingMode.None;

        int x = area.X + (area.Width / 2);
        int y = area.Y + (area.Height / 2);

        pen ??= controlEnabled == true ? ArrowButtonEnabledPen : SystemPens.ControlDark;

        switch (direction)
        {
            case Direction.Left:
                x -= 2;
                y -= 4;

                // Arrow tip
                g.DrawLine(pen, x, y + 4, x + 1, y + 4);

                g.DrawLine(pen, x + 1, y + 3, x + 1, y + 5);
                g.DrawLine(pen, x + 2, y + 2, x + 2, y + 6);
                g.DrawLine(pen, x + 3, y + 1, x + 3, y + 7);
                g.DrawLine(pen, x + 4, y, x + 4, y + 8);

                break;
            case Direction.Right:
                x -= 2;
                y -= 4;

                g.DrawLine(pen, x, y, x, y + 8);
                g.DrawLine(pen, x + 1, y + 1, x + 1, y + 7);
                g.DrawLine(pen, x + 2, y + 2, x + 2, y + 6);
                g.DrawLine(pen, x + 3, y + 3, x + 3, y + 5);

                // Arrow tip
                g.DrawLine(pen, x + 3, y + 4, x + 4, y + 4);

                break;
            case Direction.Up:
                x -= 4;
                y -= 2;

                // Arrow tip
                g.DrawLine(pen, x + 4, y, x + 4, y + 1);

                g.DrawLine(pen, x + 3, y + 1, x + 5, y + 1);
                g.DrawLine(pen, x + 2, y + 2, x + 6, y + 2);
                g.DrawLine(pen, x + 1, y + 3, x + 7, y + 3);
                g.DrawLine(pen, x, y + 4, x + 8, y + 4);

                break;
            case Direction.Down:
            default:
                x -= 4;
                y -= 2;

                g.DrawLine(pen, x, y, x + 8, y);
                g.DrawLine(pen, x + 1, y + 1, x + 7, y + 1);
                g.DrawLine(pen, x + 2, y + 2, x + 6, y + 2);
                g.DrawLine(pen, x + 3, y + 3, x + 5, y + 3);

                // Arrow tip
                g.DrawLine(pen, x + 4, y + 3, x + 4, y + 4);

                break;
        }
    }

    #endregion

    internal static void DrawHorizDiv(Graphics g, int left, int top, int width)
    {
        const int height = 16;

        int y = top + (height / 2);
        if (Config.DarkMode)
        {
            g.DrawLine(DarkColors.LighterBorderPen, left, y, width, y);
        }
        else
        {
            g.DrawLine(Sep1Pen, left, y, width - 1, y);
            g.DrawLine(Sep2Pen, left + 1, y + 1, width, y + 1);
        }
    }

    #endregion

    #region Release date accuracy testing

#if DateAccTest
    private static Bitmap CreateDateAccuracyImage(DateAccuracy da)
    {
        AssertR(da != DateAccuracy.Null, "da is null");

        Bitmap ret = new(21, 21, PixelFormat.Format32bppPArgb);
        using Graphics g = Graphics.FromImage(ret);

        Brush brush = da switch
        {
            DateAccuracy.Red => Brushes.Red,
            DateAccuracy.Yellow => Brushes.Yellow,
            _ => Brushes.Green,
        };

        g.FillRectangle(brush, 6, 6, 8, 8);
        g.DrawRectangle(Pens.Black, 5, 5, 9, 9);

        return ret;
    }

    private static Bitmap? _dateAccuracyRed;
    public static Bitmap DateAccuracy_Red => _dateAccuracyRed ??= CreateDateAccuracyImage(DateAccuracy.Red);

    private static Bitmap? _dateAccuracyYellow;
    public static Bitmap DateAccuracy_Yellow => _dateAccuracyYellow ??= CreateDateAccuracyImage(DateAccuracy.Yellow);

    private static Bitmap? _dateAccuracyGreen;
    public static Bitmap DateAccuracy_Green => _dateAccuracyGreen ??= CreateDateAccuracyImage(DateAccuracy.Green);
#endif

    #endregion
}
