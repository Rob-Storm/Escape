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

    public Vector3 LocalPosition { get; protected set; } = Vector3.Zero;

    public BoxCollider(Entity? parent = null)
    {
        Parent = parent;
        OverlappingColliders = new HashSet<Collider>();
    }

    public override void Update(Transform transform)
    {
        Vector3 center = transform.Position + LocalPosition;
        Vector3 halfSize = CollisionBounds * transform.Scale;

        BoundingBox = new BoundingBox
        {
            Min = center - halfSize,
            Max = center + halfSize
        };
    }

    public override void DebugDraw()
    {
        Raylib.DrawBoundingBox(BoundingBox, Color.Green);
    }

    public static BoxCollider FromBoundingBox(BoundingBox box, CollisionChannel channel)
    {
        Vector3 center = (box.Min + box.Max) * 0.5f;
        Vector3 halfExtents = (box.Max - box.Min) * 0.5f;

        return new BoxCollider() { LocalPosition = center, CollisionBounds = halfExtents, Channel = channel, BoundingBox = box };
    }
}
