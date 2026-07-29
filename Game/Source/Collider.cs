using Raylib_cs;
using System.Numerics;

namespace Game;

public class Collider
{
    public event Action<Collider> OnBeginOverlap;
    public event Action<Collider> OnEndOverlap;

    public Entity Parent { get; init; }

    public BoundingBox BoundingBox { get; protected set; }

    public Vector3 CollisionBounds { get; set; }

    public bool IsColliding { get; set; } = false;

    public Color Color { get; private set; } = Color.SkyBlue;

    public Collider(Entity parent)
    {
        Parent = parent;
    }

    public void Update(Transform transform)
    {
        Vector3 halfSize = CollisionBounds * transform.Scale;

        BoundingBox = new BoundingBox
        {
            Min = transform.Position - halfSize,
            Max = transform.Position + halfSize
        };
    }
}
