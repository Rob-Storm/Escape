using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;

namespace Game.LevelEditor.Panels;

public class ToolSettings : EditorPanel
{
    private List<Type> _entityTypeList;
    private List<string> _typeStringList;
    private int _entityIndex;

    public ToolSettings(EditorContext context) : base(context)
    {
        _entityTypeList = new List<Type>();
        _typeStringList = new List<string>();

        IEnumerable<Type> types = typeof(Entity).Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Entity)) &&
            Attribute.GetCustomAttribute(type, typeof(HideFromSpawnMenuAttribute)) == null);

        _entityTypeList = types.ToList();

        _typeStringList = types.Select(name => name.Name).ToList();
    }

    public override void Draw()
    {
        ImGui.Begin("Tool Settings");

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

        ImGui.SeparatorText("Spawn Entity");

        ImGui.Combo("Entity Class", ref _entityIndex, _typeStringList.ToArray(), _typeStringList.Count);

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