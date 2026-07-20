using Game.LevelEditor;
using Raylib_cs;
using System.Numerics;

namespace Game;

public class EditorCamera : Camera
{
    private float _moveSpeed = 1f;

    private float _pitch, _yaw;

    private Editor _editor;

    public void SetEditor(Editor editor)
    {
        _editor = editor;

        _editor.ViewportControlChanged += (controlled) =>
        {
            if(controlled)
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
        if (!_editor.ViewportControlled)
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

        if (Raylib.IsKeyDown(KeyboardKey.Space))
        {
            Move(Directions.Up);
        }
        if (Raylib.IsKeyDown(KeyboardKey.LeftControl))
        {
            Move(Directions.Down);
        }

        Vector2 delta = Raylib.GetMouseDelta();
        _yaw -= delta.X * Sensitivity;

        _pitch -= delta.Y * Sensitivity;
        _pitch = Math.Clamp(_pitch, -89.9f * Raylib.DEG2RAD, 89.9f * Raylib.DEG2RAD);

        Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_yaw, 0, 0);
        Transform.Rotation = Quaternion.Normalize(Transform.Rotation * Quaternion.CreateFromYawPitchRoll(0, _pitch, 0));

    }

    private void Move(Vector3 direction)
    {
        Transform.Translate(direction * _moveSpeed * (float)Time.FrameDelta);
    }
}
