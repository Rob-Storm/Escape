using ImGuiNET;
using System.Numerics;

namespace Game.LevelEditor.Panels;

public class MapGrid : EditorPanel
{
    private Editor _editor;
    public MapGrid(EditorContext context) : base(context)
    {
        _editor = (Editor)context.World;
    }

    public override void Draw()
    {
        ImGui.Begin("Map");

        var drawList = ImGui.GetWindowDrawList();

        Vector2 origin = ImGui.GetCursorScreenPos();

        float cellSize = 32f;

        uint gridColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 1.0f));

        uint wallColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 0f, 0f, 1.0f));

        uint filledColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0.75f, 0.75f, 0.75f, 1f));

        uint selectedColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1.0f, 0.5f, 0f, 0.75f));

        uint playerStartColor = ImGui.ColorConvertFloat4ToU32(new Vector4(0f, 0.75f, 0f, 0.75f));

        
        for (int y = 0; y < World.WORLD_HEIGHT; y++)
        {
            for (int x = 0; x < World.WORLD_WIDTH; x++)
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

                if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    _context.SelectedX = x;
                    _context.SelectedY = y;

                    Debug.Log($"Selected cell at '{x},{y}'");
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

        for (int y = 0; y < World.WORLD_HEIGHT; y++)
        {
            for (int x = 0; x < World.WORLD_WIDTH; x++)
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
}
