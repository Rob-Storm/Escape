using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;

namespace Game;

public class Engine
{
    public bool ShowFPS = false;

    private int _screenWidth = 1280;
    private int _screenHeight = 720;

    private World _world;

    public static bool IsEditor { get; private set; } = false;

    public void Init(bool isEditorMode = false, string? levelPath = null)
    {
        Raylib.SetTraceLogLevel(TraceLogLevel.None);

        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);

        Raylib.InitWindow(_screenWidth, _screenHeight, isEditorMode ? "Editor" : "Escape");
        Raylib.InitAudioDevice();

        if (isEditorMode)
        {
            IsEditor = true;

            Raylib.MaximizeWindow();

            AssetManager.ScanRegistries();

            rlImGui.Setup(true);

            ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            _world = new LevelEditor.Editor();

        }
        else
        {
            _world = new World();

            _world.LoadLevel(Level.LoadFromFile(Path.Combine(Paths.MapsFolder, "Man.hdl")));
        }

        if (!string.IsNullOrEmpty(levelPath))
        {
            _world.LoadLevel(Level.LoadFromFile(levelPath));
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
