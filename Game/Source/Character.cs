using Raylib_cs;
using System.Numerics;

namespace Game;

public class Character : Entity
{
    public Engine Engine { get; set; }

    protected float _moveSpeed = 1.5f;
    public World World;


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

        Vector3 halfSize = CollisionBounds * Transform.Scale;

        BoundingBox sweepCollider = new BoundingBox
        {
            Min = (Transform.Position + direction) - halfSize,
            Max = (Transform.Position + direction) + halfSize
        };

        foreach(var cellData in World.GetCells())
        {
            if (World.IsCollidingWithCell(cellData.cell, sweepCollider))
            {
                return false;
            }
        }

        return true;
    }
}
