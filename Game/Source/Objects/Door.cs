using Game.LevelEditor;
using Raylib_cs;
using System.Numerics;

namespace Game.Objects;

public class Door : Entity, IInteractable
{
    [ToolTip("Used by DoorKey to unlock")]
    public int ID = 0;
    public Sound UseSound;
    public Sound LockSound;

    private Model model;

    public Door()
    {
        model = Raylib.LoadModelFromMesh(Raylib.GenMeshCube(1f, 1f, 1f));
        UseSound = AssetManager.Load<Sound>(@"Assets\Sounds\DoorUse.wav");
        LockSound = AssetManager.Load<Sound>(@"Assets\Sounds\DoorLocked.wav");

        Collider = new Collider(this)
        {
            CollisionBounds = Vector3.One * 0.5f
        };

        Renderer = new MeshRenderer
        {
            Texture = AssetManager.Load<Texture2D>(@"Assets\Textures\Default.png"),
            Model = model
        };
    }

    public override void Update()
    {
        base.Update();
    }

    public void Interact(Entity callingEntity)
    {
        Player? player = callingEntity as Player;

        if (player == null)
        {
            return;
        }

        if (!player.HasKey(ID))
        {
            GameplayStatics.PlaySoundAtLocation(LockSound, Transform.Position, 1.25f);
            return;
        }

        GameplayStatics.PlaySoundAtLocation(UseSound, Transform.Position, 1.25f);
        Destroy();
    }
}
