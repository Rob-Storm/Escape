using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;

namespace Game.LevelEditor.Panels;

public class PropertyInspector : EditorPanel
{
    public PropertyInspector(EditorContext context) : base(context)
    {
    }

    public override void Draw()
    {
        ImGui.Begin("Properties");

        if (_context.SelectedCell != null)
        {
            uint flags = (uint)_context.SelectedCell.Walls;

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

            if (ImGui.BeginTable("WallTextures", 2))
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                DrawTextureSlot("North", ref _context.SelectedCell.NorthWallTexturePath);

                ImGui.TableNextColumn();
                DrawTextureSlot("East", ref _context.SelectedCell.EastWallTexturePath);

                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                DrawTextureSlot("South", ref _context.SelectedCell.SouthWallTexturePath);

                ImGui.TableNextColumn();
                DrawTextureSlot("West", ref _context.SelectedCell.WestWallTexturePath);

                ImGui.EndTable();
            }

            ImGui.SeparatorText("Floor / Ceiling");

            if (ImGui.BeginTable("FloorTable", 2))
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                DrawTextureSlot("Floor", ref _context.SelectedCell.FloorTexturePath);
                ImGui.TableNextColumn();
                DrawTextureSlot("Ceiling", ref _context.SelectedCell.CeilingTexturePath);

                ImGui.EndTable();
            }

            _context.SelectedCell.Walls = (Walls)flags;
        }
        else
        {
            string text = "Select a cell to view properties";

            ImGui.SetCursorPos((ImGui.GetContentRegionAvail() * 0.5f) - (ImGui.CalcTextSize(text) * 0.5f));
            ImGui.TextDisabled(text);
        }

        ImGui.End();
    }

    private void DrawTextureSlot(string name, ref string texturePath)
    {
        ImGui.Text(name);

        rlImGui.ImageSize(AssetManager.Load<Texture2D>(texturePath), new Vector2(80));

        if (ImGui.BeginDragDropTarget() && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        {
            ImGui.AcceptDragDropPayload("texture_path");

            texturePath = _context.DraggedTexturePath;

            ImGui.EndDragDropTarget();
        }
    }
}
