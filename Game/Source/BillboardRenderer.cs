using Raylib_cs;
using System.Numerics;

namespace Game;

public class BillboardRenderer
{
    public Texture2D Texture { get; set; }

    ~BillboardRenderer()
    {
        Raylib.UnloadTexture(Texture);
    }

    public void Render(Camera camera, Vector3 position)
    {
        if(Texture.Id == 0)
        {
            return;
        }

        Rectangle source = new Rectangle(Vector2.Zero, Texture.Dimensions);
        Vector2 size = Vector2.One;
        Vector2 origin = new Vector2(0.5f, 0.5f);

        Raylib.DrawBillboardPro(camera, Texture, source, position, Vector3.UnitY, size, origin, 0f, Color.White);

        // Todo: fix bug with billboard rotating with camera rotation instead of it's position
    }
}
