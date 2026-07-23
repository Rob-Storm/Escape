using ImGuiNET;

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
            foreach (var asset in AssetManager.GetAssets().Where(a => Directory.GetParent(a.Key).Name  != "Editor"))
            {
                AssetTypeInfo info = AssetManager.GetAssetTypeInfo(asset.Value.GetType());

                ImGui.TableNextColumn();

                ImGui.PushID(asset.Key);

                if(info.DrawPreview(asset.Value))
                {
                    _context.SetSelectedAsset(asset.Value);
                }

                if (ImGui.BeginDragDropSource())
                {
                    ImGui.SetDragDropPayload("asset_path", IntPtr.Zero, 0);

                    _context.DraggedAssetPath = asset.Key;

                    ImGui.Text(asset.Key);
                    ImGui.EndDragDropSource();
                }

                ImGui.TextColored(info.Color, Path.GetFileNameWithoutExtension(asset.Key));
                ImGui.TextDisabled(info.Type.Name);

                ImGui.PopID();
            }

            ImGui.EndTable();
        }

        ImGui.End();
    }
}
