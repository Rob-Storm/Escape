using Raylib_cs;
using System.Numerics;

namespace Game.Physics;

/// <summary>
/// Basic AABB collider
/// </summary>
public class BoxCollider : Collider
{
    public BoundingBox BoundingBox { get; protected set; }

    public Vector3 CollisionBounds { get; set; } = Vector3.Zero;


    public BoxCollider(Entity? parent = null)
    {
        Parent = parent;
        OverlappingColliders = new HashSet<Collider>();
    }

    public override void Update(Transform transform)
    {
        Vector3 halfSize = CollisionBounds * transform.Scale;

        BoundingBox = new BoundingBox
        {
            Min = transform.Position - halfSize,
            Max = transform.Position + halfSize
        };
    }

    public override void DebugDraw(Transform transform)
    {
        Raylib.DrawBoundingBox(BoundingBox, Color);
    }

    public static BoxCollider FromBoundingBox(BoundingBox box, CollisionChannel channel)
    {
        return new BoxCollider() { CollisionBounds = Vector3.Abs(box.Min), Channel = channel };
    }
}
