using Raylib_cs;
using System.Numerics;

namespace Game;

public class World
{
    public List<Entity> EntityList { get; protected set; }
    public List<Cell> CellList { get; protected set; }

    protected Player _player;
    protected Camera _camera;
    protected string walls, floor, ceiling;

    public World()
    {
        EntityList = new List<Entity>();
        CellList = new List<Cell>();

        walls = "Assets/Textures/Wall.png";
        floor = "Assets/Textures/Floor.png";
        ceiling = "Assets/Textures/Ceiling.png";
    }


    public void LoadLevel(Level level)
    {
        _player = new Player();
        _player.World = this;
        _camera = _player.Camera;
        EntityList = level.EntityList;
        CellList = level.CellList;

        _player.Transform.Position = new Vector3(level.PlayerStart.X, 0.5f, level.PlayerStart.Y);

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

        foreach (Cell cell in CellList)
        {
            cell.Render();
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
