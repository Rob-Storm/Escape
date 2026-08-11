using Game.LevelEditor;
using Game.Utility;
using Raylib_cs;
using System.Numerics;

namespace Game.Objects;

[HideFromSpawnMenu]
public class Player : Character
{
    public event Action<Dictionary<AmmoType, int>>? OnAmmoChanged;
    public event Action<WeaponData?>? OnWeaponChanged;

    public Camera Camera { get; private set; }
    private Vector3 _cameraOffset = Directions.Up * 0.375f;

    private float _pitch, _yaw;

    private HashSet<int> _collectedKeys;
    private Dictionary<AmmoType, int> _ammoInventory;

    private List<WeaponData> _weaponInventory;

    private WeaponData? _currentWeapon = null;

    private bool _shootCooldownFinished = true;

    public Player()
    {
        Name = "Player";
        MoveSpeed = 2.75f;
        Camera = new Camera();
        _collectedKeys = new HashSet<int>();
        _ammoInventory = new Dictionary<AmmoType, int>();
        _weaponInventory = new List<WeaponData>();

        foreach (AmmoType ammoType in EnumUtil.GetValues<AmmoType>())
        {
            _ammoInventory.Add(ammoType, 0);
        }

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
            InteractTrace(0.65f);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Q))
        {
            ChangeWeapon(false);
        }

        if (Raylib.IsMouseButtonDown(MouseButton.Left))
        {
            TryShoot();
        }

        if (Raylib.GetMouseWheelMoveV() != Vector2.Zero)
        {
            float deltaY = Raylib.GetMouseWheelMoveV().Y;

            if (deltaY > 0)
            {
                ChangeWeapon(false);
            }
            else if (deltaY < 0)
            {
                ChangeWeapon(true);
            }
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

    // HACK: Line traces should be done by the engine or world!

    public void InteractTrace(float range)
    {
        Vector3 start = Camera.Transform.Position;

        Vector3 end = start + (Camera.GetForwardVector() * range);

        RayHit trace = _world.LineTrace(start, end, CollisionChannel.WorldDynamic, this);

        if(trace.Hit)
        {
            IInteractable? interactable = trace.Collider.Parent as IInteractable;

            if (interactable != null)
            {
                interactable.Interact(this);
            }
        }
    }

    public void ShootTrace(float range)
    {
        if(_currentWeapon == null)
        {
            return;
        }

        WeaponData currentWeapon = _currentWeapon!.Value;

        Vector3 start = Camera.Transform.Position;

        Vector3 end = start + (Camera.GetForwardVector() * range);

        RayHit trace = _world.LineTrace(start, end, CollisionChannel.Character, this);

        if(trace.Hit)
        {
            IDamageable? damageable = trace.Collider.Parent as IDamageable;

            if (damageable != null)
            {
                damageable.Damage(currentWeapon.Damage);
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

    private void TryShoot()
    {
        if(_currentWeapon ==  null)
        {
            return;
        }

        if (!_shootCooldownFinished)
        {
            return;
        }

        WeaponData currentWeapon = _currentWeapon.Value!;

        AmmoType? ammo = currentWeapon.AmmoType;

        // Melee or other infinite-ammo weapon
        if (ammo == null)
        {
            ShootTrace(currentWeapon.Range);
            GameplayStatics.PlaySound2D(currentWeapon.FireSound);
            _shootCooldownFinished = false;

            TimerManager.SetTimer(currentWeapon.FireRate, () => { _shootCooldownFinished = true; });
            return;
        }

        // Weapon is empty
        if (_ammoInventory[ammo!.Value] <= 0)
        {
            GameplayStatics.PlaySound2D(AssetManager.Load<Sound>(@"Assets\Sounds\DryFire.wav"), 1f);
            _shootCooldownFinished = false;

            TimerManager.SetTimer(0.5f, () => { _shootCooldownFinished = true; });

            return;
        }

        ShootTrace(currentWeapon.Range);

        // Normal shoot logic
        GameplayStatics.PlaySound2D(currentWeapon.FireSound);

        RemoveAmmo(currentWeapon.AmmoType!.Value, 1);

        _shootCooldownFinished = false;

        TimerManager.SetTimer(currentWeapon.FireRate, () => { _shootCooldownFinished = true; });
    }

    public void AddWeapon(WeaponData weapon)
    {
        if(_weaponInventory.Contains(weapon))
        {
            return;
        }

        _weaponInventory.Add(weapon);
    }

    public void RemoveWeapon(WeaponData weapon)
    {
        if(!_weaponInventory.Contains(weapon))
        {
            return;
        }

        _weaponInventory.Remove(weapon);
    }

    private void ChangeWeapon(bool previous)
    {
        if(_weaponInventory.Count <= 0 )
        {
            return;
        }

        int currentWeaponIndex = _currentWeapon != null ? _weaponInventory.IndexOf(_currentWeapon!.Value) : 0;

        if (previous)
        {
            PreviousWeapon(currentWeaponIndex);
        }
        else
        {
            NextWeapon(currentWeaponIndex);
        }

        OnWeaponChanged?.Invoke(_currentWeapon.Value);
    }

    private void PreviousWeapon(int weaponIndex)
    {
        if(weaponIndex == 0)
        {
            weaponIndex = _weaponInventory.Count - 1;
        }
        else
        {
            weaponIndex--;
        }

        _currentWeapon = _weaponInventory[weaponIndex];
    }

    private void NextWeapon(int weaponIndex)
    {
        if (weaponIndex == _weaponInventory.Count - 1)
        {
            weaponIndex = 0;
        }
        else
        {
            weaponIndex++;
        }

        _currentWeapon = _weaponInventory[weaponIndex];
    }
}
