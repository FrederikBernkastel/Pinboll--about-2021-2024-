namespace _Win32;

public class NGConsole
{
    public readonly record struct StdHandle(
        IntPtr SystemHandle,
        IntPtr StdInput,
        IntPtr StdOutput,
        IntPtr StdError);
    public static StdHandle GetHandles => new StdHandle(
            GetConsoleWindow(),
            GetStdHandle(-10),
            GetStdHandle(-11),
            GetStdHandle(-12));


    [DllImport("user32.dll")]
    private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
    [DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
    public static void SetResizable(IntPtr SystemHandle)
    {
        const int MF_BYCOMMAND = 0x00000000;
        const int SC_MINIMIZE = 0xF020;
        const int SC_MAXIMIZE = 0xF030;
        const int SC_SIZE = 0xF000;
        
        IntPtr sysMenu = GetSystemMenu(SystemHandle, false);

        if (SystemHandle != IntPtr.Zero)
        {
            DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
            DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
            DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
        }
    }
    
    [DllImport("kernel32.dll")]
    public static extern bool GetNumberOfConsoleInputEvents(
        IntPtr hConsoleInput,
        out uint lpcNumberOfEvents);
    [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW")]
    public static extern bool ReadConsoleInput(
        IntPtr hConsoleInput,
        [Out] INPUT_RECORD[] lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsRead);
    [DllImport("kernel32.dll")]
    public static extern bool GetConsoleMode(
        IntPtr hConsoleOutputOrInput,
        out ModeEdit lpMode);
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetConsoleWindow();
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern bool GetCurrentConsoleFontEx(
        IntPtr ConsoleOutput,
        bool MaximumWindow,
        ref FONT_INFO ConsoleCurrentFont);
    [DllImport("kernel32.dll")]
    public static extern IntPtr GetStdHandle(
        int nStdHandle);
    [DllImport("kernel32.dll")]
    public static extern bool ScrollConsoleScreenBuffer(
        IntPtr hConsoleOutput,
        ref Vector4<short> lpScrollRectangle,
        ref Vector4<short> lpClipRectangle,
        Vector2<short> dwDestinationOrigin,
        ref CHAR_INFO lpFill);
    [DllImport("kernel32.dll")]
    public static extern bool SetConsoleMode(
        IntPtr hConsoleOutputOrInput,
        ModeEdit dwMode);
    [DllImport("kernel32.dll")]
    public static extern bool SetConsoleScreenBufferSize(
        IntPtr hConsoleOutput,
        Vector2<short> dwSize);
    [DllImport("kernel32.dll")]
    public static extern bool SetConsoleWindowInfo(
        IntPtr hConsoleOutput,
        bool bAbsolute,
        ref Vector4<short> lpConsoleWindow);
    [DllImport("kernel32.dll")]
    public static extern bool SetCurrentConsoleFontEx(
        IntPtr ConsoleOutput,
        bool MaximumWindow,
        ref FONT_INFO ConsoleCurrentFontEx);
    [DllImport("kernel32.dll")]
    public static extern bool WriteConsoleOutput(
        IntPtr hConsoleOutput,
        CHAR_INFO[] lpBuffer,
        Vector2<short> dwBufferSize,
        Vector2<short> dwBufferCoord,
        ref Vector4<short> lpWriteRegion);
}