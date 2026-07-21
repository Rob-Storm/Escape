using Game.LevelEditor.Panels;
using ImGuiNET;
using NativeFileDialogSharp;
using Raylib_cs;
using rlImGui_cs;
using System.Diagnostics;
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
    public Vector2 PlayerStart = Vector2.Zero;
    public string LevelName = "Level";


    private Cell _selectedCell;
    public Cell SelectedCell
    {
        get => SelectedX >= 0 && SelectedY >= 0 ? GetCell(SelectedX, SelectedY): null;
    }

    public int SelectedX, SelectedY;

    public string DraggedTexturePath;

    private string _defaultPath = Paths.MapsFolder;

    private string _pendingPopup = null;

    private Viewport _viewport;
    private MenuBar _menuBar;
    private DeveloperConsole _console;
    private AssetBrowser _assetBrowser;
    private PropertyInspector _inspector;
    private MapGrid _mapGrid;

    public Editor()
    {
        _camera = new EditorCamera();
        _camera.Transform.Position = new Vector3(WORLD_WIDTH / 2, 1, WORLD_HEIGHT / 2);

        _viewport = new Viewport(this, (EditorCamera)_camera);
        _menuBar = new MenuBar(this);
        _console = new DeveloperConsole(this);
        _assetBrowser = new AssetBrowser(this);
        _inspector = new PropertyInspector(this);
        _mapGrid = new MapGrid(this);

        ((EditorCamera)_camera).SetEditor(_viewport);
    }

    ~Editor()
    {
        rlImGui.Shutdown();
    }

    public override void Update()
    {
        base.Update();

        _camera.Update();

        /*
        if (selectedCell != null && !ViewportControlled)
        {
            if (Raylib.IsKeyPressed(KeyboardKey.W))
            {
                selectedCell.Walls ^= Walls.North;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.S))
            {
                selectedCell.Walls ^= Walls.South;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.A))
            {
                selectedCell.Walls ^= Walls.West;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.D))
            {
                selectedCell.Walls ^= Walls.East;
            }
        }
        */
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

        foreach(var cellData in GetCells())
        {
            cellData.cell.Render();
        }

        if(SelectedCell != null)
        {
            SelectedCell.RenderBounds(Color.Orange, Color.Green);
        }        

        DrawWorldGrid();

        Raylib.EndMode3D();

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


        DrawLevelSettings();

        if (ImGui.GetIO().KeyCtrl && ImGui.IsKeyPressed(ImGuiKey.S))
        {
            SaveLevel();
        }

        if (ImGui.GetIO().KeyCtrl && ImGui.IsKeyPressed(ImGuiKey.N))
        {
            NewLevel();
        }

        if (ImGui.GetIO().KeyCtrl && ImGui.IsKeyPressed(ImGuiKey.O))
        {
            LoadEditorLevel();
        }

        if (ImGui.GetIO().KeyCtrl && ImGui.IsKeyPressed(ImGuiKey.R))
        {
            RunLevel(SaveLevel().path);
        }

        rlImGui.End();
    }


    private void DrawWorldGrid()
    {
        float cellSize = 1f;

        Vector3 offset = new Vector3(-0.5f, 0, -0.5f);

        for (int x = 0; x <= WORLD_WIDTH; x++)
        {
            Raylib.DrawLine3D
                (
                    new Vector3(x * cellSize, 0, 0) + offset, 
                    new Vector3(x * cellSize, 0, WORLD_HEIGHT * cellSize) + offset, 
                    Color.Gray
                );
        }

        for (int z = 0; z <= WORLD_HEIGHT; z++)
        {
            Raylib.DrawLine3D
                (
                    new Vector3(0, 0, z * cellSize) + offset, 
                    new Vector3(WORLD_WIDTH * cellSize, 0, z * cellSize) + offset, 
                    Color.Gray
                );
        }
    }

    private void DrawLevelSettings()
    {
        ImGui.Begin("Level Settings");

        ImGui.InputText("Level Name", ref LevelName, 16);
        ImGui.InputFloat2("Player Start", ref PlayerStart);

        ImGui.End();
    }

    public void NewLevel()
    {
        EntityList.Clear();
        Cells = new Cell[WORLD_WIDTH, WORLD_HEIGHT];
        LevelName = "New Level";
        PlayerStart = Vector2.Zero;

        Debug.Log("New level");
    }

    public (DialogResult result, string path) SaveLevel()
    {
        var result = Dialog.FileSave("hdl", Paths.MapsFolder);

        string path = null;

        if(result.IsOk)
        {
            Level level = Level.FromWorld(this);
            level.PlayerStart = PlayerStart;

            path = Level.SaveToFile(level, result.Path);
        }

        return (result, path);
    }

    public void LoadEditorLevel()
    {
        var result = Dialog.FileOpen("hdl", Paths.MapsFolder);

        if(result.IsOk)
        {
            Debug.Log("Load level");
            Level editorLevel = Level.LoadFromFile(result.Path);

            EntityList = editorLevel.EntityList;
            Cells = editorLevel.Cells;

            PlayerStart = editorLevel.PlayerStart;
            LevelName = Path.GetFileNameWithoutExtension(result.Path);
        }
    }

    public async Task RunLevel(string path)
    {
        Debug.Log("Starting play session");

        ProcessStartInfo info = new ProcessStartInfo(Paths.ApplicationExecutable, $"--level \"{path}\"");

        info.RedirectStandardOutput = true;
        info.UseShellExecute = false;
        info.CreateNoWindow = false;

        using(Process process = new Process())
        {
            process.StartInfo = info;
            process.Start();

            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            string output = await outputTask;
            string error = await errorTask;

            if(!string.IsNullOrEmpty(output))
            {
                Debug.Log(output);
            }

            if (!string.IsNullOrEmpty(error))
            {
                Debug.Log(error, LogLevel.Error);
            }

            Debug.Log("End play session");
            Debug.Log($"Exit code: {process.ExitCode}");
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
