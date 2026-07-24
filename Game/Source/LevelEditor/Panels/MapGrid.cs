using ImGuiNET;
using Raylib_cs;
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

    private bool _drawGrid = true;

    private uint _gridColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 1.0f));

    private uint _wallColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 0f, 0f, 1.0f));

    private uint _filledColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.75f, 0.75f, 0.75f, 1f));

    private uint _roomPreviewColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0f, 0.75f, 1f));

    private uint _selectedColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 0.5f, 0f, 0.75f));

    private uint _playerStartColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0.75f, 0f, 0.75f));

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

        ImGui.SameLine();

        ImGui.Checkbox("Draw Grid", ref _drawGrid);
    }

    public override void Draw()
    {
        ImGui.Begin("Map", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

        DrawTools();

        ImGui.BeginChild("ScaleArea", ImGui.GetContentRegionAvail(), ImGuiChildFlags.None, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

        if (ImGui.IsWindowHovered())
        {
            float wheel = ImGui.GetIO().MouseWheel;

            if (wheel != 0)
            {
                _zoom += wheel * 0.05f;

                _zoom = Math.Clamp(_zoom, 0.1f, 2.5f);
            }
        }

        if (ImGui.IsWindowHovered() && ImGui.IsMouseDragging(ImGuiMouseButton.Middle))
        {
            _pan += ImGui.GetIO().MouseDelta;
        }


        var drawList = ImGui.GetWindowDrawList();

        Vector2 origin = ImGui.GetCursorScreenPos() + _pan;

        float cellSize = BASE_CELL_SIZE * _zoom;

        Vector2 mapSize = new Vector2(_context.World.SizeX, _context.World.SizeY) * cellSize;

        ImGui.SetCursorScreenPos(origin);

        ImGui.InvisibleButton($"MapHitTest", mapSize);

        bool hovered = ImGui.IsItemHovered();

        if (hovered)
        {
            Vector2 mouse = ImGui.GetIO().MousePos - origin;
            int cellX = (int)(mouse.X / cellSize);
            int cellY = (int)(mouse.Y / cellSize);

            if (ImGui.IsItemClicked())
            {
                HandleCellClick(cellX, cellY);
            }

            if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                HandleCellPress(cellX, cellY);
            }

            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                HandleCellRelease(cellX, cellY);
            }
        }

        ImGui.PushClipRect(origin, origin + mapSize, true);


        if (_drawGrid)
        {
            DrawGrid(drawList, origin, cellSize);
        }
        else
        {
            drawList.AddRect(origin, origin + mapSize, _gridColor);
        }

        for (int y = 0; y < _context.World.SizeY; y++)
        {
            for (int x = 0; x < _context.World.SizeX; x++)
            {
                Cell cell = _editor.GetCell(x, y);

                Vector2 cellMin = origin + new Vector2(x * cellSize, y * cellSize);

                Vector2 cellMax = cellMin + new Vector2(cellSize, cellSize);

                if (cell != null)
                {
                    drawList.AddRectFilled(cellMin, cellMax, _filledColor);

                    if (cell == _editor.GetCell(_context.PlayerStart))
                    {
                        drawList.AddRectFilled(cellMin, cellMax, _playerStartColor);
                    }
                }

                if (_context.ToolMode == ToolMode.Select)
                {
                    if (_context.SelectedX == x && _context.SelectedY == y)
                    {
                        drawList.AddRectFilled(cellMin, cellMax, _selectedColor);
                    }
                }

                if(_context.ToolMode == ToolMode.Room)
                {
                    if (x == _startRoomCellX || x == _cursorRoomCellX)
                    {
                        if (y == _startRoomCellY || y == _cursorRoomCellY)
                        {
                            drawList.AddRectFilled(cellMin, cellMax, _roomPreviewColor);
                        }
                    }
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
                        DrawWallLine(Walls.North, drawList, cellMin, cellSize, _wallColor);
                    }
                    if (cell.Walls.HasFlag(Walls.East))
                    {
                        DrawWallLine(Walls.East, drawList, cellMin, cellSize, _wallColor);
                    }
                    if (cell.Walls.HasFlag(Walls.South))
                    {
                        DrawWallLine(Walls.South, drawList, cellMin, cellSize, _wallColor);
                    }
                    if (cell.Walls.HasFlag(Walls.West))
                    {
                        DrawWallLine(Walls.West, drawList, cellMin, cellSize, _wallColor);
                    }
                }
            }
        }

        ImGui.PopClipRect();

        ImGui.EndChild();

        ImGui.End();
    }

    private void DrawGrid(ImDrawListPtr drawList, Vector2 origin, float cellSize)
    {
        for (int x = 0; x < _context.World.SizeX; x++)
        {
            float px = origin.X + x * cellSize;

            drawList.AddLine(new Vector2(px, origin.Y), new Vector2(px, origin.Y + _context.World.SizeY * cellSize), _gridColor);
        }

        for (int y = 0; y < _context.World.SizeY; y++)
        {
            float py = origin.Y + y * cellSize;

            drawList.AddLine(new Vector2(origin.X, py), new Vector2(origin.X + _context.World.SizeX * cellSize, py), _gridColor);
        }
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
                _context.SetSelectedObject(_context.World.GetCell(cellX, cellY));
                break;
            case ToolMode.Draw:
                break;
            case ToolMode.Room:
                _startRoomCellX = cellX;
                _startRoomCellY = cellY;
                Debug.Log($"Start {cellX} {cellY}");
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
                newCell.NorthWallTexture = AssetManager.Load<Texture2D>(_context.ToolSettings.NorthWallTexturePath);
                newCell.EastWallTexture = AssetManager.Load<Texture2D>(_context.ToolSettings.EastWallTexturePath);
                newCell.WestWallTexture = AssetManager.Load<Texture2D>(_context.ToolSettings.SouthWallTexturePath);
                newCell.SouthWallTexture = AssetManager.Load<Texture2D>(_context.ToolSettings.WestWallTexturePath);

                newCell.FloorTexture = AssetManager.Load<Texture2D>(_context.ToolSettings.FloorTexturePath);
                newCell.CeilingTexture = AssetManager.Load<Texture2D>(_context.ToolSettings.CeilingTexturePath);

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
                Debug.Log($"End {cellX} {cellY}");

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

        int startX, startY, endX, endY;

        startX = (int)start.X;
        startY = (int)start.Y;

        endX = (int)end.X;
        endY = (int)end.Y;

        // Check to see if the room was start right->left or bottom->top and flip
        if (start.X > end.X)
        {
            startX = (int)end.X;
            endX = (int)start.X;
        }
        if(start.Y > end.Y)
        {
            startY = (int)end.Y;
            endY = (int)start.Y;
        }


        for (int x = startX; x < endX + 1; x++)
        {
            for (int y = startY; y < endY + 1; y++)
            {
                Cell newCell = new Cell(x, y);

                newCell.NorthWallTexture = AssetManager.Load<Texture2D>(_context.ToolSettings.NorthWallTexturePath);
                newCell.EastWallTexture = AssetManager.Load<Texture2D>(_context.ToolSettings.EastWallTexturePath);
                newCell.WestWallTexture = AssetManager.Load<Texture2D>(_context.ToolSettings.SouthWallTexturePath);
                newCell.SouthWallTexture = AssetManager.Load<Texture2D>(_context.ToolSettings.WestWallTexturePath);

                newCell.FloorTexture = AssetManager.Load<Texture2D>(_context.ToolSettings.FloorTexturePath);
                newCell.CeilingTexture = AssetManager.Load<Texture2D>(_context.ToolSettings.CeilingTexturePath);

                Walls newWalls = Walls.None;

                if (y == startY)
                {
                    newWalls |= Walls.North;
                }
                if (y == endY)
                {
                    newWalls |= Walls.South;
                }
                if (x == startX)
                {
                    newWalls |= Walls.West;
                }
                if (x == endX)
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