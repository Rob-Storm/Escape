using Game.LevelEditor;
using Raylib_cs;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Game.Objects;

public class Character : Entity, IDamageable
{
    public event Action<int>? OnDamaged;
    public event Action? OnKill;

    public int Health { get; set; } = 10;

    public float MoveSpeed { get; set; } = 1.5f;

    public Sound DeathSound { get; set; } = AssetManager.Load<Sound>(@"Assets\Sounds\Die.wav");

    public Texture2D DeadTexture { get; set; } = AssetManager.Load<Texture2D>(@"Assets\Textures\ManDead.png");

    [HideProperty]
    [JsonIgnore]
    public World? _world;

    public Character()
    {
        Collider = new Collider(this)
        {
            CollisionBounds = new Vector3(0.35f, 0.5f, 0.35f),
            Channel = CollisionChannel.Character
        };

        Renderer = new BillboardRenderer
        {
            Texture = AssetManager.Load<Texture2D>(@"Assets\Textures\Man.png")
        };

        _world = World.Instance;
    }

    protected void Move(Vector3 direction)
    {
        Vector3 movement = direction * MoveSpeed * (float)Time.FrameDelta;

        if (CanMove(new Vector3(movement.X, 0, 0)))
        {
            Transform.Position.X += movement.X;
        }

        if (CanMove(new Vector3(0, 0, movement.Z)))
        {
            Transform.Position.Z += movement.Z;
        }
    }

    protected bool CanMove(Vector3 direction)
    {
        Vector3 halfSize = Collider!.CollisionBounds * Transform.Scale;

        BoundingBox sweepCollider = new BoundingBox
        {
            Min = Transform.Position + direction - halfSize,
            Max = Transform.Position + direction + halfSize
        };

        foreach (var cellData in _world!.GetCells())
        {
            if (_world.IsCollidingWithCell(cellData.cell, sweepCollider))
            {
                return false;
            }
        }

        foreach (Entity entity in _world!.EntityList)
        {
            if (entity == this || !entity.Collider!.Solid)
            {
                continue;
            }

            if (_world.IsCollidingWithEntity(entity, sweepCollider))
            {
                return false;
            }
        }

        return true;
    }

    public void Kill()
    {
        OnKill?.Invoke();
        GameplayStatics.PlaySoundAtLocation(DeathSound, Transform.Position, 1f);
        Destroy();
    }

    public void Damage(int amount)
    {
        Health -= amount;

        if (Health <= 0)
        {
            Kill();
        }

        OnDamaged?.Invoke(Health);
    }
}
