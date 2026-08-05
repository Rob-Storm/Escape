using Raylib_cs;
using System.Numerics;

namespace Game;

public class Collider
{
    public event Action<Collider>? OnBeginOverlap;
    public event Action<Collider>? OnEndOverlap;

    public Entity Parent { get; init; }

    public BoundingBox BoundingBox { get; protected set; }

    public Vector3 CollisionBounds { get; set; } = Vector3.Zero;

    // Non-solid colliders will still trigger overlap events, but will not block movement.

    // Possible refactor to use collision channels like Unreal Engine, but that is for
    // a next iteration of the engine
    public bool Solid { get; set; } = true;

    public bool IsColliding { get; protected set; } = false;

    public Color Color { get; private set; } = Color.SkyBlue;

    private HashSet<Collider> _overlappingColliders;


    public Collider(Entity parent)
    {
        Parent = parent;
        _overlappingColliders = new HashSet<Collider>();
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

    public void SetIsColliding(bool colliding, Collider collider)
    {

        if (!IsColliding && colliding && !_overlappingColliders.Contains(collider))
        {
            OnBeginOverlap?.Invoke(collider);

            _overlappingColliders.Add(collider);

            Debug.Log($"{Parent.Name} Begin overlap {collider.Parent}");
        }

        if (IsColliding && !colliding && _overlappingColliders.Contains(collider))
        {
            OnEndOverlap?.Invoke(collider);

            _overlappingColliders.Remove(collider);

            Debug.Log($"{Parent.Name} End Overlap {collider.Parent}");
        }

        IsColliding = colliding;
    }
}


