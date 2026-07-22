using Raylib_cs;
using System.Numerics;

namespace Game;

public class World
{
    public List<Entity> EntityList { get; set; }

    public Cell[,] Cells { get; set; }

    protected Player _player;
    protected Camera _camera;

    public int SizeX = 25;
    public int SizeY = 25;

    protected bool _debugDrawMode = false;

    public World()
    {
        EntityList = new List<Entity>();
        Cells = new Cell[SizeX, SizeY];
    }

    public Cell GetCell(int x, int y)
    {
        if(x < 0 || y < 0)
        {
            return null;
        }
        if (x > SizeX || y > SizeY)
        {
            return null;
        }

        return Cells[x, y];
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
        _player.World = this;
        _camera = _player.Camera;
        EntityList = level.EntityList;
        Cells = level.Cells;

        _player.Transform.Position = new Vector3(level.PlayerStart.X, 0.5f, level.PlayerStart.Y);
        _player.SetYaw(level.StartRotation * -Raylib.DEG2RAD);

        EntityList.Add(_player);
    }

    public virtual void Update()
    {
        foreach (Entity entity in EntityList)
        {
            entity.Update();
        }

        if (Raylib.IsKeyPressed(KeyboardKey.F3))
        {
            _debugDrawMode = !_debugDrawMode;
        }
    }

    public virtual void Render()
    {
        Raylib.ClearBackground(Color.Black);

        Raylib.BeginMode3D(_camera);

        foreach (Entity entity in EntityList)
        {
            entity.Render(_camera);
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
}
