using Game.LevelEditor;
using Raylib_cs;
using System.Numerics;

namespace Game.Objects;

public class WeaponPickup : Entity
{
    private WeaponPreset _weaponPreset;
    public WeaponPreset WeaponPreset
    {
        get
        {
            return _weaponPreset;
        }
        set 
        {
            _weaponPreset = value;

            switch (value)
            {
                case WeaponPreset.Knife:
                    _weapon = WeaponData.Knife();
                    break;
                case WeaponPreset.Pistol:
                    _weapon = WeaponData.Pistol();
                    break;
                case WeaponPreset.Revolver:
                    _weapon = WeaponData.Revolver();
                    break;
                case WeaponPreset.Shotgun:
                    _weapon = WeaponData.Shotgun();
                    break;
                case WeaponPreset.SMG:
                    _weapon = WeaponData.SMG();
                    break;
                case WeaponPreset.Rifle:
                    _weapon = WeaponData.Rifle();
                    break;
                case WeaponPreset.AutoRifle:
                    _weapon = WeaponData.AutoRifle();
                    break;
                case WeaponPreset.GrenadeLauncher:
                    _weapon = WeaponData.GrenadeLauncher();
                    break;
            }

            Renderer!.Texture = _weapon.WeaponTexture;
        }
    }

    [ToolTip("Ammo that will be added upon pickup")]
    public int Ammo { get; set; } = 0;

    public Sound PickupSound { get; set; } = AssetManager.Load<Sound>(@"Assets\Sounds\WeaponPickup.wav");

    private WeaponData _weapon;
    public WeaponPickup()
    {
        Collider = new Collider(this)
        {
            CollisionBounds = Vector3.One * 0.5f,
            Solid = false
        };

        Renderer = new BillboardRenderer
        {
            Texture = _weapon.WeaponTexture,
            AutoSize = true,
            Bounce = true
        };

        Collider.OnBeginOverlap += Collider_OnBeginOverlap;
    }

    private void Collider_OnBeginOverlap(Collider other)
    {
        Player? player = other.Parent as Player;

        if (player == null)
        {
            return;
        }

        if(_weapon.AmmoType != null)
        {
            player.AddAmmo(_weapon.AmmoType!.Value, Ammo);
        }

        GameplayStatics.PlaySoundAtLocation(PickupSound, Transform.Position, 1f);
        Destroy();
    }
}
