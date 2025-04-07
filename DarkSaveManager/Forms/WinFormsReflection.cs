namespace DarkSaveManager.Forms;

// @NET5(WinFormsReflection): Make sure these still work with whatever .NET version we're currently using
// Tested working for .NET 9
internal static class WinFormsReflection
{
    internal const string ToolTipNativeWindow_ToolTipFieldName =
        "_toolTip";

    internal const string Form_RestoredWindowBounds =
        "_restoredWindowBounds";

    internal const string Form_RestoredWindowBoundsSpecified =
        "_restoredWindowBoundsSpecified";
}
