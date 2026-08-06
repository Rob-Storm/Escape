using Raylib_cs;
using System.Numerics;

namespace Game;

public class BillboardRenderer : RenderComponent
{

    public bool Bounce { get; set; }

    private float _bounceAmount = 0f;
    private float _offset = 0f;

    public override void Render(Camera camera, Transform transform)
    {
        // A texture ID of 0 means there is no texture loaded
        if (Texture.Id == 0)
        {
            return;
        }

        if(Bounce && !Engine.IsEditor)
        {
            _bounceAmount += 1f * (float)Time.FrameDelta;
            _offset = MathF.Sin(_bounceAmount) * 0.1f;
        }

        Rectangle source = new Rectangle(Vector2.Zero, Texture.Dimensions);
        Vector2 size = new Vector2(transform.Scale.X, transform.Scale.Z);
        Vector2 origin = new Vector2(0.5f, 0.5f) * size;

        Vector3 finalPosition = transform.Position + new Vector3(0, _offset, 0);

        Raylib.DrawBillboardPro(camera, Texture, source, finalPosition, Vector3.UnitY, size, origin, 0f, Color.White);

        // Todo: fix bug with billboard rotating with camera rotation instead of it's position
    }

    public override void DebugRender(Camera camera, Transform transform)
    {

    }
}
