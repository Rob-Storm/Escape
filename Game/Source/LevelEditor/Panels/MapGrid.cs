using ImGuiNET;
using System.Numerics;

namespace Game.LevelEditor.Panels;

public class MapGrid : EditorPanel
{
    private Editor _editor;
    private float _zoom = 1.0f;
    private Vector2 _pan = Vector2.Zero;
    private const float BASE_CELL_SIZE = 32f;

    private int _startRoomCellX = -1;
    private int _startRoomCellY = -1;

    private int _cursorRoomCellX = -1;
    private int _cursorRoomCellY = -1;

    private int _endRoomCellX = -1;
    private int _endRoomCellY = -1;

    public MapGrid(EditorContext context) : base(context)
    {
        _editor = (Editor)context.World;


    }

    private void DrawTools()
    {
        DrawToolButton($"{IconFonts.FontAwesome6.ArrowPointer} Select", ToolMode.Select);

        ImGui.SameLine();

        DrawToolButton($"{IconFonts.FontAwesome6.Paintbrush} Draw", ToolMode.Draw);

        ImGui.SameLine();

        DrawToolButton($"{IconFonts.FontAwesome6.Hammer} Room", ToolMode.Room);

        ImGui.SameLine();

        DrawToolButton($"{IconFonts.FontAwesome6.Eraser} Erase", ToolMode.Delete);

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

            if (wheel != 0)
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

        uint roomPreviewColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0f, 0.75f, 1f));

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

                if (x == _startRoomCellX || x == _cursorRoomCellX)
                {
                    if (y == _startRoomCellY || y == _cursorRoomCellY)
                    {
                        drawList.AddRectFilled(cellMin, cellMax, roomPreviewColor);
                    }
                }

                drawList.AddRect(cellMin, cellMax, gridColor);

                if (ImGui.IsItemClicked())
                {
                    HandleCellClick(x, y);
                }

                if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenBlockedByActiveItem) && ImGui.IsMouseDown(ImGuiMouseButton.Left))
                {
                    HandleCellPress(x, y);
                }

                if (hovered && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                {
                    HandleCellRelease(x, y);
                    Debug.Log($"Release cell {x},{y}");
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


        // A second pass lets us guarantee the wall lines are drawn on top of the grid 
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
    private void DrawToolButton(string label, ToolMode mode)
    {
        ImGui.BeginDisabled(_context.ToolMode == mode);

        if (ImGui.Button(label))
        {
            _context.ToolMode = mode;
            Debug.Log($"Set active mode: {mode}");
        }

        ImGui.EndDisabled();
    }

    private void HandleCellClick(int cellX, int cellY)
    {
        switch (_context.ToolMode)
        {
            case ToolMode.Select:
                _context.SelectedX = cellX;
                _context.SelectedY = cellY;
                break;
            case ToolMode.Draw:
                break;
            case ToolMode.Room:
                _startRoomCellX = cellX;
                _startRoomCellY = cellY;
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
                _cursorRoomCellX = cellX;
                _cursorRoomCellY = cellY;
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

    private void HandleCellRelease(int cellX, int cellY)
    {
        switch (_context.ToolMode)
        {
            case ToolMode.Select:
                break;
            case ToolMode.Draw:
                break;
            case ToolMode.Room:
                _endRoomCellX = cellX;
                _endRoomCellY = cellY;

                CreateRoom(new Vector2(_startRoomCellX, _startRoomCellY), new Vector2(_endRoomCellX, _endRoomCellY));

                _startRoomCellX = -1;
                _startRoomCellY = -1;

                _cursorRoomCellX = -1;
                _cursorRoomCellY = -1;

                _endRoomCellX = -1;
                _endRoomCellY = -1;
                break;
            case ToolMode.Delete:
                break;
        }
    }

    private void CreateRoom(Vector2 start, Vector2 end)
    {
        Debug.Log($"Building room. Start: {start}. End: {end}");

        for (int x = (int)start.X; x < (int)end.X + 1; x++)
        {
            for (int y = (int)start.Y; y < (int)end.Y + 1; y++)
            {
                Cell newCell = new Cell(x, y);

                newCell.NorthWallTexturePath = _context.ToolSettings.NorthWallTexturePath;
                newCell.EastWallTexturePath = _context.ToolSettings.EastWallTexturePath;
                newCell.WestWallTexturePath = _context.ToolSettings.WestWallTexturePath;
                newCell.SouthWallTexturePath = _context.ToolSettings.SouthWallTexturePath;

                newCell.FloorTexturePath = _context.ToolSettings.FloorTexturePath;
                newCell.CeilingTexturePath = _context.ToolSettings.CeilingTexturePath;

                Walls newWalls = Walls.None;

                if (y == (int)start.Y)
                {
                    newWalls |= Walls.North;
                }
                if (y == (int)end.Y)
                {
                    newWalls |= Walls.South;
                }
                if (x == (int)start.X)
                {
                    newWalls |= Walls.West;
                }
                if (x == (int)end.X)
                {
                    newWalls |= Walls.East;
                }

                newCell.Walls = newWalls;

                _editor.SetCell(x, y, newCell);
            }
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