using ImGuiNET;

namespace Game.LevelEditor.Panels;

public class LevelSettings : EditorPanel
{
    private int _startRotationIndex = 0;
    private Dictionary<string, float> _directions;
    public LevelSettings(EditorContext context) : base(context)
    {
        // may refactor to an enum
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
        ImGui.Begin("Level Settings", ImGuiWindowFlags.NoDocking);

        if(ImGui.InputText("Level Name", ref _context.LevelName, 16));
        {
            _context.MarkDirty();
        }

        if(ImGui.InputFloat2("Start Position", ref _context.PlayerStart))
        {
            _context.MarkDirty();
        }
        
        if(ImGui.Combo("Start Rotation", ref _startRotationIndex, _directions.Keys.ToArray(), 4))
        {
            _context.MarkDirty();
        }        

        _context.StartRotation = _directions.Values.ToArray()[_startRotationIndex];

        ImGui.BeginDisabled();
        if(ImGui.DragInt("LevelSizeX", ref _context.World.SizeX, 1, 1, 25))
        {
            _context.MarkDirty();
        }
        
        if(ImGui.DragInt("LevelSizeY", ref _context.World.SizeY, 1, 1, 25))
        {
            _context.MarkDirty();
        }

        ImGui.EndDisabled();

        ImGui.End();
    }
}
