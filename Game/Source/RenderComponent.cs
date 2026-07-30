using Raylib_cs;
using System.Numerics;

namespace Game;

public abstract class RenderComponent
{
    public Texture2D Texture { get; set; }
    public abstract void Render(Camera camera, Transform transform);
}
