using Raylib_cs;
using System.Numerics;

namespace Game;

public class BillboardRenderer : RenderComponent
{
    ~BillboardRenderer()
    {
        Raylib.UnloadTexture(Texture);
    }

    public override void Render(Camera camera, Transform transform)
    {
        if (Texture.Id == 0)
        {
            return;
        }

        Rectangle source = new Rectangle(Vector2.Zero, Texture.Dimensions);
        Vector2 size = new Vector2(transform.Scale.X, transform.Scale.Z);
        Vector2 origin = new Vector2(0.5f, 0.5f) * size;

        Raylib.DrawBillboardPro(camera, Texture, source, transform.Position, Vector3.UnitY, size, origin, 0f, Color.White);

        // Todo: fix bug with billboard rotating with camera rotation instead of it's position
    }

    public override void DebugRender(Camera camera, Transform transform)
    {
        
    }
}
