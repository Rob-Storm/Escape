using ImGuiNET;

namespace Game.LevelEditor.Panels;

public class MenuBar : EditorPanel
{
    private string _levelName = "New Level";
    private int _levelSizeX = 25;
    private int _levelSizeY = 25;

    private Editor _editor;
    public MenuBar(EditorContext context) : base(context)
    {
        _editor = (Editor)context.World;
    }

    public override void Draw()
    {
        bool openNewPopup = false;

        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New", "Ctrl+N")) 
                {
                    openNewPopup = true;    
                }

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

        if(openNewPopup)
        {
            ImGui.OpenPopup("Create New Level");
        }

        ShowPopup();
    }

    private void ShowPopup()
    {
        if (ImGui.BeginPopupModal("Create New Level", ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.InputText("Level Name", ref _levelName, 16);
            ImGui.InputInt("Level Size X", ref _levelSizeX, step: 1);
            ImGui.InputInt("Level Size Y", ref _levelSizeY);

            if (ImGui.Button("Create"))
            {
                _context.LevelFileService.NewLevel(_context, _levelName, _levelSizeX, _levelSizeY); 
                ImGui.CloseCurrentPopup();
            }

            ImGui.SameLine();

            if (ImGui.Button("Cancel"))
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }

        _levelSizeX = Math.Clamp(_levelSizeX, 1, 150);
        _levelSizeY = Math.Clamp(_levelSizeY, 1, 150);
    }
}