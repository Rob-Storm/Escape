using System.Numerics;

namespace Game.Objects;

/// <summary>
/// An object with a visual appearance that does nothing
/// </summary>
public class Decoration : Entity
{
    public Decoration()
    {
        Collider = new Collider(this)
        {
            CollisionBounds = Vector3.Zero,
            Solid = false
        };

        Renderer = new BillboardRenderer
        {

        };
    }
}
