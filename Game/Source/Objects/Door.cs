using Raylib_cs;
using System.Numerics;

namespace Game.Objects;

public class Door : Entity
{
    public bool IsLocked { get; set; } = false;

    public Door()
    {
        Model model = Raylib.LoadModelFromMesh(Raylib.GenMeshPlane(1.0f, 1.0f, 1, 1));
        model.Transform = Matrix4x4.CreateRotationX(Raylib.DEG2RAD * -90f);

        Collider = new Collider(this)
        {
            CollisionBounds = Vector3.One * 0.5f
        };

        _renderer = new MeshRenderer
        {
            Texture = AssetManager.Load<Texture2D>("Assets/Textures/Default.png"),
            Model = model
        };
    }
}
