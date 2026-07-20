using Raylib_cs;
using System.Numerics;

namespace Game;

public class Camera : Entity
{
    public float FieldOfView { get; protected set; } = 80f;

    public float Sensitivity { get; protected set; } = 0.002f;

    protected float _aspectRatio;

    private Sound _sound;

    public Camera()
    {
        Raylib.HideCursor();
        Raylib.DisableCursor();

        _sound = Raylib.LoadSound("Assets/Sounds/Toggle.wav");
    }

    public override void Update()
    {
        _aspectRatio = (float)(Raylib.GetScreenWidth() / Raylib.GetScreenHeight());

        if (Raylib.IsKeyPressed(KeyboardKey.F1))
        {
            if (Raylib.IsCursorHidden())
            {
                Raylib.EnableCursor();
                Raylib.ShowCursor();
            }
            else
            {
                Raylib.HideCursor();
                Raylib.DisableCursor();
            }

            Raylib.PlaySound(_sound);
        }
    }

    public void SetAspectRatio(float width, float height) => _aspectRatio = width / height;
    public void SetAspectRatio(Vector2 size) => SetAspectRatio(size.X, size.Y);

    public Matrix4x4 GetViewMatrix() => Matrix4x4.CreateLookAt(Transform.Position, Transform.Position + GetForwardVector(), GetUpVector());
    public Matrix4x4 GetProjectionMatrix() => Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView, _aspectRatio, 0.01f, 5000f);

    public static implicit operator Camera3D(Camera camera)
    {
        return new Camera3D
        {
            Position = camera.Transform.Position,
            Target = camera.Transform.Position + camera.GetForwardVector(),
            Up = camera.GetUpVector(),
            FovY = camera.FieldOfView,
            Projection = CameraProjection.Perspective
        };
    }
}
