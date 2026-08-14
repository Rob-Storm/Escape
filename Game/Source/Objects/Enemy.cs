using Raylib_cs;
using System.Numerics;

namespace Game.Objects;

public class Enemy : Character
{
    private bool _canAttack = true;

    private Player? _player;

    private AIState _aiState;

    public Enemy()
    {
        Collider = new Collider(this)
        {
            CollisionBounds = new Vector3(0.35f, 0.5f, 0.35f),
            Solid = false,
            Channel = CollisionChannel.Character
        };

        Renderer = new BillboardRenderer
        {
            Texture = AssetManager.Load<Texture2D>(@"Assets\Textures\Man.png")
        };

        OnDamaged += (health) => { Debug.Log(health.ToString()); };

        Collider.OnBeginOverlap += Collider_OnBeginOverlap;
        Collider.OnEndOverlap += Collider_OnEndOverlap;
    }

    public override void Start()
    {
        if(World.Instance != null)
        {
            _player = World.Instance.GetEntityOfType<Player>();
        }

        TimerManager.SetTimer(5f, () => { _aiState = AIState.Move; });
    }

    public override void Update()
    {
        base.Update();

        switch(_aiState)
        {
            case AIState.Wander:
                // wander
                break;
            case AIState.Move:

                if(_player == null)
                {
                    break;
                }

                Vector3 moveDirection = Vector3.Normalize(_player.Transform.Position - Transform.Position);

                Transform.Translate(moveDirection * (float)Time.FrameDelta);

                break;
            case AIState.Attack:
                DamagePlayer();
                break;
        }
        
    }

    private void Collider_OnEndOverlap(Collider other)
    {
        Player? player = other.Parent as Player;

        if (player == null)
        {
            return;
        }

        _player = null;

        _canAttack = true;
    }

    private void Collider_OnBeginOverlap(Collider other)
    {
        Player? player = other.Parent as Player;

        if(player == null)
        {
            return;
        }

        _player = player;

        DamagePlayer();

    }

    private void DamagePlayer()
    {
        if (!_canAttack || _player == null)
        {
            return;
        }

        _player.Damage(5);

        _canAttack = false;

        TimerManager.SetTimer(1f, () => { _canAttack = true; });
    }
}


/* AI State:
 * 
 * Wander (no player in sight)
 * Move (get in range of weapon)
 * Attack (shoot/stab)
 * 
 */

public enum AIState
{
    Wander,
    Move,
    Attack
}