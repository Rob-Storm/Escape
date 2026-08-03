using Game.Objects;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Game;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Character), "character")]
[JsonDerivedType(typeof(Door), "door")]
public class Entity
{
    // An identifier for the editor
    public string Name = "Entity";
    public Transform Transform { get; set; }

    [JsonIgnore]
    public Collider? Collider { get; set; }

    protected RenderComponent? _renderer;

    public bool MarkedForDelete { get; private set; } = false;

    public Entity()
    {
        Transform = new Transform();

        _renderer = new BillboardRenderer
        {
        };

        Transform.Translate(Directions.Up * 0.5f);
    }

    public virtual void Update()
    {
        Collider?.Update(Transform);
    }

    public virtual void Render(Camera camera)
    {
        _renderer?.Render(camera, Transform);
    }

    public virtual void DebugRender(Camera camera)
    {
        _renderer?.DebugRender(camera, Transform);
    }

    public void Destroy() => MarkedForDelete = true;

    public Vector3 GetForwardVector() => Vector3.Transform(Directions.Forward, GetRotationMatrix());
    public Vector3 GetBackwardVector() => Vector3.Transform(Directions.Backward, GetRotationMatrix());
    public Vector3 GetUpVector() => Vector3.Transform(Directions.Up, GetRotationMatrix());
    public Vector3 GetDownVector() => Vector3.Transform(Directions.Down, GetRotationMatrix());
    public Vector3 GetRightVector() => Vector3.Transform(Directions.Left, GetRotationMatrix());
    public Vector3 GetLeftVector() => Vector3.Transform(Directions.Right, GetRotationMatrix());
    public Matrix4x4 GetRotationMatrix() => Matrix4x4.CreateFromQuaternion(Transform.Rotation);
}
