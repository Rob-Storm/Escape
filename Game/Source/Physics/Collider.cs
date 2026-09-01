using Game.LevelEditor;
using Game.Objects;
using Raylib_cs;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Game.Physics;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(BoxCollider), "collider_box")]
[JsonDerivedType(typeof(CellCollider), "collider_cell")]
public abstract class Collider
{
    public event Action<Collider>? OnBeginOverlap;

    public event Action<Collider>? OnEndOverlap;

    [HideProperty]
    public Entity? Parent { get; init; }

    public CollisionChannel Channel { get; set; }

    // Non-solid colliders will still trigger overlap events, but will not block movement.
    public bool Solid { get; set; } = true;

    [HideProperty]
    public bool IsColliding { get; protected set; } = false;

    public Color Color { get; protected set; } = Color.SkyBlue;

    [HideProperty]
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

    public abstract void DebugDraw();
}

[Flags]
public enum CollisionChannel
{
    None = 0,

    // Cells, terrain, etc.
    WorldStatic = 1 << 0,

    // Non-static objects that aren't character such as an explosive barrel, window, or collectable
    WorldDynamic = 1 << 1,

    Character = 1 << 2,
}