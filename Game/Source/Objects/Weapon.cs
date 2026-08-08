using Raylib_cs;

namespace Game.Objects;

public struct WeaponData
{
    public string Name { get; set; } = "Weapon";
    public int Damage { get; set; } = 1;
    public float Range { get; set; } = 5f;
    public float FireRate { get; set; } = 0.5f;
    public Sound FireSound { get; set; } = AssetManager.Load<Sound>(@"Assets\Sounds\ShootGeneric.wav");
    public Texture2D WeaponTexture { get; set; } = AssetManager.Load<Texture2D>(@"Assets\Textures\Pistol.png");

    // Melee or infinite-ammo weapons should use null
    public AmmoType? AmmoType { get; set; } = null;

    public WeaponData()
    {
        
    }

    #region Weapon preset definitions
    public static WeaponData Pistol()
    {
        return new WeaponData
        {
            Name = "Pistol",
            Damage = 5,
            Range = 5f,
            FireRate = 1f,
            FireSound = AssetManager.Load<Sound>(@"Assets\Sounds\ShootGeneric.wav"),
            WeaponTexture = AssetManager.Load<Texture2D>(@"Assets\Textures\Pistol.png"),
            AmmoType = Objects.AmmoType.Pistol
        };
    }

    public static WeaponData Shotgun()
    {
        return new WeaponData
        {
            Name = "Shotgun",
            Damage = 1,
            Range = 5f,
            FireRate = 1f,
            FireSound = AssetManager.Load<Sound>(@"Assets\Sounds\ShootGeneric.wav"),
            WeaponTexture = AssetManager.Load<Texture2D>(@"Assets\Textures\Shotgun.png"),
            AmmoType = Objects.AmmoType.Shotgun
        };
    }

    #endregion
}