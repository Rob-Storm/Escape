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
    public static WeaponData Knife()
    {
        return new WeaponData
        {
            Name = "Knife",
            Damage = 3,
            Range = 5f,
            FireRate = 1f,
            FireSound = AssetManager.Load<Sound>(@"Assets\Sounds\ShootGeneric.wav"),
            WeaponTexture = AssetManager.Load<Texture2D>(@"Assets\Textures\Knife.png"),
            AmmoType = null
        };
    }

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

    public static WeaponData Revolver()
    {
        return new WeaponData
        {
            Name = "Revolver",
            Damage = 15,
            Range = 5f,
            FireRate = 1f,
            FireSound = AssetManager.Load<Sound>(@"Assets\Sounds\ShootGeneric.wav"),
            WeaponTexture = AssetManager.Load<Texture2D>(@"Assets\Textures\Revolver.png"),
            AmmoType = Objects.AmmoType.Revolver
        };
    }

    public static WeaponData Shotgun()
    {
        return new WeaponData
        {
            Name = "Shotgun",
            Damage = 20,
            Range = 5f,
            FireRate = 1f,
            FireSound = AssetManager.Load<Sound>(@"Assets\Sounds\ShootGeneric.wav"),
            WeaponTexture = AssetManager.Load<Texture2D>(@"Assets\Textures\Shotgun.png"),
            AmmoType = Objects.AmmoType.Shotgun
        };
    }

    public static WeaponData SMG()
    {
        return new WeaponData
        {
            Name = "SMG",
            Damage = 5,
            Range = 5f,
            FireRate = 0.15f,
            FireSound = AssetManager.Load<Sound>(@"Assets\Sounds\ShootGeneric.wav"),
            WeaponTexture = AssetManager.Load<Texture2D>(@"Assets\Textures\SMG.png"),
            AmmoType = Objects.AmmoType.Pistol
        };
    }

    public static WeaponData Rifle()
    {
        return new WeaponData
        {
            Name = "Rifle",
            Damage = 20,
            Range = 10f,
            FireRate = 1.5f,
            FireSound = AssetManager.Load<Sound>(@"Assets\Sounds\ShootGeneric.wav"),
            WeaponTexture = AssetManager.Load<Texture2D>(@"Assets\Textures\Rifle.png"),
            AmmoType = Objects.AmmoType.Rifle
        };
    }

    public static WeaponData AutoRifle()
    {
        return new WeaponData
        {
            Name = "Auto Rifle",
            Damage = 10,
            Range = 10f,
            FireRate = 0.35f,
            FireSound = AssetManager.Load<Sound>(@"Assets\Sounds\ShootGeneric.wav"),
            WeaponTexture = AssetManager.Load<Texture2D>(@"Assets\Textures\AutoRifle.png"),
            AmmoType = Objects.AmmoType.AutoRifle
        };
    }

    public static WeaponData GrenadeLauncher()
    {
        return new WeaponData
        {
            Name = "Grenade Launcher",
            Damage = 50,
            Range = 5f,
            FireRate = 2f,
            FireSound = AssetManager.Load<Sound>(@"Assets\Sounds\ShootGeneric.wav"),
            WeaponTexture = AssetManager.Load<Texture2D>(@"Assets\Textures\GrenadeLauncher.png"),
            AmmoType = Objects.AmmoType.Grenade
        };
    }

    #endregion
}

public enum WeaponPreset
{
    Knife,
    Pistol,
    Revolver,
    Shotgun,
    SMG,
    Rifle,
    AutoRifle,
    GrenadeLauncher
}