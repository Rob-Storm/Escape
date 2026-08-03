using Raylib_cs;
using System.Numerics;

namespace Game.Objects;

public class Door : Entity
{
    public bool IsLocked { get; set; } = true;
    private Model model;

    public Door()
    {
        model = Raylib.LoadModelFromMesh(Raylib.GenMeshCube(1f, 1f, 1f));

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

    public override void Update()
    {
        base.Update();
    }
}
