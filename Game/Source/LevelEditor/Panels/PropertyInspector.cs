using ImGuiNET;
using Raylib_cs;
using System.Numerics;
using System.Reflection;

namespace Game.LevelEditor.Panels;

public class PropertyInspector : EditorPanel
{
    private delegate void OnPropertyDrawSignature(object owner, ref object propertyValue, string propertyName);

    private Dictionary<Type, OnPropertyDrawSignature> _typeFactory;

    private readonly Dictionary<(object Owner, string PropertyName), Vector3> _cachedEuler = new Dictionary<(object Owner, string PropertyName), Vector3>();

    public PropertyInspector(EditorContext context) : base(context)
    {
        _typeFactory = new Dictionary<Type, OnPropertyDrawSignature>
        {
            { typeof(bool), DrawBool},
            { typeof(string), DrawString},
            { typeof(int), DrawInt},
            { typeof(float), DrawFloat},
            { typeof(Vector2), DrawVector2},
            { typeof(Vector3), DrawVector3},
            { typeof(Quaternion), DrawQuaternion},
            { typeof(AssetTypeInfo), DrawAsset}

            // enum and enum flags are checked and handled explicitly
            // complex objects are likewise handled explicitly
        };

    }

    public override void Draw()
    {
        ImGui.Begin("Properties");

        if(!_context.SelectedAnything)
        {
            ImGui.TextDisabled("Select an object or asset to view properties");
            ImGui.End();

            return;
        }

        if (_context.SelectedObject != null)
        {
            DrawProperties(_context.SelectedObject);
        }

        if(_context.SelectedAsset != null)
        {
            AssetTypeInfo info = AssetManager.GetAssetTypeInfo(_context.SelectedAsset.GetType());

            ImGui.PushID(info.DisplayName);

            ImGui.Text(AssetManager.GetPath(_context.SelectedAsset));
            ImGui.TextDisabled(info.Type.ToString());

            info.DrawInspector!(_context.SelectedAsset);

            ImGui.PopID();
        }

        ImGui.End();
    }

    private void DrawProperties(object inObject)
    {
        foreach (FieldInfo field in inObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            if(Attribute.GetCustomAttribute(field, typeof(HidePropertyAttribute)) != null)
            {
                continue;
            }

            object? value = field.GetValue(inObject);
            DrawProperty(inObject, ref value, field.Name, field.FieldType);
            field.SetValue(inObject, value);
        }

        foreach (PropertyInfo property in inObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead)
            {
                continue;
            }

            if (Attribute.GetCustomAttribute(property, typeof(HidePropertyAttribute)) != null)
            {
                continue;
            }

            if(!property.CanWrite)
            {
                ImGui.BeginDisabled();
            }

            object? value = property.GetValue(inObject);

            DrawProperty(inObject, ref value, property.Name, property.PropertyType);
            property.SetValue(inObject, value);

            if(!property.CanWrite)
            {
                ImGui.EndDisabled();
            }
        }
    }

    #region Draw Methods
    private void DrawProperty(object owner, ref object property, string name, Type type)
    {

        if (type.IsEnum)
        {
            if (type.IsDefined(typeof(FlagsAttribute), false))
            {
                DrawFlags(owner, ref property, name);
            }
            else
            {
                DrawEnum(owner, ref property, name);
            }

            return;
        }

        if (_typeFactory.TryGetValue(type, out var drawFunction))
        {
            drawFunction(owner, ref property, name);

            return;
        }

        if (AssetManager.IsRegisteredAssetType(type))
        {
            DrawAsset(owner, ref property, name);

            return;
        }

        // complex object (i.e entity subclass)

        if(property != null)
        {
           ImGui.PushID($"##{name}");

           if(ImGui.TreeNode(name))
           {
                DrawProperties(property);
                ImGui.TreePop();
           }

            ImGui.PopID();

            return;
        }
    }

    private void DrawBool(object owner, ref object propertyValue, string propertyName)
    {
        bool property = (bool)propertyValue;

        ImGui.Checkbox(propertyName, ref property);

        propertyValue = property;
    }

    private void DrawString(object owner, ref object propertyValue, string propertyName)
    {
        string property = (string)propertyValue;

        ImGui.InputText(propertyName, ref property, 32);

        propertyValue = property;
    }

    private void DrawInt(object owner, ref object propertyValue, string propertyName)
    {
        int property = (int)propertyValue;

        ImGui.InputInt(propertyName, ref property, 1);

        propertyValue = property;
    }

    private void DrawFloat(object owner, ref object propertyValue, string propertyName)
    {
        float property = (float)propertyValue;

        ImGui.InputFloat(propertyName, ref property, 1);

        propertyValue = property;
    }

    private void DrawEnum(object owner, ref object propertyValue, string propertyName)
    {
        Type enumType = propertyValue.GetType();

        string[] names = Enum.GetNames(enumType);
        Array values = Enum.GetValues(enumType);

        int currentIndex = Array.IndexOf(values, propertyValue);

        if (ImGui.Combo(propertyName, ref currentIndex, names, names.Length))
        {
            propertyValue = values.GetValue(currentIndex)!;
        }

    }

    private void DrawFlags(object owner, ref object propertyValue, string propertyName)
    {
        Type enumType = propertyValue.GetType();

        ulong current = Convert.ToUInt64(propertyValue);

        ImGui.Text(propertyName);
        ImGui.Indent();

        foreach(Enum value in Enum.GetValues(enumType))
        {
            ulong flag = Convert.ToUInt64(value);

            if(flag == 0)
            {
                continue;
            }

            bool enabled = (current & flag) == flag;

            string name = Enum.GetName(enumType, value)!;

            if(ImGui.Checkbox(name, ref enabled))
            {
                if(enabled)
                {
                    current |= flag;
                }
                else
                {
                    current &= ~flag;
                }
            }
        }

        ImGui.Unindent();

        propertyValue = Enum.ToObject(enumType, current);
    }

    private void DrawVector2(object owner, ref object propertyValue, string propertyName)
    {
        Vector2 property = (Vector2)propertyValue;

        ImGui.InputFloat2(propertyName, ref property);

        propertyValue = property;
    }

    private void DrawVector3(object owner, ref object propertyValue, string propertyName)
    {
        Vector3 property = (Vector3)propertyValue;

        ImGui.InputFloat3(propertyName, ref property);

        propertyValue = property;
    }

    private void DrawQuaternion(object owner, ref object propertyValue, string propertyName)
    {
        Quaternion rotation = (Quaternion)propertyValue;

        var key = (owner, propertyName);

        if (!_cachedEuler.TryGetValue(key, out Vector3 euler))
        {
            euler = rotation.ToEulerAngles();
            _cachedEuler[key] = euler;
        }

        if (ImGui.InputFloat3(propertyName, ref euler))
        {
            _cachedEuler[key] = euler;

            propertyValue = Quaternion.Normalize(Quaternion.CreateFromYawPitchRoll(euler.X * Raylib.DEG2RAD, euler.Y * Raylib.DEG2RAD, euler.Z * Raylib.DEG2RAD));
        }
    }

    private void DrawAsset(object owner, ref object propertyValue, string propertyName)
    {
        AssetTypeInfo info = AssetManager.GetAssetTypeInfo(propertyValue.GetType());


        ImGui.PushID(propertyName);

        if(info.DrawPreview!(propertyValue))
        {
            // do stuff
        }

        if (ImGui.BeginDragDropTarget() && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        {
            AssetTypeInfo? dragInfo = AssetManager.GetAssetTypeInfo(_context.DraggedAssetPath!);

            ImGuiPayloadPtr  payload = ImGui.AcceptDragDropPayload("asset_path");

            if(info.Type == dragInfo!.Type)
            {
                propertyValue = AssetManager.Load(_context.DraggedAssetPath!, info.Type);
            }

            ImGui.EndDragDropTarget();
        }

        ImGui.PopID();

        ImGui.SameLine();

        ImGui.Text(propertyName);
    }
    #endregion
}