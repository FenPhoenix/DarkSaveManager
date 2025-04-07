namespace DarkSaveManager.Forms.CustomControls;

internal enum DarkControlState
{
    Normal,
    Hover,
    Pressed,
}

public enum Direction
{
    Left,
    Right,
    Up,
    Down,
}

/// <summary>
/// Set a control's tag to this to tell the darkable control dictionary filler to ignore it.
/// </summary>
internal enum LoadType { Lazy }
