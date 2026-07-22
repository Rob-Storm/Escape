using Game.LevelEditor.Panels;
using Game.LevelEditor.Services;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;

namespace Game.LevelEditor;

/*
 * Todo:
 * 
 * 2D view,
 * Quick room creation,
 * 
 */


public class Editor : World
{
    public string LevelName = "Level";

    public Vector2 PlayerStart = Vector2.Zero;
    public float StartRotation = 0f;

    private EditorContext _context;

    private Viewport _viewport;
    private MenuBar _menuBar;
    private DeveloperConsole _console;
    private AssetBrowser _assetBrowser;
    private PropertyInspector _inspector;
    private MapGrid _mapGrid;
    private LevelSettings _levelSettings;
    private ToolSettings _toolSettings;

    public Editor()
    {
        _camera = new EditorCamera();
        _camera.Transform.Position = new Vector3(SizeX / 2, 1, SizeY / 2);

        _context = new EditorContext(this, (EditorCamera)_camera);

        _viewport = new Viewport(_context);
        _menuBar = new MenuBar(_context);
        _console = new DeveloperConsole(_context);
        _assetBrowser = new AssetBrowser(_context);
        _inspector = new PropertyInspector(_context);
        _mapGrid = new MapGrid(_context);
        _levelSettings = new LevelSettings(_context);
        _toolSettings = new ToolSettings(_context);

        ((EditorCamera)_camera).SetEditor(_viewport);
    }

    ~Editor()
    {
        rlImGui.Shutdown();
    }

    public override void Update()
    {
        //base.Update();

        _camera.Update();


        if (_context.SelectedCell != null && !_viewport.ViewportControlled)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.W))
            {
                _context.SelectedCell.Walls ^= Walls.North;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.S))
            {
                _context.SelectedCell.Walls ^= Walls.South;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.A))
            {
                _context.SelectedCell.Walls ^= Walls.West;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.D))
            {
                _context.SelectedCell.Walls ^= Walls.East;
            }
        }

        if (Raylib.IsKeyPressed(KeyboardKey.F3))
        {
            _debugDrawMode = !_debugDrawMode;
        }

    }

    public override void Render()
    {
        Raylib.BeginTextureMode(_viewport.ViewportRenderTarget);

        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode3D(_camera);

        foreach (Entity entity in EntityList)
        {
            entity.Render(_camera);
        }

        foreach (var cellData in GetCells())
        {
            cellData.cell.Render();

            if(_debugDrawMode)
            {
                cellData.cell.RenderBounds(Color.Blank, Color.SkyBlue);
            }
        }

        if (_context.SelectedCell != null)
        {
            _context.SelectedCell.RenderBounds(Color.Orange, Color.Green);
        }

        DrawWorldGrid();

        Raylib.EndMode3D();

        if (_debugDrawMode)
        {
            Raylib.DrawFPS(0, 0);
        }

        Raylib.EndTextureMode();

    }

    public override void Render2D()
    {
        rlImGui.Begin();

        _menuBar.Draw();

        ImGui.DockSpaceOverViewport();

        _viewport.Draw();

        _console.Draw();

        _assetBrowser.Draw();

        _inspector.Draw();

        _mapGrid.Draw();

        _levelSettings.Draw();

        _toolSettings.Draw();

        if (ImGui.GetIO().KeyCtrl && ImGui.IsKeyPressed(ImGuiKey.S))
        {
            _context.LevelFileService.Save(_context);
        }

        if (ImGui.GetIO().KeyCtrl && ImGui.IsKeyPressed(ImGuiKey.N))
        {
            _context.LevelFileService.NewLevel(_context);
        }

        if (ImGui.GetIO().KeyCtrl && ImGui.IsKeyPressed(ImGuiKey.O))
        {
            _context.LevelFileService.Load(_context);
        }

        if (ImGui.GetIO().KeyCtrl && ImGui.IsKeyPressed(ImGuiKey.R))
        {
            var output = _context.LevelFileService.Save(_context);

            if (output.result.IsOk)
            {
                _context.PlayModeService.RunLevel(output.result.Path);
            }
        }

        rlImGui.End();
    }

    private void DrawWorldGrid()
    {
        float cellSize = 1f;

        Vector3 offset = new Vector3(-0.5f, 0, -0.5f);

        for (int x = 0; x <= SizeX; x++)
        {
            Raylib.DrawLine3D
                (
                    new Vector3(x * cellSize, 0, 0) + offset,
                    new Vector3(x * cellSize, 0, SizeY * cellSize) + offset,
                    Color.Gray
                );
        }

        for (int z = 0; z <= SizeY; z++)
        {
            Raylib.DrawLine3D
                (
                    new Vector3(0, 0, z * cellSize) + offset,
                    new Vector3(SizeX * cellSize, 0, z * cellSize) + offset,
                    Color.Gray
                );
        }
    }

    public Cell CreateDefaultCell(int x, int y)
    {
        return new Cell(x, y)
        {
            NorthWallTexturePath = @"Assets\Textures\Wall.png",
            EastWallTexturePath = @"Assets\Textures\Wall.png",
            SouthWallTexturePath = @"Assets\Textures\Wall.png",
            WestWallTexturePath = @"Assets\Textures\Wall.png",
            FloorTexturePath = @"Assets\Textures\Floor.png",
            CeilingTexturePath = @"Assets\Textures\Ceiling.png"
        };
    }
}

public class EditorContext
{
    public World World { get; }
    public EditorCamera Camera { get; }

    public Cell SelectedCell => World.GetCell(SelectedX, SelectedY);
    public ToolMode ToolMode = ToolMode.Select;
    public PaintWallSettings ToolSettings;

    public int SelectedX;
    public int SelectedY;

    public string LevelName;
    public Vector2 PlayerStart;
    public float StartRotation;

    public string DraggedTexturePath;

    public PlayModeService PlayModeService;
    public AssetService AssetService;
    public LevelFileService LevelFileService;

    public EditorContext(World world, EditorCamera camera)
    {
        World = world;
        Camera = camera;

        LevelName = "Level";
        PlayerStart = Vector2.Zero;
        StartRotation = 0f;

        ToolSettings = new PaintWallSettings();

        PlayModeService = new PlayModeService();
        LevelFileService = new LevelFileService();
        AssetService = new AssetService();
    }
}


public enum ToolMode
{
    Select,
    Draw,
    Room,
    Delete
}