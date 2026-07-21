using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;

namespace Game.LevelEditor.Panels;

public class PropertyInspector : EditorPanel
{
    public PropertyInspector(Editor editor) : base(editor)
    {
    }

    public override void Draw()
    {
        ImGui.Begin("Properties");

        if (_editor.SelectedCell != null)
        {
            uint flags = (uint)_editor.SelectedCell.Walls;

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
                DrawTextureSlot("North", ref _editor.SelectedCell.NorthWallTexturePath);

                ImGui.TableNextColumn();
                DrawTextureSlot("East", ref _editor.SelectedCell.EastWallTexturePath);

                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                DrawTextureSlot("South", ref _editor.SelectedCell.SouthWallTexturePath);

                ImGui.TableNextColumn();
                DrawTextureSlot("West", ref _editor.SelectedCell.WestWallTexturePath);

                ImGui.EndTable();
            }

            ImGui.SeparatorText("Floor / Ceiling");

            if (ImGui.BeginTable("FloorTable", 2))
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                DrawTextureSlot("Floor", ref _editor.SelectedCell.FloorTexturePath);
                ImGui.TableNextColumn();
                DrawTextureSlot("Ceiling", ref _editor.SelectedCell.CeilingTexturePath);

                ImGui.EndTable();
            }

            _editor.SelectedCell.Walls = (Walls)flags;
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

            texturePath = _editor.DraggedTexturePath;

            ImGui.EndDragDropTarget();
        }
    }
}
