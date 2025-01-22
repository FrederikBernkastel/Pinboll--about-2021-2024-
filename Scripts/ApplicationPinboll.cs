namespace Pinboll;

public interface NGApplication
{
    public RenderConsoleWindow Init();
    public void DispatchEvents(ReadOnlySpan<INPUT_RECORD> array);
    public void Update();
    public void Render(IRender render);
}
public class ApplicationPinboll : NGApplication
{
    private RectangleObject Rectangle { get; set; }
    private TextObject Text { get; set; }
    private TextObject TextPosition { get; set; }
    private RectangleObject Krug { get; set; }
    private RectangleObject GG { get; set; }
    private RectangleObject Anto { get; set; }
    private bool Flag { get; set; }
    private ConsoleWindow Window { get; set; }

    public RenderConsoleWindow Init()
    {
        RenderConsoleWindow window = new(new(40, 20));

        Console.SetWindowSize(window.SizeWindow.X, window.SizeWindow.Y);
        Console.SetBufferSize(window.SizeWindow.X, window.SizeWindow.Y);
        NGConsole.SetResizable(window.Handles.SystemHandle);
        Console.CursorVisible = false;

        NGConsole.GetConsoleMode(window.Handles.StdInput, out var mode);
        mode.OperationAdd(INPUTMODES.MOUSE_INPUT);
        mode.OperationSub(INPUTMODES.QUICK_EDIT_MODE);
        NGConsole.SetConsoleMode(window.Handles.StdInput, mode);

        Rectangle = new();
        Rectangle.Position = new(0, 0);
        Rectangle.Color = CHARCOLOR.BACKGROUND_GREEN;
        Rectangle.Size = new(40, 1);

        string str = "Wellcom to Russia!";
        Text = new();
        Text.Position = new(40 * 0.5f - str.Length * 0.5f, 0);
        Text.Color = CHARCOLOR.FOREGROUND_RED;
        Text.Text = str;

        Krug = new();
        Krug.Position = new(40 * 0.5f, 20 * 0.5f);
        Krug.Color = CHARCOLOR.BACKGROUND_RED;
        Krug.Size = new(2, 1);

        GG = new();
        GG.Position = new(3, 6);
        GG.Size = new(2, 4);
        GG.Color = CHARCOLOR.BACKGROUND_BLUE;

        Anto = new();
        Anto.Position = new(35, 6);
        Anto.Size = new(2, 4);
        Anto.Color = CHARCOLOR.BACKGROUND_BLUE | CHARCOLOR.BACKGROUND_GREEN | CHARCOLOR.BACKGROUND_INTENSITY;

        TextPosition = new();
        TextPosition.Position = new(0, 0);
        TextPosition.Color = CHARCOLOR.FOREGROUND_RED;
        TextPosition.Text = "";

        return window;
    }
    public void DispatchEvents(ReadOnlySpan<INPUT_RECORD> array)
    {
        foreach (var s in array)
        {
            switch (s.EventType)
            {
                case InputEventTypes.MOUSE_EVENT:
                    TextPosition.Text = " - " + s.Event.MouseEvent.dwMousePosition;
                    break;
                case InputEventTypes.KEY_EVENT:
                    if (s.Event.KeyEvent.bKeyDown)
                    {
                        switch (s.Event.KeyEvent.wVirtualKeyCode)
                        {
                            case KEY.Escape:
                                Window.Exit();
                                break;
                            case KEY.E:
                                Flag = true;
                                break;
                        }
                        if (Flag)
                        {
                            switch (s.Event.KeyEvent.wVirtualKeyCode)
                            {
                                case KEY.Up:
                                    GG.Position += new Vector2f(0, -2);
                                    break;
                                case KEY.Down:
                                    GG.Position += new Vector2f(0, 2);
                                    break;
                            }
                            GG.Position = new(GG.Position.X, GG.Position.Y < 1 ? 1 :
                                GG.Position.Y + GG.Size.Y > 20 - 1 ? 20 - GG.Size.Y : GG.Position.Y);
                        }
                    }
                    break;
            }
        }
    }

    private Vector2f Gradus { get; set; } = new Vector2f((float)Math.Cos(60), (float)Math.Sin(60));
    private float Speed { get; set; } = 0.9f;

    public void Update()
    {
        if (Flag)
        {
            Krug.Position += new Vector2f(Gradus.X * Speed, Gradus.Y * Speed);
            if (Krug.Position.Y <= 1 || Krug.Position.Y + Krug.Size.Y >= 20)
            {
                Gradus = new(Gradus.X, -Gradus.Y);
            }
            if (Krug.Position.X <= GG.Position.X + GG.Size.X && Krug.Position.Y > GG.Position.Y && Krug.Position.Y < GG.Position.Y + GG.Size.Y ||
                Krug.Position.X + Krug.Size.X >= Anto.Position.X)
            {
                Gradus = new(-Gradus.X, Gradus.Y);
            }
            if (Gradus.X > 0)
            {
                Anto.Position = new(Anto.Position.X, Krug.Position.Y);
                Anto.Position = new(Anto.Position.X,
                    Anto.Position.Y < 1 ? 1 : Anto.Position.Y + Anto.Size.Y > 20 - 1 ? 20 - Anto.Size.Y : Anto.Position.Y);
            }
            if (Krug.Position.X <= 0 || Krug.Position.X + Krug.Size.X >= 40)
            {
                Flag = false;
            }
        }
    }
    public void Render(IRender render)
    {
        render.Draw(Rectangle);
        render.Draw(Text);
        render.Draw(Krug);
        render.Draw(GG);
        render.Draw(Anto);
        render.Draw(TextPosition);
    }
}