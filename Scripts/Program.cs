new EngineConsole(new ApplicationPinboll()).Run();

public interface IRender
{
    public Vector2<ushort> SizeFramebuffer {get;}
    public void Clear();
    public void Clear(CHARCOLOR color);
    public void Draw(RectangleObject rectangle);
    public void Draw(TextObject text);
    public void Display();
}
public class ConsoleWindow
{
    public NGConsole.StdHandle Handles { get; }
    public Vector2<ushort> SizeWindow { get; }
    public bool IsOpen { get; private set; } = true;
    private INPUT_RECORD[] ArrayInput { get; }
    
    public ConsoleWindow(Vector2<ushort> size_window)
    {
        SizeWindow = size_window;
        Handles = NGConsole.GetHandles;
        ArrayInput = new INPUT_RECORD[128];
    }

    public ReadOnlySpan<INPUT_RECORD> DispatchEvents()
    {
        for (int i = 0; i < ArrayInput.Length; i++)
            ArrayInput[i] = default;
        NGConsole.GetNumberOfConsoleInputEvents(Handles.StdInput, out var number);
        if (number > 0)
            NGConsole.ReadConsoleInput(Handles.StdInput, ArrayInput, 128, out var _);
        return new ReadOnlySpan<INPUT_RECORD>(ArrayInput, 0, (int)number);
    }
    public void Exit() => IsOpen = false;
}
public class RenderConsoleWindow : ConsoleWindow, IRender
{
    public Vector2<ushort> SizeFramebuffer {get; private set;}
    private CHAR_INFO[] Texture {get;}

    public RenderConsoleWindow(Vector2<ushort> size_window) : base(size_window)
    {
        SizeFramebuffer = size_window;
        Texture = new CHAR_INFO[size_window.X * size_window.Y];
    }

    public void Clear() => Clear(0);
    public void Clear(CHARCOLOR color)
    {
        for (int i = 0; i < Texture.Length; i++)
            Texture[i] = new CHAR_INFO {
                UnicodeChar = ' ', 
                Attributes = (ushort)color};
    }
    public void Draw(RectangleObject rectangle)
    {
        if (!(rectangle.Position.X >= 0 && rectangle.Position.X < SizeFramebuffer.X && rectangle.Position.Y >= 0 && rectangle.Position.Y < SizeFramebuffer.Y) &&
            !(rectangle.Position.X + rectangle.Size.X > 0 && rectangle.Position.X + rectangle.Size.X <= SizeFramebuffer.X && rectangle.Position.Y + rectangle.Size.Y > 0 &&
            rectangle.Position.Y + rectangle.Size.Y <= SizeFramebuffer.Y))
            return;
        for (int i = (int)rectangle.Position.Y; i < (int)rectangle.Position.Y + rectangle.Size.Y; i++)
        {
            for (int j = (int)rectangle.Position.X; j < (int)rectangle.Position.X + rectangle.Size.X; j++)
            {
                int temp = (i < 0 ? 0 : i > SizeFramebuffer.Y - 1 ? SizeFramebuffer.Y - 1 : i) *
                SizeFramebuffer.X + (j < 0 ? 0 : j > SizeFramebuffer.X - 1 ? SizeFramebuffer.X - 1 : j);
                Texture[temp].Attributes |= (ushort)rectangle.Color;
            }
        }
    }
    public void Draw(TextObject text)
    {
        if ((text.Position.Y < 0 || text.Position.Y > SizeFramebuffer.Y - 1) || (text.Position.X < 0 && text.Position.X + text.Text.Length <= 0) ||
            (text.Position.X > SizeFramebuffer.X - 1 && text.Position.X + text.Text.Length > SizeFramebuffer.X - 1))
            return;
        for (int i = (int)text.Position.X; i < text.Position.X + text.Text.Length; i++)
        {
            int temp = (int)text.Position.Y * SizeFramebuffer.X + (i < 0 ? 0 : i > SizeFramebuffer.X - 1 ? SizeFramebuffer.X - 1 : i);
            Texture[temp].Attributes |= (ushort)text.Color;
            Texture[temp].UnicodeChar = text.Text[(i < 0 ? 0 : i > SizeFramebuffer.X - 1 ? SizeFramebuffer.X - 1 : i) - (int)text.Position.X];
        }
    }
    public void Display()
    {
        var rct = new Vector4<short>(0, 0, (short)SizeFramebuffer.X, (short)SizeFramebuffer.Y);
        NGConsole.WriteConsoleOutput(
            Handles.StdOutput, Texture, new Vector2<short>((short)SizeFramebuffer.X, (short)SizeFramebuffer.Y), 
            new Vector2<short>(), ref rct);
    }
}
public class EngineConsole
{
    private RenderConsoleWindow Window { get; set; }
    private NGApplication Application { get; }

    public EngineConsole(NGApplication application)
    {
        Application = application;
        Window = Application.Init();
    }
    
    public void Run()
    {
        const int TICKS_PER_SECOND = 30;
        const int SKIP_TICKS = 1000 / TICKS_PER_SECOND;
        const int MAX_FRAMESKIP = 5;

        long next_game_tick = Environment.TickCount64;
        int loops;
        float interpolation;

        Console.Clear();

        while (Window.IsOpen)
        {
            Console.SetCursorPosition(0, 0);
            loops = 0;
            while (Environment.TickCount64 > next_game_tick && loops < MAX_FRAMESKIP)
            {
                Application.DispatchEvents(Window.DispatchEvents());
                Application.Update();

                next_game_tick += SKIP_TICKS;
                loops++;
            }

            interpolation = (float)(Environment.TickCount64 + SKIP_TICKS - next_game_tick) / SKIP_TICKS;

            Window.Clear();
            Application.Render(Window);
            Window.Display();
        }
        Console.Clear();
    }
}
public interface IDrawObject
{
    public void Draw(IRender render);
}
public class RectangleObject : IDrawObject
{
    public Vector2f Position {get; set;}
    public CHARCOLOR Color {get; set;}
    public Vector2<float> Size {get; set;}
    public void Draw(IRender render) => render.Draw(this);
}
public class TextObject : IDrawObject
{
    public Vector2f Position {get; set;}
    public CHARCOLOR Color {get; set;}
    public string Text {get; set;} = "";
    public void Draw(IRender render) => render.Draw(this);
}