using Game.LevelEditor.Panels;
using Raylib_cs;
using System.Numerics;

namespace Game.LevelEditor;

[HideFromSpawnMenu]
public class EditorCamera : Camera
{
    public float MoveSpeed = 5f;

    private float _pitch, _yaw;

    private Viewport _viewport;

    public void SetEditor(Viewport viewport)
    {
        _viewport = viewport;

        _viewport.ViewportControlChanged += (controlled) =>
        {
            if (controlled)
            {
                Raylib.HideCursor();
            }
            else
            {
                Raylib.ShowCursor();
            }
        };

    }


    public override void Update()
    {
        if (!_viewport.ViewportControlled)
        {
            return;
        }

        if (Raylib.IsKeyDown(KeyboardKey.W))
        {
            Move(GetForwardVector());
        }
        if (Raylib.IsKeyDown(KeyboardKey.S))
        {
            Move(GetBackwardVector());
        }
        if (Raylib.IsKeyDown(KeyboardKey.A))
        {
            Move(GetLeftVector());
        }
        if (Raylib.IsKeyDown(KeyboardKey.D))
        {
            Move(GetRightVector());
        }

        if (Raylib.IsKeyDown(KeyboardKey.E))
        {
            Move(Directions.Up);
        }
        if (Raylib.IsKeyDown(KeyboardKey.Q))
        {
            Move(Directions.Down);
        }

        Vector2 delta = Raylib.GetMouseDelta();
        _yaw -= (delta.X * Sensitivity) * (float)Time.FrameDelta;

        _pitch -= (delta.Y * Sensitivity) * (float)Time.FrameDelta;
        _pitch = Math.Clamp(_pitch, -89.9f * Raylib.DEG2RAD, 89.9f * Raylib.DEG2RAD);

        Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_yaw, 0, 0);
        Transform.Rotation = Quaternion.Normalize(Transform.Rotation * Quaternion.CreateFromYawPitchRoll(0, _pitch, 0));
    }

    private void Move(Vector3 direction)
    {
        Transform.Translate(direction * MoveSpeed * (float)Time.FrameDelta);
    }
}
