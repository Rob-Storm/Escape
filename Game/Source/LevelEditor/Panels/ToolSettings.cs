using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;

namespace Game.LevelEditor.Panels;

public class ToolSettings : EditorPanel
{
    private int _entityIndex;

    private Dictionary<string, Type> _entityTypeName;

    public ToolSettings(EditorContext context) : base(context)
    {
        _entityTypeName = new Dictionary<string, Type>();

        IEnumerable<Type> types = typeof(Entity).Assembly.GetTypes();
            //.Where(type => type.IsSubclassOf(typeof(Entity)) &&
            //Attribute.GetCustomAttribute(type, typeof(HideFromSpawnMenuAttribute)) == null);

        foreach(Type type in types)
        {
            if (type.IsSubclassOf(typeof(Entity)) && Attribute.GetCustomAttribute(type, typeof(HideFromSpawnMenuAttribute)) == null)
            {
                _entityTypeName.Add(type.Name, type);
            }
        }
    }

    public override void Draw()
    {
        ImGui.Begin("Tool Settings");

        if(ImGui.TreeNode("Paint/Room"))
        {

            uint flags = (uint)_context.ToolSettings.Walls;

            ImGui.SeparatorText("Create Walls");

            if (ImGui.BeginTable("Create Wall", 2))
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

                _context.ToolSettings.Walls = (Walls)flags;

                ImGui.EndTable();
            }

            ImGui.SeparatorText("Walls");

            if (ImGui.BeginTable("WallTextures", 2))
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                DrawTextureSlot("North", ref _context.ToolSettings.NorthWallTexturePath);

                ImGui.TableNextColumn();
                DrawTextureSlot("East", ref _context.ToolSettings.EastWallTexturePath);

                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                DrawTextureSlot("South", ref _context.ToolSettings.SouthWallTexturePath);

                ImGui.TableNextColumn();
                DrawTextureSlot("West", ref _context.ToolSettings.WestWallTexturePath);

                ImGui.EndTable();
            }

            ImGui.SeparatorText("Floor / Ceiling");

            if (ImGui.BeginTable("FloorTable", 2))
            {
                ImGui.TableNextRow();

                ImGui.TableNextColumn();
                DrawTextureSlot("Floor", ref _context.ToolSettings.FloorTexturePath);
                ImGui.TableNextColumn();
                DrawTextureSlot("Ceiling", ref _context.ToolSettings.CeilingTexturePath);

                ImGui.EndTable();
            }

            ImGui.TreePop();
        }

        if(ImGui.TreeNode("Entity"))
        {
            ImGui.Combo("Entity Class", ref _entityIndex, _entityTypeName.Keys.ToArray(), _entityTypeName.Count);
            _context.EntitySpawnClass = _entityTypeName.Values.ToArray()[_entityIndex];

            ImGui.TreePop();
        }

        ImGui.End();
    }

    // HACK: Copy-pasted from PropertyInspector.cs
    private void DrawTextureSlot(string name, ref string texturePath)
    {
        ImGui.Text(name);

        rlImGui.ImageSize(AssetManager.Load<Texture2D>(texturePath), new Vector2(80));


        if (ImGui.BeginDragDropTarget() && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        {
            ImGui.AcceptDragDropPayload("texture_path");

            texturePath = _context.DraggedAssetPath!;

            ImGui.EndDragDropTarget();
        }
    }
}

public struct PaintWallSettings
{
    public string NorthWallTexturePath, EastWallTexturePath, WestWallTexturePath, SouthWallTexturePath;
    public string FloorTexturePath, CeilingTexturePath;

    public Walls Walls;

    public PaintWallSettings()
    {
        NorthWallTexturePath = @"Assets\Textures\Default.png";
        EastWallTexturePath = @"Assets\Textures\Default.png";
        WestWallTexturePath = @"Assets\Textures\Default.png";
        SouthWallTexturePath = @"Assets\Textures\Default.png";

        FloorTexturePath = @"Assets\Textures\Default.png";
        CeilingTexturePath = @"Assets\Textures\Default.png";

        Walls = Walls.None;
    }
}