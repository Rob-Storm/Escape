using Game.LevelEditor;
using Game.Physics;
using Game.Graphics;


namespace Game.Objects;

[HideFromSpawnMenu]
public class Projectile : Entity
{
    public Projectile()
    {
        Renderer = new BillboardRenderer
        {
        };

        Collider = new BoxCollider(this)
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
