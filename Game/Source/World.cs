using Raylib_cs;
using System.Numerics;

namespace Game;

public class World
{
    public List<Entity> EntityList { get; protected set; }
    public List<Cell> CellList { get; protected set; }

    protected Player _player;
    protected Camera _camera;
    protected Texture2D walls, floor, ceiling;

    public World()
    {
        EntityList = new List<Entity>();
        CellList = new List<Cell>();

        walls = AssetManager.Load<Texture2D>("Assets/Textures/Wall.png");
        floor = AssetManager.Load<Texture2D>("Assets/Textures/Floor.png");
        ceiling = AssetManager.Load<Texture2D>("Assets/Textures/Ceiling.png");
    }

    public void GenerateWorld()
    {
        int sizeX = 2;
        int sizeY = 2;

        for (int w = 0; w < sizeX; w++)
        {
            for (int h = 0; h < sizeY; h++)
            {
                Cell cell = new Cell(walls, floor, ceiling) { Position = new Vector3(w, 0, h) };

                if (w == 0)
                {
                    cell.Walls |= Walls.North;
                }

                if (w == sizeX - 1)
                {
                    cell.Walls |= Walls.South;
                }

                if (h == 0)
                {
                    cell.Walls |= Walls.East;
                }

                if (h == sizeY - 1)
                {
                    cell.Walls |= Walls.West;
                }
                
                CellList.Add(cell);
            }
        }
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
