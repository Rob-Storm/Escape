using Game.LevelEditor.Panels;

using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;

namespace Game.LevelEditor;

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
    private ToolSettings _toolSettings;
    private EntityHeirarchy _entityHeirarchy;


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
        _toolSettings = new ToolSettings(_context);
        _entityHeirarchy = new EntityHeirarchy(_context);

        ((EditorCamera)_camera).SetEditor(_viewport);

        _context.SelectedObject = null;
    }

    ~Editor()
    {
        // May remove if I decide to use imgui for the final game ui
        rlImGui.Shutdown();
    }

    public override void Update()
    {
        foreach (Entity entity in EntityList)
        {
            entity.Update();
        }

        _camera.Update();

        _viewport.Update();

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
            if (_debugDrawMode && entity.Collider != null)
            {
                Raylib.DrawBoundingBox(entity.Collider.BoundingBox, entity.Collider.Color);
            }

            entity.Render(_camera);
        }

        foreach (var cellData in GetCells())
        {
            cellData.cell.Render();

            if (_debugDrawMode)
            {
                cellData.cell.RenderBounds(Color.Blank, Color.SkyBlue);
            }
        }

        if (_context.SelectedCell != null)
        {
            _context.SelectedCell.RenderBounds(Color.Orange, Color.Green);
        }

        if (_context.SelectedObject != null && _context.SelectedObject is Entity selectedEntity)
        {
            selectedEntity.DebugRender(_camera);
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

        _toolSettings.Draw();

        _entityHeirarchy.Draw();

        if (ImGui.GetIO().KeyCtrl && ImGui.IsKeyPressed(ImGuiKey.S))
        {
            _context.LevelFileService.Save(_context);
        }

        if (ImGui.GetIO().KeyCtrl && ImGui.IsKeyPressed(ImGuiKey.N))
        {

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
                // Todo: make this await/async
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


public enum ToolMode
{
    Select,
    Entity,
    Draw,
    Room,
    Delete
}