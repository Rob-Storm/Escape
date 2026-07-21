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
                if (ImGui.MenuItem("New", "Ctrl+N")) { _context.LevelFileService.NewLevel(_context); }
                if (ImGui.MenuItem("Save", "Ctrl+S")) { _context.LevelFileService.Save(_context); }
                if (ImGui.MenuItem("Open", "Ctrl+O")) { _context.LevelFileService.Load(_context); }

                if (ImGui.MenuItem("Run Map", "Ctrl+R"))
                {
                    var output = _context.LevelFileService.Save(_context);

                    if (output.result.IsOk)
                    {
                        _context.PlayModeService.RunLevel(output.result.Path);
                    }
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }
}
