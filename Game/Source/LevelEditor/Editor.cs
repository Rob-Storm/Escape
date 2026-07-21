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

    private List<string> _consoleHistory;


    private Vector2 _playerStart = Vector2.Zero;
    private string _levelName = "Level";


    private Cell _selectedCell;

    private Cell selectedCell
    {
        get => selectedX >= 0 && selectedY >= 0 ? GetCell(selectedX, selectedY): null;
    }

    private int selectedX, selectedY;

    private string _draggedTexturePath;

    private string _defaultPath = Paths.MapsFolder;

    private string _pendingPopup = null;

    private Viewport _viewport;

    public Editor()
    {
        _camera = new EditorCamera();
        _camera.Transform.Position = new Vector3(WORLD_WIDTH / 2, 1, WORLD_HEIGHT / 2);
        _consoleHistory = new List<string>();

        _viewport = new Viewport(this, (EditorCamera)_camera);

        Debug.OnLogCommitted += (message, level, channel) =>
        {
            _consoleHistory.Add(message);
        };

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

        if(selectedCell != null)
        {
            selectedCell.RenderBounds(Color.Orange, Color.Green);
        }        

        DrawWorldGrid();

        Raylib.EndMode3D();

        Raylib.EndTextureMode();

    }

    public override void Render2D()
    {
        rlImGui.Begin();

        DrawMenuBar();

        ImGui.DockSpaceOverViewport();

        _viewport.Draw();

        DrawConsole();

        DrawAssets();

        DrawProperties();

        DrawMapGrid();

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

    private void DrawMenuBar()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New", "Ctrl+N")) { NewLevel(); }
                if (ImGui.MenuItem("Save", "Ctrl+S")) { SaveLevel(); }
                if (ImGui.MenuItem("Open", "Ctrl+O")) { LoadEditorLevel(); }
                ImGui.EndMenu();
            }

            if (ImGui.MenuItem("Run"))
            {
                var output = SaveLevel();

                if (output.result.IsOk)
                {
                    RunLevel(output.path);
                }
            }

            ImGui.EndMainMenuBar();
        }
    }

    private void DrawMapGrid()
    {
        ImGui.Begin("Map");

        var drawList = ImGui.GetWindowDrawList();

        Vector2 origin = ImGui.GetCursorScreenPos();

        float cellSize = 32f;

        uint gridColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 1.0f));

        uint wallColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 0f, 0f, 1.0f));

        uint filledColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.75f, 0.75f, 0.75f, 1f));

        uint selectedColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 0.5f, 0f, 0.75f));

        for (int y = 0; y < WORLD_HEIGHT; y++)
        {
            for (int x = 0; x < WORLD_WIDTH; x++)
            {
                Cell cell = GetCell(x, y);

                Vector2 cellMin = origin + new Vector2(x * cellSize, y * cellSize);

                Vector2 cellMax = cellMin + new Vector2(cellSize, cellSize);

                ImGui.SetCursorScreenPos(cellMin);

                ImGui.InvisibleButton($"Cell##{x}_{y}", new Vector2(cellSize, cellSize));

                bool hovered = ImGui.IsItemHovered();

                if (cell != null)
                {
                    drawList.AddRectFilled(cellMin, cellMax, filledColor);
                }

                if (selectedX == x && selectedY == y)
                {
                    drawList.AddRectFilled(cellMin, cellMax, selectedColor);
                }

                drawList.AddRect(cellMin, cellMax, gridColor);

                if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    selectedX = x;
                    selectedY = y;
                }

                if (ImGui.BeginPopupContextItem($"CellOptions##{x}_{y}"))
                {
                    if (cell == null)
                    {
                        if (ImGui.MenuItem("Add Cell"))
                        {
                            SetCell(x, y, CreateDefaultCell(x,y));
                        }
                    }
                    else
                    {
                        if (ImGui.MenuItem("Remove Cell"))
                        {
                            SetCell(x, y, null);

                            if (selectedX == x && selectedY == y)
                            {
                                selectedX = -1;
                                selectedY = -1;
                            }
                        }

                        if (ImGui.MenuItem("Set Player Start"))
                        {
                            _playerStart = new Vector2(x, y);
                        }
                    }

                    ImGui.EndPopup();
                }

            }
        }

        for (int y = 0; y < WORLD_HEIGHT; y++)
        {
            for (int x = 0; x < WORLD_WIDTH; x++)
            {
                Cell cell = GetCell(x, y);

                Vector2 cellMin = origin + new Vector2(x * cellSize, y * cellSize);


                if (cell != null)
                {
                    if (cell.Walls.HasFlag(Walls.North))
                    {
                        DrawWallLine(Walls.North, drawList, cellMin, cellSize, wallColor);
                    }
                    if (cell.Walls.HasFlag(Walls.East))
                    {
                        DrawWallLine(Walls.East, drawList, cellMin, cellSize, wallColor);
                    }
                    if (cell.Walls.HasFlag(Walls.South))
                    {
                        DrawWallLine(Walls.South, drawList, cellMin, cellSize, wallColor);
                    }
                    if (cell.Walls.HasFlag(Walls.West))
                    {
                        DrawWallLine(Walls.West, drawList, cellMin, cellSize, wallColor);
                    }
                }
            }
        }

        ImGui.End();
    }

    private void DrawWallLine(Walls wall, ImDrawListPtr drawList, Vector2 cellMin, float cellSize, uint color)
    {
        Vector2 start = cellMin;
        Vector2 end = cellMin;

        switch (wall)
        {
            case Walls.North:
                start += new Vector2(cellSize, 0);
                end += new Vector2(0, 0);
                break;
            case Walls.East:
                start += new Vector2(cellSize, cellSize);
                end += new Vector2(cellSize, 0);
                break;
            case Walls.South:
                start += new Vector2(0, cellSize);
                end += new Vector2(cellSize, cellSize);
                break;
            case Walls.West:
                start += new Vector2(0, 0);
                end += new Vector2(0, cellSize);
                break;
        }

        drawList.AddLine(start, end, color, 1.5f);
    }

    private void DrawProperties()
    {
        ImGui.Begin("Properties");

        if(selectedCell != null)
        {
            uint flags = (uint)selectedCell.Walls;

            if (ImGui.BeginTable("Walls", 2))
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.CheckboxFlags("North", ref flags, (uint)Walls.North);

                ImGui.TableNextColumn();
                ImGui.CheckboxFlags("East", ref flags, (uint)Walls.East);

                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                ImGui.CheckboxFlags("South", ref flags, (uint)Walls.South);

                ImGui.TableNextColumn();
                ImGui.CheckboxFlags("West", ref flags, (uint)Walls.West);

                ImGui.EndTable();
            }

            ImGui.SeparatorText("Walls");

            if(ImGui.BeginTable("WallTextures", 2))
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                DrawTextureSlot("North", ref selectedCell.NorthWallTexturePath);

                ImGui.TableNextColumn();
                DrawTextureSlot("East", ref selectedCell.EastWallTexturePath);

                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                DrawTextureSlot("South", ref selectedCell.SouthWallTexturePath);

                ImGui.TableNextColumn();
                DrawTextureSlot("West", ref selectedCell.WestWallTexturePath);

                ImGui.EndTable();
            }

            ImGui.SeparatorText("Floor / Ceiling");

            if(ImGui.BeginTable("FloorTable", 2))
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                DrawTextureSlot("Floor", ref selectedCell.FloorTexturePath);
                ImGui.TableNextColumn();
                DrawTextureSlot("Ceiling", ref selectedCell.CeilingTexturePath);

                ImGui.EndTable();
            }

            selectedCell.Walls = (Walls)flags;
        }
        else
        {
            string text = "Select a cell to view properties";

            ImGui.SetCursorPos((ImGui.GetContentRegionAvail() * 0.5f) - (ImGui.CalcTextSize(text) * 0.5f));
            ImGui.TextDisabled(text);
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

        float buttonWidth = 120.0f;
        float spacing = ImGui.GetStyle().ItemSpacing.X;

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - buttonWidth - spacing);

        ImGui.InputTextWithHint("##ConsoleInput", "Enter Command", ref test, 256,
            ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.EscapeClearsAll);

        ImGui.SameLine();

        if(ImGui.Button("Clear History", new Vector2(buttonWidth, 0))) { _consoleHistory.Clear(); }

        ImGui.End();
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
        float padding = 8.0f;
        float cellSize = 24f;

        float panelWidth = ImGui.GetContentRegionAvail().X;
        int columnCount = Math.Max(1, (int)(panelWidth / (cellSize + padding)));

        ImGui.Begin("Browser");

        if(ImGui.BeginTable("Assets", columnCount))
        {
            foreach (var texture in AssetManager.GetAssets<Texture2D>())
            {
                ImGui.TableNextColumn();

                ImGui.PushID(texture.Key);

                rlImGui.ImageButtonSize("##preview", texture.Value, new Vector2(96));

                if (ImGui.BeginDragDropSource())
                {
                    ImGui.SetDragDropPayload("texture_path", IntPtr.Zero, 0);

                    _draggedTexturePath = texture.Key;

                    ImGui.Text(texture.Key);
                    ImGui.EndDragDropSource();
                }

                ImGui.Text(Path.GetFileNameWithoutExtension(texture.Key));
                ImGui.TextDisabled(AssetManager.GetAssetType(texture.Value));

                ImGui.PopID();
            }

            /*foreach (var asset in AssetManager.Assets)
            {
                ImGui.TableNextColumn();

                ImGui.PushID(asset.Key);

                rlImGui.ImageButtonSize("##thumbnail", AssetManager.Load<Texture2D>(@"Assets\Textures\Man.png"), new Vector2(96));

                if (ImGui.BeginDragDropSource())
                {
                    ImGui.SetDragDropPayload("texture_path", IntPtr.Zero, 0);

                    _draggedTexturePath = asset.Key;

                    ImGui.Text(asset.Key);
                    ImGui.EndDragDropSource();
                }

                ImGui.Text(Path.GetFileNameWithoutExtension(asset.Key));
                ImGui.TextDisabled(AssetManager.GetAssetType(asset.Value));

                ImGui.PopID();
            }*/

            ImGui.EndTable();
        }

        ImGui.End();
    }

    private void DrawTextureSlot(string name, ref string texturePath)
    {
        ImGui.Text(name);

        rlImGui.ImageSize(AssetManager.Load<Texture2D>(texturePath), new Vector2(80));

        if(ImGui.BeginDragDropTarget() && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        {
            ImGui.AcceptDragDropPayload("texture_path");

            texturePath = _draggedTexturePath;

            ImGui.EndDragDropTarget();
        }
    }

    private void NewLevel()
    {
        EntityList.Clear();
        Cells = new Cell[WORLD_WIDTH, WORLD_HEIGHT];
        _levelName = "New Level";
        _playerStart = Vector2.Zero;

        Debug.Log("New level");
    }

    private (DialogResult result, string path) SaveLevel()
    {
        var result = Dialog.FileSave("hdl", Paths.MapsFolder);

        string path = null;

        if(result.IsOk)
        {
            Level level = Level.FromWorld(this);
            level.PlayerStart = _playerStart;

            path = Level.SaveToFile(level, result.Path);
        }

        return (result, path);
    }

    private void LoadEditorLevel()
    {
        var result = Dialog.FileOpen("hdl", Paths.MapsFolder);

        if(result.IsOk)
        {
            Debug.Log("Load level");
            Level editorLevel = Level.LoadFromFile(result.Path);

            EntityList = editorLevel.EntityList;
            Cells = editorLevel.Cells;

            _playerStart = editorLevel.PlayerStart;
            _levelName = Path.GetFileNameWithoutExtension(result.Path);
        }
    }

    private async Task RunLevel(string path)
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

    private Cell CreateDefaultCell(int x, int y)
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
