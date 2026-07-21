using ImGuiNET;

namespace Game.LevelEditor.Panels;

public class MenuBar : EditorPanel
{
    private Editor _editor;
    public MenuBar(EditorContext context) : base(context)
    {
        _editor = (Editor)context.World;
    }

    public override void Draw()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New", "Ctrl+N")) { _context.LevelFileService.Save(_editor); }
                if (ImGui.MenuItem("Save", "Ctrl+S")) { _context.LevelFileService.Save(_editor); }
                if (ImGui.MenuItem("Open", "Ctrl+O")) { _context.LevelFileService.Load(_editor); }
                ImGui.EndMenu();
            }

            if (ImGui.MenuItem("Run"))
            {
                var output = _context.LevelFileService.Save(_editor);

                if (output.result.IsOk)
                {
                    _context.PlayModeService.RunLevel(output.result.Path);
                }
            }

            ImGui.EndMainMenuBar();
        }
    }
}
