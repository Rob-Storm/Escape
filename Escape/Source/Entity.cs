using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Escape;

public class Entity
{
    public bool MarkedForDelete { get; protected set; } = false;
    public Transform Transform { get; protected set; } = new Transform();
    public Texture2D Texture { get; protected set; }

    public BoundingBox Collider { get; protected set; } = new BoundingBox();
    public Vector3 CollisionBounds = Vector3.One;

    public bool IsColliding { get; protected set; }
    public Color ColliderColor { get; protected set; } = Color.Green;

    protected Matrix _world;
    protected Effect _effect;

    protected BasicEffect _basicEffect;
    protected GraphicsDevice _device;

    private Face _face;

    BillboardRenderer _renderer;

    public virtual void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
    {
        Texture = content.Load<Texture2D>("Textures/ManLow");

        _basicEffect = new BasicEffect(graphicsDevice);
        _device = graphicsDevice;

        _face = new Face(FaceSide.Right, graphicsDevice);

        _renderer = new BillboardRenderer(graphicsDevice);
        _renderer.Texture = Texture;
    }

    public virtual void Update(GameTime gameTime)
    {
        Vector3 halfSize = CollisionBounds * Transform.Scale;

        Collider = new BoundingBox
            (
                Transform.Position - halfSize,
                Transform.Position + halfSize
            );
    }

    public virtual void Draw(GameTime gameTime, Camera camera)
    {
        _renderer.Draw(gameTime, Transform, camera);
    }

    public virtual void HandleCollision(BoundingBox other)
    {
        ContainmentType type = Collider.Contains(other);
        IsColliding = type != ContainmentType.Disjoint;

        switch (type)
        {
            case ContainmentType.Disjoint:
                ColliderColor = Color.Green;
                break;
            case ContainmentType.Intersects:
                ColliderColor = Color.Yellow;
                break;
            case ContainmentType.Contains:
                ColliderColor = Color.Red;
                break;
        }
    }

    public void Destroy()
    {
        MarkedForDelete = true;
    }

    public Vector3 GetForwardVector() => Vector3.Transform(Vector3.Forward, GetRotationMatrix());
    public Vector3 GetBackwardVector() => Vector3.Transform(Vector3.Backward, GetRotationMatrix());
    public Vector3 GetUpVector() => Vector3.Transform(Vector3.Up, GetRotationMatrix());
    public Vector3 GetDownVector() => Vector3.Transform(Vector3.Down, GetRotationMatrix());
    public Vector3 GetRightVector() => Vector3.Transform(Vector3.Right, GetRotationMatrix());
    public Vector3 GetLeftVector() => Vector3.Transform(Vector3.Left, GetRotationMatrix());
    public Matrix GetRotationMatrix() => Matrix.CreateFromQuaternion(Transform.Rotation);
}


public class Transform
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;

    public Transform()
    {
        Position = Vector3.Zero;
        Rotation = Quaternion.Identity;
        Scale = Vector3.One;
    }

    public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }

    public Transform Copy()
    {
        return (Transform)MemberwiseClone();
    }

    public void Translate(Vector3 translation)
    {
        Position += translation;
    }

    public void Rotate(Vector3 rotation)
    {
        Rotation *= Quaternion.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
        Rotation = Quaternion.Normalize(Rotation);
    }

    public override string ToString()
    {
        return $"T[{Position.ToStringTruncated()} | {Rotation.ToEulerAngles().ToStringTruncated()} | {Scale.ToString()}]";
    }

}