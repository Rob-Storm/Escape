using Game.LevelEditor;
using Raylib_cs;
using System.Numerics;

namespace Game.Objects;

public class DoorKey : Entity, IInteractable
{
    [ToolTip("The ID of the door this key unlocks")]
    public int DoorID = 0;
    public Sound CollectSound;

    public DoorKey()
    {
        CollectSound = AssetManager.Load<Sound>(@"Assets\Sounds\KeyCollect.wav");

        Renderer = new BillboardRenderer
        {
            Texture = AssetManager.Load<Texture2D>(@"Assets\Textures\KeyGeneric.png")
        };

        Collider = new Collider(this)
        {
            CollisionBounds = Vector3.One * 0.25f
        };
    }

    public void Interact(Entity callingEntity)
    {
        Player? player = callingEntity as Player;

        if (player == null)
        {
            return;
        }

        player.AddKey(DoorID);
        GameplayStatics.PlaySoundAtLocation(CollectSound, Transform.Position, 0.5f);

        Debug.Log($"Collected key: {DoorID}");

        Destroy();
    }
}
