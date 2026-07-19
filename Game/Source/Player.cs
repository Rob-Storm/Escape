using Raylib_cs;
using System.Numerics;

namespace Game;

public class Player : Character
{
    public Camera Camera { get; private set; }
    private Vector3 _cameraOffset = Directions.Up * 0.375f;

    private float _pitch, _yaw;
    public Player()
    {
        Camera = new Camera();

        CollisionBounds = new Vector3(0.125f, 0.5f, 0.125f);
    }

    public override void Update()
    {
        base.Update();

        Vector3 moveVector = Vector3.Zero;

        if (Raylib.IsKeyDown(KeyboardKey.W))
        {
            moveVector += GetForwardVector();
        }
        if (Raylib.IsKeyDown(KeyboardKey.S))
        {
            moveVector += GetBackwardVector();
        }
        if (Raylib.IsKeyDown(KeyboardKey.A))
        {
            moveVector += GetLeftVector();
        }
        if (Raylib.IsKeyDown(KeyboardKey.D))
        {
            moveVector += GetRightVector();
        }

        if(moveVector != Vector3.Zero)
        {
            moveVector = Vector3.Normalize(moveVector);
            Move(moveVector);
        }

        Vector2 delta = Raylib.GetMouseDelta();
        _yaw -= delta.X * Camera.Sensitivity;

        _pitch -= delta.Y * Camera.Sensitivity;
        _pitch = Math.Clamp(_pitch, -89.9f * Raylib.DEG2RAD, 89.9f * Raylib.DEG2RAD);

        if(Raylib.IsCursorHidden())
        {
            Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_yaw, 0, 0);
            Camera.Transform.Rotation = Quaternion.Normalize(Transform.Rotation * Quaternion.CreateFromYawPitchRoll(0, _pitch, 0));
        }

        Camera.Transform.Position = Transform.Position + _cameraOffset;
        Camera.Update();
    }

}
