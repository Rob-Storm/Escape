using ImGuiNET;
using System.Numerics;

namespace Game.LevelEditor.Panels;

public class MapGrid : EditorPanel
{
    private Editor _editor;
    private float _zoom = 1.0f;
    private Vector2 _pan = Vector2.Zero;
    private const float BASE_CELL_SIZE = 32f;

    public MapGrid(EditorContext context) : base(context)
    {
        _editor = (Editor)context.World;
    }

    private void DrawTools()
    {
        if (ImGui.Button($"{IconFonts.FontAwesome6.ArrowPointer} Select"))
        {
            _context.ToolMode = ToolMode.Select;
            Debug.Log("Select");
        }

        ImGui.SameLine();

        if (ImGui.Button($"{IconFonts.FontAwesome6.Paintbrush} Draw"))
        {
            _context.ToolMode = ToolMode.Draw;
            Debug.Log("Draw");
        }

        ImGui.SameLine();

        if (ImGui.Button($"{IconFonts.FontAwesome6.Hammer} Quick Room"))
        {
            _context.ToolMode = ToolMode.Room;
            Debug.Log("Room");
        }

        ImGui.SameLine();

        if (ImGui.Button($"{IconFonts.FontAwesome6.Eraser} Erase"))
        {
            _context.ToolMode = ToolMode.Delete;
            Debug.Log("Delete");
        }

        ImGui.SameLine();

        ImGui.Text($"Current Tool: {_context.ToolMode.ToString()}");
    }

    public override void Draw()
    {
        ImGui.Begin("Map", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

        DrawTools();

        ImGui.BeginChild("ScrollArea");

        if (ImGui.IsWindowHovered())
        {
            float wheel = ImGui.GetIO().MouseWheel;

            if(wheel != 0)
            {
                _zoom += wheel * 0.05f;

                _zoom = Math.Clamp(_zoom, 0.5f, 2f);
            }
        }

        if (ImGui.IsWindowHovered() && ImGui.IsMouseDragging(ImGuiMouseButton.Middle))
        {
            _pan += ImGui.GetIO().MouseDelta;
        }


        var drawList = ImGui.GetWindowDrawList();

        Vector2 origin = ImGui.GetCursorScreenPos() + _pan;

        float cellSize = BASE_CELL_SIZE * _zoom;

        uint gridColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 1.0f));

        uint wallColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 0f, 0f, 1.0f));

        uint filledColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.75f, 0.75f, 0.75f, 1f));

        uint selectedColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 0.5f, 0f, 0.75f));

        uint playerStartColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0.75f, 0f, 0.75f));

        
        for (int y = 0; y < _context.World.SizeY; y++)
        {
            for (int x = 0; x < _context.World.SizeX; x++)
            {
                Cell cell = _editor.GetCell(x, y);

                Vector2 cellMin = origin + new Vector2(x * cellSize, y * cellSize);

                Vector2 cellMax = cellMin + new Vector2(cellSize, cellSize);

                ImGui.SetCursorScreenPos(cellMin);

                ImGui.InvisibleButton($"Cell##{x}_{y}", new Vector2(cellSize, cellSize));

                bool hovered = ImGui.IsItemHovered();

                if (cell != null)
                {
                    drawList.AddRectFilled(cellMin, cellMax, filledColor);

                    if (cell == _editor.GetCell(_context.PlayerStart))
                    {
                        drawList.AddRectFilled(cellMin, cellMax, playerStartColor);
                    }
                }

                if (_context.SelectedX == x && _context.SelectedY == y)
                {
                    drawList.AddRectFilled(cellMin, cellMax, selectedColor);
                }

                drawList.AddRect(cellMin, cellMax, gridColor);

                if(ImGui.IsItemClicked())
                {
                    HandleCellClick(x, y);
                }

                if(ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenBlockedByActiveItem) && ImGui.IsMouseDown(ImGuiMouseButton.Left))
                {
                    HandleCellPress(x, y);
                }

                if (ImGui.BeginPopupContextItem($"CellOptions##{x}_{y}"))
                {
                    if (cell == null)
                    {
                        if (ImGui.MenuItem("Add Cell"))
                        {
                            _editor.SetCell(x, y, _editor.CreateDefaultCell(x, y));
                        }
                    }
                    else
                    {
                        if (ImGui.MenuItem("Remove Cell"))
                        {
                            _editor.SetCell(x, y, null);

                            if (_context.SelectedX == x && _context.SelectedY == y)
                            {
                                _context.SelectedX = -1;
                                _context.SelectedY = -1;
                            }
                        }

                        if (ImGui.MenuItem("Set Start Position"))
                        {
                            _context.PlayerStart = new Vector2(x, y);
                        }
                    }

                    ImGui.EndPopup();
                }

            }
        }


        // A second pass lets us guarantee the wall lines are drawn last 
        for (int y = 0; y < _context.World.SizeY; y++)
        {
            for (int x = 0; x < _context.World.SizeX; x++)
            {
                Cell cell = _editor.GetCell(x, y);

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

        ImGui.EndChild();

        ImGui.End();
    }

    private void HandleCellClick(int cellX, int cellY)
    {
        switch (_context.ToolMode)
        {
            case ToolMode.Select:
                _context.SelectedX = cellX;
                _context.SelectedY = cellY;
                Debug.Log($"Selected cell at '{cellX},{cellY}'");
                break;
            case ToolMode.Draw:
                break;
            case ToolMode.Room:
                break;
            case ToolMode.Delete:
                break;
        }
    }

    private void HandleCellPress(int cellX, int cellY)
    {
        switch (_context.ToolMode)
        {
            case ToolMode.Select:
                break;
            case ToolMode.Draw:
                Cell newCell = new Cell(cellX, cellY);
                newCell.NorthWallTexturePath = _context.ToolSettings.NorthWallTexturePath;
                newCell.EastWallTexturePath = _context.ToolSettings.EastWallTexturePath;
                newCell.WestWallTexturePath = _context.ToolSettings.WestWallTexturePath;
                newCell.SouthWallTexturePath = _context.ToolSettings.SouthWallTexturePath;

                newCell.FloorTexturePath = _context.ToolSettings.FloorTexturePath;
                newCell.CeilingTexturePath = _context.ToolSettings.CeilingTexturePath;

                newCell.Walls = _context.ToolSettings.Walls;

                _editor.SetCell(cellX, cellY, newCell);

                break;
            case ToolMode.Room:
                break;
            case ToolMode.Delete:
                _editor.SetCell(cellX, cellY, null);

                if (_context.SelectedX == cellX && _context.SelectedY == cellY)
                {
                    _context.SelectedX = -1;
                    _context.SelectedY = -1;
                }
                break;
        }
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

        drawList.AddLine(start, end, color, 1.5f * _zoom);
    }
}