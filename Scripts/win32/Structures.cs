namespace _Win32;

[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
public struct KEY_EVENT_RECORD
{
    [FieldOffset(0), MarshalAs(UnmanagedType.Bool)]
    public bool bKeyDown;
    [FieldOffset(4), MarshalAs(UnmanagedType.U2)]
    public ushort wRepeatCount;
    [FieldOffset(6), MarshalAs(UnmanagedType.U2)]
    public KEY wVirtualKeyCode;
    [FieldOffset(8), MarshalAs(UnmanagedType.U2)]
    public ushort wVirtualScanCode;
    [FieldOffset(10)]
    public char UnicodeChar;
    [FieldOffset(12), MarshalAs(UnmanagedType.U4)]
    public ControlKeyState dwControlKeyState;
}
[StructLayout(LayoutKind.Explicit)]
public struct INPUT_RECORD_UNION
{
    [FieldOffset(0)]
    public KEY_EVENT_RECORD KeyEvent;
    [FieldOffset(0)]
    public MOUSE_EVENT_RECORD MouseEvent;
};
[StructLayout(LayoutKind.Sequential)]
public struct MOUSE_EVENT_RECORD
{
    public Vector2<short> dwMousePosition;
    public MouseButtonState dwButtonState;
    public ControlKeyState dwControlKeyState;
    public MouseEventFlags dwEventFlags;
}
[StructLayout(LayoutKind.Sequential)]
public struct INPUT_RECORD
{
    public InputEventTypes EventType;
    public INPUT_RECORD_UNION Event;
};
public struct ModeEdit
{
    public uint UINT;
    public void OperationSub(INPUTMODES input) => UINT &= ~(uint)input;
    public void OperationAdd(INPUTMODES input) => UINT |= (uint)input;
}
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct FONT_INFO
{
    public uint cbSize;
    public uint nFont;
    public Vector2<short> dwFontSize;
    public int FontFamily;
    public int FontWeight;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string FaceName;
}
[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
public struct CHAR_INFO
{
    [FieldOffset(0)]
    public char UnicodeChar;
    [FieldOffset(0)]
    public char AsciiChar;
    [FieldOffset(2)]
    public ushort Attributes;
}
public struct Vector2f
{
    public float X, Y;
    public Vector2f(float x, float y)
    {
        X = x; Y = y;
    }
    public static Vector2f operator +(Vector2f vector1, Vector2f vector2) => new(vector1.X + vector2.X, vector1.Y + vector2.Y);
    public static Vector2f operator -(Vector2f vector1, Vector2f vector2) => new(vector1.X - vector2.X, vector1.Y - vector2.Y);
}
public record struct Vector2<T>(T X, T Y) where T : struct;
public record struct Vector3<T>(T X, T Y, T Z) where T : struct;
public record struct Vector4<T>(T X, T Y, T Z, T W) where T : struct;