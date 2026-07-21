using Raylib_cs;
using System.Numerics;

namespace Game;

public class World
{
    public List<Entity> EntityList { get; set; }

    public Cell[,] Cells { get; set; }

    protected Player _player;
    protected Camera _camera;

    public const int WORLD_WIDTH = 10, WORLD_HEIGHT = 10;

    public World()
    {
        EntityList = new List<Entity>();
        Cells = new Cell[WORLD_WIDTH, WORLD_HEIGHT];
    }

    public Cell GetCell(int x, int y)
    {
        return Cells[x,y];
    }
    public void SetCell(int x, int y, Cell cell)
    {
        Cells[x,y] = cell;
    }

    public IEnumerable<(int x, int y, Cell cell)> GetCells()
    {
        for(int x = 0; x < WORLD_WIDTH;  x++)
        {
            for (int y = 0; y < WORLD_HEIGHT; y++)
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
        _player.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(level.StartRotation, 0, 0);

        EntityList.Add(_player);
    }

    public virtual void Update()
    {
        foreach (Entity entity in EntityList)
        {
            entity.Update();
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

        foreach(var cellData in GetCells())
        {
            cellData.cell.Render();
        }

        Raylib.EndMode3D();
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
