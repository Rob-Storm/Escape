using Game.LevelEditor;
using Raylib_cs;
using System.Numerics;

namespace Game.Objects;

[HideFromSpawnMenu]
public class Character : Entity, IDamageable
{
    public event Action<int>? OnDamaged;
    public event Action? OnKill;

    public int Health { get; protected set; }

    public Engine? Engine { get; set; }

    protected float _moveSpeed = 1.5f;
    public World? World;

    public Character()
    {
        Collider = new Collider(this);
    }

    protected void Move(Vector3 direction)
    {
        Vector3 movement = direction * _moveSpeed * (float)Time.FrameDelta;

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

        foreach (var cellData in World!.GetCells())
        {
            if (World.IsCollidingWithCell(cellData.cell, sweepCollider))
            {
                return false;
            }
        }

        foreach (Entity entity in World!.EntityList)
        {
            if (entity == this || !entity.Collider!.Solid)
            {
                continue;
            }

            if (World.IsCollidingWithEntity(entity, sweepCollider))
            {
                return false;
            }
        }

        return true;
    }

    public void Kill()
    {
        OnKill?.Invoke();
    }

    public void Damage(int amount)
    {
        Health -= amount;

        if(Health <= 0)
        {
            Kill();
        }

        OnDamaged?.Invoke(Health);
    }
}
