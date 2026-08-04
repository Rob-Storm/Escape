using ImGuiNET;
using System.Numerics;
using System.Reflection;

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
        bool openLevelSettingsPopup = false;

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

            if(ImGui.BeginMenu("Edit"))
            {
                if (ImGui.BeginMenu("Spawn Entity"))
                {
                    // HACK: copy-pasted from MapGrid.cs and ToolSettings.cs
                    IEnumerable<Type> types = typeof(Entity).Assembly.GetTypes();

                    foreach (Type type in types)
                    {
                        if (type.IsSubclassOf(typeof(Entity)) && Attribute.GetCustomAttribute(type, typeof(HideFromSpawnMenuAttribute)) == null)
                        {

                            if (ImGui.MenuItem(type.Name))
                            {
                                ConstructorInfo ctor = type.GetConstructor(new Type[] { })!;
                                Entity instance = (Entity)ctor.Invoke(new Type[] { });

                                instance.Transform.Position = new Vector3(0, 0, 0);
                                _context.World.EntityList.Add(instance);

                                _context.SelectedObject = instance;
                            }
                        }
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("View"))
            {
                ImGui.Checkbox("Level Settings", ref _context.Layout.ShowLevelSettings);

                ImGui.Checkbox("Editor Preferences", ref _context.Layout.ShowEditorPreferences);

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }

        if (openNewPopup)
        {
            ImGui.OpenPopup("Create New Level");
        }


        ShowNewLevelPopup();
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

}