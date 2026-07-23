using System.Numerics;

namespace Game;

public class AssetTypeInfo
{
    public Type Type { get; init; }
    public Func<string, object> Loader { get; init; }
    public string FallbackPath { get; init; }
    public string DisplayName { get; init; }
    public Vector4 Color { get; init; }
    public string DragDropPayload { get; init; }
    public string[] Extensions { get; init; }
    public Func<object, bool>? DrawPreview { get; init; }
    public Action<object>? DrawInspector { get; init; }
}