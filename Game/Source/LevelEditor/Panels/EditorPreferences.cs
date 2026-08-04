using ImGuiNET;

namespace Game.LevelEditor.Panels;

/*
 * Todo: Impelement
 */
public class EditorPreferences : EditorPanel
{
    // this is where settings that should not be
    // synchronized by version control will be saved
    // (e.g. mouse sensitivity, move speed, etc.)

    public EditorPreferences(EditorContext context) : base(context)
    {
    }

    public override void Draw()
    {
        ImGui.Begin("Editor Preferences", ImGuiWindowFlags.NoDocking);

        float mouseSens = _context.Camera.Sensitivity;

        ImGui.InputFloat("Camera Speed", ref _context.Camera.MoveSpeed);
        ImGui.DragFloat("Mouse Sensitivity", ref mouseSens, 0.5f, 1f, 10.0f);

        _context.Camera.Sensitivity = mouseSens;

        ImGui.End();
    }
}
