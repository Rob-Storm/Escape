using Raylib_cs;
using System.Numerics;

namespace Game.Physics;

public abstract class Collider
{
    public event Action<Collider>? OnBeginOverlap;
    public event Action<Collider>? OnEndOverlap;

    public Entity? Parent { get; init; }

    public CollisionChannel Channel { get; set; }

    // Non-solid colliders will still trigger overlap events, but will not block movement.
    public bool Solid { get; set; } = true;

    public bool IsColliding { get; protected set; } = false;

    public Color Color { get; protected set; } = Color.SkyBlue;

    public HashSet<Collider> OverlappingColliders { get; protected set; }


    public Collider(Entity? parent = null)
    {
        Parent = parent;
        OverlappingColliders = new HashSet<Collider>();
    }

    public abstract void Update(Transform transform);

    public virtual void SetIsColliding(bool colliding, Collider collider)
    {
        if (colliding)
        {
            if (OverlappingColliders.Add(collider))
            {
                OnBeginOverlap?.Invoke(collider);

                Debug.Log($"{Parent.Name} Begin overlap {collider.Parent}", channel: LogChannel.Physics);
            }
        }
        else
        {
            if (OverlappingColliders.Remove(collider))
            {
                OnEndOverlap?.Invoke(collider);

                Debug.Log($"{Parent.Name} End overlap {collider.Parent}", channel: LogChannel.Physics);
            }
        }

        IsColliding = OverlappingColliders.Count > 0;
    }

    public abstract void DebugDraw(Transform transform);
}

[Flags]
public enum CollisionChannel
{
    None = 0,
    All = ~None,

    // Cells, terrain, etc.
    WorldStatic = 1 << 0,

    // Non-static objects that aren't character such as an explosive barrel, window, or collectable
    WorldDynamic = 1 << 1,

    Character = 1 << 2,
}