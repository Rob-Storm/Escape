using ImGuiNET;

namespace Game.LevelEditor.Panels;

public class LevelSettings : EditorPanel
{
    private int _startRotationIndex = 0;
    private Dictionary<string, float> _directions;
    public LevelSettings(EditorContext context) : base(context)
    {
        _directions = new Dictionary<string, float>
        {
            { "North", 0f },
            { "East", 90f },
            { "South", 180f },
            { "West", 270f }
        };
    }

    public override void Draw()
    {
        ImGui.Begin("Level Settings");

        ImGui.InputText("Level Name", ref _context.LevelName, 16);
        ImGui.InputFloat2("Start Position", ref _context.PlayerStart);
        ImGui.Combo("Start Rotation", ref _startRotationIndex, _directions.Keys.ToArray(),  4);

        _context.StartRotation = _directions.Values.ToArray()[_startRotationIndex];

        ImGui.BeginDisabled();
        ImGui.DragInt("LevelSizeX", ref _context.World.SizeX, 1, 1, 25);
        ImGui.DragInt("LevelSizeY", ref _context.World.SizeY, 1, 1, 25);
        ImGui.EndDisabled();

        ImGui.End();
    }
}
