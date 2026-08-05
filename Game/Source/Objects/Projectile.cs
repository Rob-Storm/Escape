using Game.LevelEditor;

namespace Game.Objects;

[HideFromSpawnMenu]
public class Projectile : Entity
{
    public Projectile()
    {
        Renderer = new BillboardRenderer
        {
        };

        Collider = new Collider(this)
        {
            Solid = false
        };
    }

    public override void Update()
    {
        base.Update();

        // todo:
        // move forward until overlap
        // do damage
        // destroy self
    }
}
