using Raylib_cs;
using System.Text.Json.Serialization;

namespace Game.Graphics;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(MeshRenderer), "mesh")]
[JsonDerivedType(typeof(BillboardRenderer), "billboard")]
[JsonDerivedType(typeof(AnimatedBillboardRenderer), "anim_billboard")]
public abstract class RenderComponent
{
    public Texture2D Texture { get; set; }

    public Color Tint { get; set; } = Color.White;

    protected RenderComponent()
    {
        
    }

    protected RenderComponent(Texture2D texture)
    {
        Texture = texture;
    }

    public abstract void Render(Camera camera, Transform transform);
    public abstract void DebugRender(Camera camera, Transform transform);
}
