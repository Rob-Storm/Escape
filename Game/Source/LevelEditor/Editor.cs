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

    private string _draggedTexturePath;

    private string _defaultPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Maps");

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

        if(selectedCell != null)
        {
            selectedCell.RenderBounds(Color.Orange, Color.Green);
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
                if (ImGui.MenuItem("Add Cell")) 
                {
                    Cell cell = new Cell();
                    cell.NorthWallTexturePath = @"Assets\Textures\Wall.png";
                    cell.EastWallTexturePath = @"Assets\Textures\Wall.png";
                    cell.SouthWallTexturePath = @"Assets\Textures\Wall.png";
                    cell.WestWallTexturePath = @"Assets\Textures\Wall.png";

                    cell.FloorTexturePath = @"Assets\Textures\Floor.png";
                    cell.CeilingTexturePath = @"Assets\Textures\Ceiling.png";

                    CellList.Add(cell); 
                }
                ImGui.EndMenu();
            }

            if (ImGui.MenuItem("Run")) 
            {
                var output = SaveLevel();

                if(output.result.IsOk)
                {
                    RunLevel(output.path);
                }
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

                    if (ImGui.BeginPopupContextItem("Options"))
                    {
                        ImGui.PushID(i);

                        if (ImGui.MenuItem("Delete"))
                        {
                            selectedCell = null;

                            CellList.Remove(CellList[i]);
                            i--;
                        }

                        ImGui.PopID();

                        ImGui.EndPopup();

                    }
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
        CellList.Clear();
        _levelName = "New Level";
        _playerStart = Vector2.Zero;

        Debug.Log("New level");
    }

    private (DialogResult result, string path) SaveLevel()
    {
        var result = Dialog.FileSave("hdl", _defaultPath);

        string path = null;

        if(result.IsOk)
        {
            Level level = Level.FromWorld(this);
            level.PlayerStart = _playerStart;

            path = Level.SaveToFile(level, _levelName);

            Debug.Log("Saving level");
        }

        return (result, path);
    }

    private void LoadEditorLevel()
    {
        var result = Dialog.FileOpen("hdl", _defaultPath);

        if(result.IsOk)
        {
            Debug.Log("Load level");
            Level editorLevel = Level.LoadFromFile(result.Path);

            EntityList = editorLevel.EntityList;
            CellList = editorLevel.CellList;

            _playerStart = editorLevel.PlayerStart;
            _levelName = Path.GetFileNameWithoutExtension(result.Path);
        }
    }

    private async Task RunLevel(string path)
    {
        Debug.Log("Starting play session");

        ProcessStartInfo info = new ProcessStartInfo(Path.Combine(Directory.GetCurrentDirectory(), "Game.exe"), $"--level \"{path}\"");

        info.RedirectStandardOutput = true;
        info.UseShellExecute = false;
        info.CreateNoWindow = false;

        using(Process process = new Process())
        {
            process.StartInfo = info;
            process.Start();

            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();

            await process.WaitForExitAsync();

            string result = await outputTask;

            Debug.Log("End play session");
            Debug.Log($"Exit code: {process.ExitCode}");
        }
    }

}
