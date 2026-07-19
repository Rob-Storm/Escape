using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Escape;

public class Camera : Entity
{
    private float speed = 0.02f;

    private float yaw = 0f;
    private float pitch = 0f;

    private const float MOUSE_SENSITIVITY = 0.001f;

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if(Keyboard.GetState().IsKeyDown(Keys.W))
        {
            Transform.Translate(GetForwardVector() * speed);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            Transform.Translate(GetBackwardVector() * speed);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            Transform.Translate(GetLeftVector() * speed);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            Transform.Translate(GetRightVector() * speed);
        }

        MouseState mouse = Mouse.GetState();

        int centerX = _device.Viewport.Width / 2;
        int centerY = _device.Viewport.Height / 2;

        float dx = mouse.X - 500;
        float dy = mouse.Y - 500;

        yaw -= dx * MOUSE_SENSITIVITY;
        pitch -= dy * MOUSE_SENSITIVITY;

        Transform.Rotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0f);

        Mouse.SetPosition(500, 500);
    }

    public Matrix GetViewMatrix()
    {
        return Matrix.CreateLookAt(Transform.Position, Transform.Position + GetForwardVector(), GetUpVector());
    }

    public Matrix GetProjectionMatrix()
    {
        return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90f), 800f / 480f, 0.01f, 1000f);
    }
}
