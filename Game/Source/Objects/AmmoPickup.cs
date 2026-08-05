using Game.LevelEditor;

namespace Game.Objects;

public class AmmoPickup : Entity, IInteractable
{
    [ToolTip("The ammo type provided by this pickup")]
    public AmmoType AmmoType = AmmoType.Pistol;

    [ToolTip("The amount of ammo provided by this pickup")]
    public int AmmoAmount = 0;

    public void Interact(Entity callingEntity)
    {
        Player? player = callingEntity as Player;

        if(player != null)
        {
            player.AddAmmo(AmmoType.Pistol, 10);
            Destroy();
        }   
    }
}

public enum AmmoType
{
    Pistol
};