using System.ComponentModel;
using JetBrains.Annotations;

namespace DarkSaveManager.Forms.CustomControls;

public sealed class DarkFlowLayoutPanel : FlowLayoutPanel, IDarkable
{
    [PublicAPI]
    public Color DrawnBackColor = SystemColors.Control;

    [PublicAPI]
    public Color DarkModeDrawnBackColor = DarkColors.Fen_ControlBackground;

    [PublicAPI]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool DarkModeEnabled { get; set; }

    protected override void OnPaint(PaintEventArgs e)
    {
        SolidBrush brush = DarkColors.GetCachedSolidBrush(DarkModeEnabled ? DarkModeDrawnBackColor : DrawnBackColor);
        e.Graphics.FillRectangle(brush, ClientRectangle);
    }
}
