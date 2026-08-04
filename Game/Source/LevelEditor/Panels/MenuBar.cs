using ImGuiNET;

namespace Game.LevelEditor.Panels;

public class MenuBar : EditorPanel
{
    private string _levelName = "New Level";
    private int _levelSizeX = 25;
    private int _levelSizeY = 25;

    private Editor _editor;

    private int _startRotationIndex = 0;
    private Dictionary<string, float> _directions;

    public MenuBar(EditorContext context) : base(context)
    {
        _editor = (Editor)context.World;

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
        bool openNewPopup = false;
        bool openLevelSettingsPopup = false;
        bool openEditorPreferencesPopup = false;

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

            if (ImGui.BeginMenu("Edit"))
            {
                if (ImGui.MenuItem("Level Settings"))
                {
                    openLevelSettingsPopup = true;
                }

                if (ImGui.MenuItem("Editor Preferences"))
                {
                    openEditorPreferencesPopup = true;
                }

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        if (openNewPopup)
        {
            ImGui.OpenPopup("Create New Level");
        }

        if (openLevelSettingsPopup)
        {
            ImGui.OpenPopup("Level Settings");
        }

        if (openEditorPreferencesPopup)
        {
            ImGui.OpenPopup("Editor Preferences");
        }

        ShowNewLevelPopup();
        ShowLevelSettingsPopup();
        ShowEditorPreferencesPopup();
    }

    private void ShowNewLevelPopup()
    {
        if (ImGui.BeginPopupModal("Create New Level", ImGuiWindowFlags.Popup | ImGuiWindowFlags.AlwaysAutoResize))
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

    private void ShowLevelSettingsPopup()
    {
        if (ImGui.BeginPopupModal("Level Settings", ImGuiWindowFlags.Popup | ImGuiWindowFlags.AlwaysAutoResize))
        {
            ImGui.InputText("Level Name", ref _context.LevelName, 16);
            ImGui.InputFloat2("Start Position", ref _context.PlayerStart);
            ImGui.Combo("Start Rotation", ref _startRotationIndex, _directions.Keys.ToArray(), 4);

            _context.StartRotation = _directions.Values.ToArray()[_startRotationIndex];

            ImGui.BeginDisabled();
            ImGui.DragInt("LevelSizeX", ref _context.World.SizeX, 1, 1, 25);
            ImGui.DragInt("LevelSizeY", ref _context.World.SizeY, 1, 1, 25);
            ImGui.EndDisabled();

            if (ImGui.Button("Close"))
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }
    }

    private void ShowEditorPreferencesPopup()
    {
        if (ImGui.BeginPopupModal("Editor Preferences", ImGuiWindowFlags.Popup | ImGuiWindowFlags.AlwaysAutoResize))
        {
            float mouseSens = _context.Camera.Sensitivity;

            ImGui.InputFloat("Camera Speed", ref _context.Camera.MoveSpeed);
            ImGui.DragFloat("Mouse Sensitivity", ref mouseSens, 0.5f, 1f, 10.0f);

            _context.Camera.Sensitivity = mouseSens;

            if (ImGui.Button("Close"))
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }
    }
}