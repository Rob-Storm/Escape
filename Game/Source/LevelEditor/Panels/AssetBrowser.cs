using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;

namespace Game.LevelEditor.Panels;

public class AssetBrowser : EditorPanel
{
    public AssetBrowser(EditorContext context) : base(context)
    {
    }

    public override void Draw()
    {
        float padding = 8.0f;
        float cellSize = 24f;

        float panelWidth = ImGui.GetContentRegionAvail().X;
        int columnCount = Math.Max(1, (int)(panelWidth / (cellSize + padding)));

        ImGui.Begin("Browser");

        if (ImGui.BeginTable("Assets", columnCount))
        {
            foreach (var texture in AssetManager.GetAssets<Texture2D>())
            {
                ImGui.TableNextColumn();

                ImGui.PushID(texture.Key);

                rlImGui.ImageButtonSize("##preview", texture.Value, new Vector2(96));

                if (ImGui.BeginDragDropSource())
                {
                    ImGui.SetDragDropPayload("texture_path", IntPtr.Zero, 0);

                    _context.DraggedTexturePath = texture.Key;

                    ImGui.Text(texture.Key);
                    ImGui.EndDragDropSource();
                }

                ImGui.Text(Path.GetFileNameWithoutExtension(texture.Key));
                ImGui.TextDisabled(AssetManager.GetAssetType(texture.Value));

                ImGui.PopID();
            }

            ImGui.EndTable();
        }

        ImGui.End();
    }
}
