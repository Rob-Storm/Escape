using Game.LevelEditor;
using System.Numerics;

namespace Game.Objects;

public class WeaponPickup : Entity
{
    public WeaponData Weapon { get; set; } = WeaponData.Pistol();

    [ToolTip("Ammo that will be added upon pickup")]
    public int Ammo { get; set; } = 0;

    public WeaponPickup()
    {
        Collider = new Collider(this)
        {
            CollisionBounds = Vector3.One * 0.5f,
            Solid = false
        };

        Renderer = new BillboardRenderer
        {
            Texture = Weapon.WeaponTexture,
            AutoSize = true,
            Bounce = true
        };
    }
}
