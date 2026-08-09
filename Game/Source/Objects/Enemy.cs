using Raylib_cs;
using System.Numerics;

namespace Game.Objects;

public class Enemy : Character
{
    private bool _canAttack = true;

    private Player? _overlappedPlayer;

    public Enemy()
    {
        Collider = new Collider(this)
        {
            CollisionBounds = new Vector3(0.35f, 0.5f, 0.35f),
            Solid = false
        };

        Renderer = new BillboardRenderer
        {
            Texture = AssetManager.Load<Texture2D>(@"Assets\Textures\Man.png")
        };

        OnDamaged += (health) => { Debug.Log(health.ToString()); };

        Collider.OnBeginOverlap += Collider_OnBeginOverlap;
        Collider.OnEndOverlap += Collider_OnEndOverlap;
    }

    public override void Update()
    {
        base.Update();

        DamagePlayer();
    }

    private void Collider_OnEndOverlap(Collider other)
    {
        Player? player = other.Parent as Player;

        if (player == null)
        {
            return;
        }

        _overlappedPlayer = null;

        _canAttack = true;
    }

    private void Collider_OnBeginOverlap(Collider other)
    {
        Player? player = other.Parent as Player;

        if(player == null)
        {
            return;
        }

        _overlappedPlayer = player;

        DamagePlayer();

    }

    private void DamagePlayer()
    {
        if (!_canAttack || _overlappedPlayer == null)
        {
            return;
        }

        _overlappedPlayer.Damage(5);

        _canAttack = false;

        TimerManager.SetTimer(1f, () => { _canAttack = true; });
    }
}
