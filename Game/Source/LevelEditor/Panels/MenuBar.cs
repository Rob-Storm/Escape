using ImGuiNET;

namespace Game.LevelEditor.Panels;

public class MenuBar : EditorPanel
{
    public MenuBar(Editor editor) : base(editor)
    {
    }

    public override void Draw()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New", "Ctrl+N")) { _editor.NewLevel(); }
                if (ImGui.MenuItem("Save", "Ctrl+S")) { _editor.SaveLevel(); }
                if (ImGui.MenuItem("Open", "Ctrl+O")) { _editor.LoadEditorLevel(); }
                ImGui.EndMenu();
            }

            if (ImGui.MenuItem("Run"))
            {
                var output = _editor.SaveLevel();

                if (output.result.IsOk)
                {
                    _editor.RunLevel(output.path);
                }
            }

            ImGui.EndMainMenuBar();
        }
    }
}
