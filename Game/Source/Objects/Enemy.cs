using System.Numerics;

namespace Game.Objects;

public class Enemy : Character
{
    public Enemy()
    {
        Collider = new Collider(this)
        {
            CollisionBounds = new Vector3(0.35f, 0.5f, 0.35f)
        };

        Renderer = new BillboardRenderer
        {

        };

        OnDamaged += (h) => { Debug.Log(h.ToString()); };
    }
}
