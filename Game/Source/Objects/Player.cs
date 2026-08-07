using Game.GUI;
using Game.LevelEditor;
using Raylib_cs;
using System.Numerics;

namespace Game.Objects;

[HideFromSpawnMenu]
public class Player : Character
{
    public event Action<Dictionary<AmmoType, int>>? OnAmmoChanged;

    public Camera Camera { get; private set; }
    private Vector3 _cameraOffset = Directions.Up * 0.375f;

    private float _pitch, _yaw;

    private HashSet<int> _collectedKeys;
    private Dictionary<AmmoType, int> _ammoInventory;

    public Player()
    {
        Name = "Player";
        Camera = new Camera();
        _collectedKeys = new HashSet<int>();
        _ammoInventory = new Dictionary<AmmoType, int>
        {
            { AmmoType.Pistol, 0 },
            { AmmoType.Shotgun, 0 }
        };

        Health = 100;

        if (Collider != null)
        {
            Collider.CollisionBounds = new Vector3(0.125f, 0.5f, 0.125f);
        }

        GameplayStatics.Camera = Camera;
    }

    public void SetYaw(float yaw)
    {
        _yaw = yaw;
    }

    public override void Update()
    {
        base.Update();

        Vector3 moveVector = Vector3.Zero;

        if (Raylib.IsKeyDown(KeyboardKey.W))
        {
            moveVector += GetForwardVector();
        }
        if (Raylib.IsKeyDown(KeyboardKey.S))
        {
            moveVector += GetBackwardVector();
        }
        if (Raylib.IsKeyDown(KeyboardKey.A))
        {
            moveVector += GetLeftVector();
        }
        if (Raylib.IsKeyDown(KeyboardKey.D))
        {
            moveVector += GetRightVector();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.E))
        {
            InteractTrace();
        }

        if(Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            GameplayStatics.PlaySoundAtLocation(AssetManager.Load<Sound>(@"Assets\Sounds\ShootGeneric.wav"), Camera.Transform.Position, 10f);
            ShootTrace();
            Debug.Log("Shoot Trace");
        }

        if (moveVector != Vector3.Zero)
        {
            moveVector = Vector3.Normalize(moveVector);
            Move(moveVector);
        }

        Vector2 delta = Raylib.GetMouseDelta();
        _yaw -= delta.X * Camera.Sensitivity * (float)Time.FrameDelta;

        _pitch -= delta.Y * Camera.Sensitivity * (float)Time.FrameDelta;
        _pitch = Math.Clamp(_pitch, -89.9f * Raylib.DEG2RAD, 89.9f * Raylib.DEG2RAD);

        if (Raylib.IsCursorHidden())
        {
            Transform.Rotation = Quaternion.CreateFromYawPitchRoll(_yaw, 0, 0);
            Camera.Transform.Rotation = Quaternion.Normalize(Transform.Rotation * Quaternion.CreateFromYawPitchRoll(0, _pitch, 0));
        }

        Camera.Transform.Position = Transform.Position + _cameraOffset;
        Camera.Update();
    }

    public bool LineTrace(float range, out Entity? hitEntity)
    {
        Vector3 start;
        start = Camera.Transform.Position;
        Ray ray = new Ray(start, Vector3.Normalize(Camera.GetForwardVector()));

        float closestDistance = float.MaxValue;
        hitEntity = null;

        foreach (Collider collider in World!.GetCollidables())
        {
            if (collider == Collider)
            {
                continue;
            }

            RayCollision hit = Raylib.GetRayCollisionBox(ray, collider.BoundingBox);

            if (!hit.Hit || hit.Distance > range)
            {
                continue;
            }

            if (hit.Distance < closestDistance)
            {
                closestDistance = hit.Distance;
                hitEntity = collider.Parent;
            }
        }

        return hitEntity != null;
    }

    public void InteractTrace()
    {
        if (LineTrace(0.65f, out Entity? hitEntity))
        {
            IInteractable? interactable = hitEntity as IInteractable;

            if (interactable != null)
            {
                interactable.Interact(this);
            }
        }
    }

    public void ShootTrace()
    {
        if(LineTrace(0.65f, out Entity? hitEntity))
        {
            IDamageable? damageable = hitEntity as IDamageable;

            if (damageable != null)
            {
                damageable.Damage(1);
            }
        }
    }

    public void AddKey(int id) => _collectedKeys.Add(id);
    public bool HasKey(int id) => _collectedKeys.Contains(id);
    public Dictionary<AmmoType, int> GetAmmoInventory() => _ammoInventory;
    public void AddAmmo(AmmoType type, int amount)
    {
        if (_ammoInventory.ContainsKey(type))
        {
            _ammoInventory[type] += amount;
        }
        else
        {
            _ammoInventory[type] = amount;
        }

        OnAmmoChanged?.Invoke(_ammoInventory);
    }
    public void RemoveAmmo(AmmoType type, int amount)
    {
        if (_ammoInventory.ContainsKey(type))
        {
            _ammoInventory[type] -= amount;

            if (_ammoInventory[type] < 0)
            {
                _ammoInventory[type] = 0;
            }
        }

        OnAmmoChanged?.Invoke(_ammoInventory);
    }

    public void TryShoot()
    {

    }

}
