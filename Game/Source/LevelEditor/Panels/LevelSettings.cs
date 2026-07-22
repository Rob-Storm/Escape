using ImGuiNET;

namespace Game.LevelEditor.Panels;

public class LevelSettings : EditorPanel
{
    public LevelSettings(EditorContext context) : base(context)
    {

    }

    public override void Draw()
    {
        ImGui.Begin("Level Settings");

        ImGui.InputText("Level Name", ref _context.LevelName, 16);
        ImGui.InputFloat2("Start Position", ref _context.PlayerStart);
        ImGui.DragFloat("Start Rotation", ref _context.StartRotation, 1f, 0f, 359f);

        ImGui.DragInt("LevelSizeX", ref _context.World.SizeX, 1, 1, 25);
        ImGui.DragInt("LevelSizeY", ref _context.World.SizeY, 1, 1, 25);

        ImGui.End();
    }
}
