using Raylib_cs;

namespace Game;

public class Engine
{
    public bool ShowFPS = false;

    private int _screenWidth = 960;
    private int _screenHeight = 540;

    private World _world;
    private Camera _camera;
    private Player _player;


    public void Init(bool isEditorMode = false)
    {
        Raylib.InitWindow(_screenWidth, _screenHeight, "Escape");
        Raylib.InitAudioDevice();

        if (isEditorMode)
        {
            DebugCamera debugCamera = new DebugCamera();
            _camera = debugCamera;

            _world = new LevelEditor.Editor(debugCamera);
        }
        else
        {
            _player = new Player();
            _camera = _player.Camera;

            _world = new World(_camera);

            _world.LoadLevel(Level.LoadFromFile(@"C:\Users\The1Wolfcast\source\Games\Escape\Game\Assets\Maps\Test.level"));

            _world.EntityList.Add(_player);

            _player.World = _world;
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
    }

    private void Render()
    {
        Raylib.BeginDrawing();

        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode3D(_camera);

        _world.Render();

        Raylib.EndMode3D();

        if(ShowFPS)
        {
            Raylib.DrawFPS(0, 0);
        }

        _world.Render2D();

        Raylib.EndDrawing();
    }

    private void Shutdown()
    {
        Raylib.CloseAudioDevice();
        Raylib.CloseWindow();
    }
}
