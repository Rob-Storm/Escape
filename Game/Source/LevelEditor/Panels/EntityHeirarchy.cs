using ImGuiNET;
using System.Numerics;
using System.Reflection;

namespace Game.LevelEditor.Panels;

// technically not a heirarchy since this engine doesn't have parent-child relationships
public class EntityHeirarchy : EditorPanel
{
    private int entitySelectedIndex = -1;
    public EntityHeirarchy(EditorContext context) : base(context)
    {
    }

    public override void Draw()
    {
        ImGui.Begin("Heirarchy");

        ImGui.PushStyleColor(ImGuiCol.ChildBg, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.Border, Vector4.Zero);

        ImGui.BeginChild("##entities", ImGui.GetContentRegionAvail(), ImGuiChildFlags.None);

        for (int i = 0; i < _context.World.EntityList.Count; i++)
        {
            ImGui.PushID(i);

            bool isSelected = entitySelectedIndex == i;

            if (ImGui.Selectable(_context.World.EntityList[i].Name, isSelected))
            {
                entitySelectedIndex = i;
                _context.SelectedObject = _context.World.EntityList[i];

            }

            if (ImGui.BeginPopupContextItem())
            {
                if (ImGui.MenuItem("Delete Entity"))
                {
                    _context.SelectedObject = null;
                    _context.World.EntityList.RemoveAt(i);
                    i--;
                }

                ImGui.EndPopup();
            }

            ImGui.PopID();
        }

        if (ImGui.BeginPopupContextWindow("SpawnContent", ImGuiPopupFlags.NoOpenOverItems | ImGuiPopupFlags.MouseButtonRight))
        {
            if (ImGui.BeginMenu("Spawn Entity"))
            {
                // HACK: copy-pasted from MapGrid.cs and ToolSettings.cs
                IEnumerable<Type> types = typeof(Entity).Assembly.GetTypes();

                foreach (Type type in types)
                {
                    if (type.IsSubclassOf(typeof(Entity)) && Attribute.GetCustomAttribute(type, typeof(HideFromSpawnMenuAttribute)) == null)
                    {

                        if(ImGui.MenuItem(type.Name))
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

            ImGui.EndPopup();
        }

        ImGui.EndChild();

        ImGui.PopStyleColor(2);

        ImGui.End();
    }
}
