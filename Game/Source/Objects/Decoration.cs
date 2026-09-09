using Game.Graphics;
using Game.Physics;
using System.Numerics;

namespace Game.Objects;

/// <summary>
/// An object with a visual appearance that does nothing
/// </summary>

public class Decoration : Entity
{
    public Decoration()
    {
        Collider = new BoxCollider(this)
        {
            CollisionBounds = Vector3.Zero,
            Solid = false,
            Channel = CollisionChannel.None
        };

        Renderer = new BillboardRenderer
        {

        };
    }
}
