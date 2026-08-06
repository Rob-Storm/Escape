using Game.Objects;
using Raylib_cs;

namespace Game.GUI;

public class GameUI
{
    private string _ammoText = string.Empty;
    private string _healthText = string.Empty;

    public GameUI(Player player)
    {
        player.OnAmmoChanged += Player_OnAmmoChanged;
        player.OnDamaged += Player_OnDamaged;

        _healthText = player.Health.ToString();
    }

    private void Player_OnDamaged(int health)
    {
        _healthText = health.ToString();
    }

    private void Player_OnAmmoChanged(Dictionary<AmmoType, int> ammoInventory)
    {
        _ammoText = string.Empty;

        foreach(var item in ammoInventory)
        {
            _ammoText += $"{item.Key.ToString()}: {item.Value.ToString()}\n";
        }

        // Remove \n from last item
        _ammoText = _ammoText.Remove(_ammoText.Length - 1, 1);
    }

    public void Render()
    {
        Raylib.DrawText("Ammo", 0, 0, 24, Color.White);
        Raylib.DrawText(_ammoText, 0, 25, 24, Color.White);

        Raylib.DrawText($"HP: {_healthText}", 250, 0, 24, Color.White);
    }
}
