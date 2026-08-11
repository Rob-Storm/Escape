using Game.GUI;
using Game.Objects;
using Raylib_cs;
using System.Numerics;

namespace Game;

public class World
{
    public static World? Instance { get; protected set; }

    public List<Entity> EntityList { get; set; }
    protected List<Entity> _removeEntityList { get; set; }
    protected List<Entity> _addEntityList { get; set; }

    protected List<Entity> _sortedBillboards;

    public Cell[,] Cells { get; set; }

    protected Player? _player;
    protected Camera? _camera;

    public int SizeX = 25;
    public int SizeY = 25;

    protected bool _debugDrawMode = false;

    private GameUI? _userInterface;

    public World()
    {
        if(Instance  == null)
        {
            Instance = this;
        }

        EntityList = new List<Entity>();
        _removeEntityList = new List<Entity>();
        _addEntityList = new List<Entity>();

        _sortedBillboards = new List<Entity>();

        Cells = new Cell[SizeX, SizeY];
    }

    public Cell? GetCell(int x, int y)
    {
        if (x < 0 || y < 0)
        {
            return null;
        }
        if (x > SizeX || y > SizeY)
        {
            return null;
        }

        return Cells[x, y];
    }

    public IEnumerable<Collider> GetCollidables()
    {
        foreach (Entity entity in EntityList)
        {
            yield return entity.Collider!;
        }

        foreach (Cell cell in Cells)
        {
            if (cell == null)
            {
                continue;
            }
        }
    }

    public Cell GetCell(Vector2 location)
    {
        return Cells[(int)location.X, (int)location.Y];
    }

    public void SetCell(int x, int y, Cell cell)
    {
        Cells[x, y] = cell;
    }

    public IEnumerable<(int x, int y, Cell cell)> GetCells()
    {
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                if (Cells[x, y] != null)
                {
                    yield return (x, y, Cells[x, y]);
                }
            }
        }
    }

    public void LoadLevel(Level level)
    {
        _player = new Player();
        _player._world = this;
        _camera = _player.Camera;
        EntityList = level.EntityList;
        Cells = level.Cells;

        _userInterface = new GameUI(_player);

        _player.Transform.Position = new Vector3(level.PlayerStart.X, 0.5f, level.PlayerStart.Y);
        _player.SetYaw(level.StartRotation * -Raylib.DEG2RAD);

        EntityList.Add(_player);

        foreach (Entity entity in level.EntityList)
        {
            entity.Start();
        }
    }

    public virtual void Update()
    {
        foreach (Entity entity in EntityList)
        {
            entity.Update();

            if (entity.GetMarkedForDelete())
            {
                _removeEntityList.Add(entity);
            }
        }

        SortBillboards();

        CheckEntityCollisions();

        EntityList.AddRange(_addEntityList);

        foreach (Entity entity in _addEntityList)
        {
            entity.Start();
        }

        _addEntityList.Clear();

        foreach (Entity entity in _removeEntityList)
        {
            EntityList.Remove(entity);
        }

        _removeEntityList.Clear();

        if (Raylib.IsKeyPressed(KeyboardKey.F3))
        {
            _debugDrawMode = !_debugDrawMode;
        }
    }

    public virtual void Render()
    {
        Raylib.ClearBackground(Color.Black);

        if (_camera == null)
        {
            return;
        }

        Raylib.BeginMode3D(_camera);

        foreach (Entity entity in EntityList.Where(e => e.Renderer is not BillboardRenderer))
        {
            entity.Render(_camera);
        }

        foreach(Entity billboard in _sortedBillboards)
        {
            billboard.Render(_camera);
        }

        foreach (var cellData in GetCells())
        {
            cellData.cell.Render();
        }

        Raylib.EndMode3D();

        if (_debugDrawMode)
        {
            Raylib.DrawFPS(0, 0);
        }
    }

    public virtual void Render2D()
    {
        if (_userInterface != null)
        {
            _userInterface.Render();
        }
    }

    public bool IsCollidingWithCell(Cell cell, BoundingBox collider)
    {
        foreach (BoundingBox wall in cell.GetWallColliders())
        {
            if (Raylib.CheckCollisionBoxes(collider, wall))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsCollidingWithEntity(Entity entity, BoundingBox collider)
    {
        return Raylib.CheckCollisionBoxes(entity.Collider!.BoundingBox, collider);
    }


    // HACK: This should really be changed to something more efficient.
    // This is O(n^2) and will not scale well with a large number of entities.
    // May try an octree for this engine iteration or use BSP after this game is shipped
    public void CheckEntityCollisions()
    {
        foreach (Entity entity in EntityList)
        {
            foreach (Entity instigator in EntityList)
            {
                if (entity == instigator)
                {
                    continue;
                }

                bool overlapping = Raylib.CheckCollisionBoxes(entity.Collider!.BoundingBox, instigator.Collider!.BoundingBox);

                entity.Collider.SetIsColliding(overlapping, instigator.Collider);
            }
        }
    }


    // Sort the billboards back to front to avoid transparency bugs
    public void SortBillboards()
    {
        List<Entity> billboards = new List<Entity>();
        billboards.AddRange(EntityList.Where(entity => entity.Renderer is BillboardRenderer));

        billboards.Sort((x, y) => GetDistanceToCamera(y).CompareTo(GetDistanceToCamera(x)));

        _sortedBillboards = billboards;
    }

    public float GetDistanceToCamera(Entity entity) => Vector3.DistanceSquared(_camera!.Transform.Position, entity.Transform.Position);


    public List<Entity> GetEntitiesOfClass(Type filter)
    {
        return EntityList.Where(entity => entity.GetType() == filter).ToList();
    }

    /// <summary>
    /// Returns the first entity of the given type T
    /// </summary>
    /// <typeparam name="T">The type to be filtered</typeparam>
    /// <returns></returns>
    public T GetEntityOfType<T>() where T : Entity
    {
        return (T)GetEntitiesOfClass(typeof(T)).First();
    }

    public RayHit LineTrace(Vector3 start, Vector3 end, CollisionChannel channelMask, params Entity[] ignoreEntity)
    {
        RayHit result = new RayHit();

        Vector3 direction = Vector3.Normalize(end - start);

        Ray ray = new Ray(start, direction);

        result.Distance = Vector3.Distance(start, end);

        foreach(Collider collider in GetCollidables())
        {
            if((collider.Channel & channelMask) == 0)
            {
                continue;
            }

            if(ignoreEntity.Contains(collider.Parent))
            {
                continue;
            }

            RayCollision collision = Raylib.GetRayCollisionBox(ray, collider.BoundingBox);

            if(collision.Hit && collision.Distance < result.Distance)
            {
                result.Hit = true;
                result.Distance = collision.Distance;
                result.Position = collision.Point;
                result.Normal = collision.Normal;
                result.Collider = collider;
            }    
        }

        return result;
    }
}

public struct RayHit
{
    public bool Hit {  get; set; }
    public float Distance { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Normal { get; set; }
    public Collider Collider { get; set; }
}