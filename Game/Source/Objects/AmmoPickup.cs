using Game.LevelEditor;
using Raylib_cs;
using System.Numerics;

namespace Game.Objects;

public class AmmoPickup : Entity
{
    [ToolTip("The ammo type provided by this pickup")]
    public AmmoType AmmoType = AmmoType.Pistol;

    [ToolTip("The amount of ammo provided by this pickup")]
    public int AmmoAmount = 0;

    public Sound PickupSound = AssetManager.Load<Sound>(@"Assets\Sounds\AmmoPickup.wav");

    public AmmoPickup()
    {
        Renderer = new BillboardRenderer
        {
            Texture = AssetManager.Load<Texture2D>(@"Assets\Textures\PistolAmmo.png"),
            Bounce = true
        };

        Collider = new Collider(this)
        {
            CollisionBounds = Vector3.One * 0.5f,
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

        player.AddAmmo(AmmoType, AmmoAmount);
        GameplayStatics.PlaySoundAtLocation(PickupSound, Transform.Position, 1f);
        Destroy();

    }
}

public enum AmmoType
{
    Pistol,
    Revolver,
    Shotgun,
    Rifle,
    AutoRifle,
    Grenade
};