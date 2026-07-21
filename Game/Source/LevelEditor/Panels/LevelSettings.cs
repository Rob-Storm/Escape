using ImGuiNET;

namespace Game.LevelEditor.Panels;

public class LevelSettings : EditorPanel
{
    public LevelSettings(Editor editor) : base(editor)
    {
    }

    public override void Draw()
    {
        ImGui.Begin("Level Settings");

        ImGui.InputText("Level Name", ref _editor.LevelName, 16);
        ImGui.InputFloat2("Player Start", ref _editor.PlayerStart);

        ImGui.End();
    }
}
