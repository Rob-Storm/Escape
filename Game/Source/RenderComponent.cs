using Raylib_cs;
using System.Text.Json.Serialization;

namespace Game;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(MeshRenderer), "mesh")]
[JsonDerivedType(typeof(BillboardRenderer), "billboard")]
public abstract class RenderComponent
{
    public Texture2D Texture { get; set; }
    public abstract void Render(Camera camera, Transform transform);
    public abstract void DebugRender(Camera camera, Transform transform);
}
