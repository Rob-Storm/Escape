using Game.LevelEditor;
using Raylib_cs;
using System.Numerics;

namespace Game.Objects;

public class DoorKey : Entity
{
    [ToolTip("The ID of the door this key unlocks")]
    public int DoorID = 0;
    public Sound CollectSound;

    public DoorKey()
    {
        CollectSound = AssetManager.Load<Sound>(@"Assets\Sounds\KeyCollect.wav");

        Renderer = new BillboardRenderer
        {
            Texture = AssetManager.Load<Texture2D>(@"Assets\Textures\KeyGeneric.png"),
            Bounce = true
        };

        Collider = new Collider(this)
        {
            CollisionBounds = Vector3.One * 0.25f,
            Solid = false
        };

        Collider.OnBeginOverlap += Collider_OnBeginOverlap;
    }

    private void Collider_OnBeginOverlap(Collider other)
    {
        Player? player = other.Parent as Player;

        if(player == null)
        {
            return;
        }

        player.AddKey(DoorID);
        GameplayStatics.PlaySoundAtLocation(CollectSound, Transform.Position, 0.5f);

        Debug.Log($"Collected key: {DoorID}");

        Destroy();
    }
}
