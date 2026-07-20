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
 * Way to change textures on a cell,
 * 
 */

public delegate void ViewportControlChangedSignature(bool newControl);

public class Editor : World
{
    public event ViewportControlChangedSignature ViewportControlChanged;
    public bool ViewportControlled { get; private set; } = false;

    private List<string> _consoleHistory;

    private RenderTexture2D _viewportRenderTarget;
    private Vector2 _viewportSize = new Vector2(960, 540);

    private Vector2 _playerStart = Vector2.Zero;
    private string _levelName = "Level";

    private bool _previousControlState;

    private Cell selectedCell;
    private int selectedIndex;

    public Editor()
    {
        _camera = new EditorCamera();
        _consoleHistory = new List<string>();
        CreateViewportRenderTarget();

        Debug.OnLogCommitted += (message, level, channel) =>
        {
            _consoleHistory.Add(message);
        };

        ((EditorCamera)_camera).SetEditor(this);
    }

    ~Editor()
    {
        rlImGui.Shutdown();
    }

    private void CreateViewportRenderTarget()
    {
        _viewportRenderTarget = Raylib.LoadRenderTexture
            (
                (int)_viewportSize.X,
                (int)_viewportSize.Y
            );
    }

    private void ResizeViewport(Vector2 newSize)
    {
        if(newSize.X <= 0 || newSize.Y <= 0)
        { 
            return;
        }

        _camera.SetAspectRatio(newSize);

        if (newSize != _viewportSize)
        {
            _viewportSize = newSize;

            Raylib.UnloadRenderTexture( _viewportRenderTarget);
            CreateViewportRenderTarget();
        }
    }

    public override void Update()
    {
        base.Update();

        _camera.Update();

        if(selectedCell != null)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Up))
            {
                selectedCell.Position += Directions.Forward;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Down))
            {
                selectedCell.Position += Directions.Backward;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Left))
            {
                selectedCell.Position += Directions.Right;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Right))
            {
                selectedCell.Position += Directions.Left;
            }


            if (Raylib.IsKeyPressed(KeyboardKey.One))
            {
                selectedCell.Walls ^= Walls.North;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Two))
            {
                selectedCell.Walls ^= Walls.East;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Three))
            {
                selectedCell.Walls ^= Walls.South;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.Four))
            {
                selectedCell.Walls ^= Walls.West;
            }
        }
    }

    public override void Render()
    {
        Raylib.BeginTextureMode(_viewportRenderTarget);

        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode3D(_camera);

        foreach (Entity entity in EntityList)
        {
            entity.Render(_camera);
        }

        foreach (Cell cell in CellList)
        {
            cell.Render();
        }

        Raylib.DrawGrid(10, 1);

        Raylib.EndMode3D();

        Raylib.EndTextureMode();

    }

    public override void Render2D()
    {
        rlImGui.Begin();

        DrawMenuBar();

        ImGui.DockSpaceOverViewport();

        DrawLevelViewport();

        DrawConsole();

        DrawAssets();

        DrawProperties();

        DrawMapGrid();

        DrawLevelSettings();

        ImGui.End();

        rlImGui.End();
    }

    private void DrawMenuBar()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New")) { NewLevel(); }
                if (ImGui.MenuItem("Save")) { SaveLevel(); }
                if (ImGui.MenuItem("Load")) { LoadEditorLevel(); }
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Add Cell")) { CellList.Add(new Cell(walls, floor, ceiling)); }
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }

    private void DrawLevelViewport()
    {
        ImGui.Begin("Viewport");

        bool viewportHovered = ImGui.IsItemHovered();

        Vector2 size = ImGui.GetContentRegionAvail();
        ResizeViewport(size);

        ViewportControlled = Raylib.IsMouseButtonDown(MouseButton.Right) && ImGui.IsWindowHovered();

        if(ViewportControlled != _previousControlState)
        {
            ViewportControlChanged?.Invoke(ViewportControlled);
        }

        if (ViewportControlled)
        {
            Vector2 viewportCenter = size;

            viewportCenter.X *= 0.5f;
            viewportCenter.Y *= 0.5f;

            Raylib.SetMousePosition((int)viewportCenter.X, (int)viewportCenter.Y);
        }

        rlImGui.ImageRenderTexture(_viewportRenderTarget);

        ImGui.End();

        _previousControlState = ViewportControlled;
    }

    private void DrawMapGrid()
    {
        ImGui.Begin("Map");

        if(ImGui.BeginListBox("Cells"))
        {
            for (int i = 0; i < CellList.Count; i++)
            {
                bool isSelected = selectedIndex == i;

                if (ImGui.Selectable($"Cell {i}##cell_{i}", isSelected))
                {
                    selectedIndex = i;
                }

                if(isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                    selectedCell = CellList[i];
                }
            }

            ImGui.EndListBox();
        }

        ImGui.End();
    }

    private void DrawProperties()
    {
        ImGui.Begin("Properties");

        if(selectedCell != null)
        {
            Vector3 position = selectedCell.Position;
            if(ImGui.InputFloat3("Position", ref position))
            {
                selectedCell.Position = new Vector3(position.X, 0, position.Z);
            }

            uint flags = (uint)selectedCell.Walls;

            ImGui.CheckboxFlags("North", ref flags, (uint)Walls.North);
            ImGui.CheckboxFlags("East", ref flags, (uint)Walls.East);
            ImGui.CheckboxFlags("South", ref flags, (uint)Walls.South);
            ImGui.CheckboxFlags("West", ref flags, (uint)Walls.West);

            selectedCell.Walls = (Walls)flags;
        }

        ImGui.End();
    }

    private void DrawConsole()
    {
        ImGui.Begin("Developer Console");

        ImGui.BeginChild("Scroll", new Vector2(0, -25), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar);

        foreach(string message in _consoleHistory)
        {
            ImGui.Text(message);
        }

        ImGui.EndChild();

        string test = string.Empty;

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        ImGui.InputTextWithHint("##ConsoleInput", "Enter Command", ref test, 256,
            ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.EscapeClearsAll);
    }

    private void DrawLevelSettings()
    {
        ImGui.Begin("Level Settings");

        ImGui.InputText("Level Name", ref _levelName, 16);
        ImGui.InputFloat2("Player Start", ref _playerStart);

        ImGui.End();
    }

    private void DrawAssets()
    {
        ImGui.Begin("Assets");


        ImGui.End();
    }

    private void NewLevel()
    {
        EntityList.Clear();
        CellList.Clear();
        _levelName = "New Level";
        _playerStart = Vector2.Zero;

        Debug.Log("New level");
    }

    private void SaveLevel()
    {
        Level level = Level.FromWorld(this);
        level.PlayerStart = _playerStart;

        Level.SaveToFile(level, _levelName);

        Debug.Log("Saving level");
    }

    private void LoadEditorLevel()
    {
        Debug.Log("Load level");
        Level editorLevel = Level.LoadFromFile(@"C:\Users\The1Wolfcast\source\Games\Escape\Game\Assets\Maps\EditorTest.hdl");

        EntityList = editorLevel.EntityList;
        CellList = editorLevel.CellList;

        _playerStart = editorLevel.PlayerStart;
        _levelName = "EditorTest";
    }

}
