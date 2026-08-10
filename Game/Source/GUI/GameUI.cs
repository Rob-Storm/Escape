using Game.Objects;
using Raylib_cs;
using System.Numerics;

namespace Game.GUI;

public class GameUI
{
    private string _ammoText = string.Empty;
    private string _healthText = string.Empty;
    private string _weaponText = string.Empty;

    private Texture2D _crosshair;

    public GameUI(Player player)
    {
        player.OnAmmoChanged += Player_OnAmmoChanged;
        player.OnDamaged += Player_OnDamaged;
        player.OnWeaponChanged += Player_OnWeaponChanged;

        _healthText = player.Health.ToString();
        Player_OnAmmoChanged(player.GetAmmoInventory());

        _weaponText = "None";

        _crosshair = AssetManager.Load<Texture2D>(@"Assets\Textures\Crosshair.png");
    }

    private void Player_OnWeaponChanged(WeaponData? weapon)
    {
        _weaponText = weapon != null ? weapon!.Value.Name : "None";
    }

    private void Player_OnDamaged(int health)
    {
        _healthText = health.ToString();
    }

    private void Player_OnAmmoChanged(Dictionary<AmmoType, int> ammoInventory)
    {
        _ammoText = string.Empty;

        foreach (var item in ammoInventory)
        {
            _ammoText += $"{item.Key.ToString()}: {item.Value.ToString()}\n";
        }

        // Remove \n from last item
        _ammoText = _ammoText.Remove(_ammoText.Length - 1, 1);
    }

    public void Render()
    {
        Vector2 viewportCenter = Raylib.GetScreenCenter();

        Raylib.DrawText("Ammo", 0, 0, 24, Color.White);
        Raylib.DrawText(_ammoText, 0, 25, 24, Color.White);

        Raylib.DrawText($"Weapon: {_weaponText}", 175, 0, 24, Color.White);

        Raylib.DrawText($"HP: {_healthText}", 500, 0, 24, Color.White);

        Raylib.DrawTexture(_crosshair, (int)viewportCenter.X - (_crosshair.Width / 2), (int)viewportCenter.Y - (_crosshair.Height / 2), Color.White);
    }
}
