using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;
using System.Reflection;

namespace Game.LevelEditor.Panels;


public class PropertyInspector : EditorPanel
{
    private delegate void OnPropertyDrawSignature(ref object propertyValue, string propertyName);

    private Dictionary<Type, OnPropertyDrawSignature> _typeFactory;

    public PropertyInspector(EditorContext context) : base(context)
    {
        _typeFactory = new Dictionary<Type, OnPropertyDrawSignature>
        {
            { typeof(string), DrawString},
            { typeof(int), DrawInt},
            { typeof(float), DrawFloat},
            { typeof(Vector2), DrawVector2},
            { typeof(AssetTypeInfo), DrawAsset}
        };

    }

    public override void Draw()
    {
        ImGui.Begin("Properties");

        DrawProperties(_context.SelectedObject);

        ImGui.End();
    }

    private void DrawProperties(object inObject)
    {
        if (inObject == null)
        {
            ImGui.TextDisabled("Select an object to view properties");

            return;
        }

        foreach (FieldInfo field in inObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            if (_typeFactory.TryGetValue(field.FieldType, out var drawFunction))
            {
                object value = field.GetValue(inObject);
                drawFunction(ref value, field.Name);
                field.SetValue(inObject, value);
            }
            else if(AssetManager.IsRegisteredAssetType(field.FieldType))
            {
                object value = field.GetValue(inObject);
                DrawAsset(ref value, field.Name);
                field.SetValue(inObject, value);
            }
        }

        foreach (PropertyInfo property in inObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead || !property.CanWrite)
                continue;

            if (_typeFactory.TryGetValue(property.PropertyType, out var drawFunction))
            {
                object value = property.GetValue(inObject);
                drawFunction(ref value, property.Name);
                property.SetValue(inObject, value);
            }
            else if (AssetManager.IsRegisteredAssetType(property.PropertyType))
            {
                object value = property.GetValue(inObject);
                DrawAsset(ref value, property.Name);
                property.SetValue(inObject, value);
            }
        }
    }

    private void DrawString(ref object propertyValue, string propertyName)
    {
        string property = (string)propertyValue;

        ImGui.InputText(propertyName, ref property, 32);

        propertyValue = property;
    }

    private void DrawInt(ref object propertyValue, string propertyName)
    {
        int property = (int)propertyValue;

        ImGui.InputInt(propertyName, ref property, 1);

        propertyValue = property;
    }

    private void DrawFloat(ref object propertyValue, string propertyName)
    {
        float property = (float)propertyValue;

        ImGui.InputFloat(propertyName, ref property, 1);

        propertyValue = property;
    }

    private void DrawVector2(ref object propertyValue, string propertyName)
    {
        Vector2 property = (Vector2)propertyValue;

        ImGui.InputFloat2(propertyName, ref property);

        propertyValue = property;
    }

    private void DrawAsset(ref object propertyValue, string propertyName)
    {
        AssetTypeInfo info = AssetManager.GetAssetTypeInfo(propertyValue.GetType());

        ImGui.PushID(propertyName);

        if(info.DrawPreview(propertyValue))
        {
            // do stuff
        }

        if (ImGui.BeginDragDropTarget() && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        {
            ImGuiPayloadPtr  payload = ImGui.AcceptDragDropPayload("texture_path");

            propertyValue = AssetManager.Load(_context.DraggedAssetPath, info.Type);

            ImGui.EndDragDropTarget();
        }

        ImGui.PopID();

        ImGui.SameLine();

        ImGui.Text(propertyName);
    }
}
