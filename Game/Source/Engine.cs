using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;

namespace Game;

public class Engine
{
    public bool ShowFPS = false;

    private int _screenWidth = 960;
    private int _screenHeight = 540;

    private World _world;

    public void Init(bool isEditorMode = false)
    {
        Raylib.SetTraceLogLevel(TraceLogLevel.None);

        Raylib.InitWindow(_screenWidth, _screenHeight, "Escape");
        Raylib.InitAudioDevice();

        if (isEditorMode)
        {
            rlImGui.Setup(true);

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            _world = new LevelEditor.Editor();

            Raylib.ToggleFullscreen();
        }
        else
        {
            _world = new World();

            _world.LoadLevel(Level.LoadFromFile(@"C:\Users\The1Wolfcast\source\Games\Escape\Game\Assets\Maps\EditorTest.hdl"));
        }

        while (!Raylib.WindowShouldClose())
        {
            Update();
            Render();
        }

        Shutdown();
    }

    private void Update()
    {
        Time.Update();

        _world.Update();

        if(Raylib.IsKeyReleased(KeyboardKey.F5))
        {
            ShowFPS = !ShowFPS;
        }

        if (Raylib.IsKeyReleased(KeyboardKey.F11))
        {
            Raylib.ToggleFullscreen();
        }
    }

    private void Render()
    {
        Raylib.BeginDrawing();

        _world.Render();

        _world.Render2D();

        if (ShowFPS)
        {
            Raylib.DrawFPS(0, 0);
        }

        Raylib.EndDrawing();
    }

    private void Shutdown()
    {
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }
}
