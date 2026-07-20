using Raylib_cs;
using System.Numerics;

namespace Game;

/*
 * Todo:
 * 
 * Implement some way of consolidating the vertices of neighboring cells
 * to reduce memory usage
 */

public class Cell
{
    public string NorthWallTexturePath;
    public string EastWallTexturePath;
    public string SouthWallTexturePath;
    public string WestWallTexturePath;
    public string FloorTexturePath;
    public string CeilingTexturePath;

    public Texture2D NorthWallTexture => AssetManager.Load<Texture2D>(NorthWallTexturePath);
    public Texture2D EastWallTexture => AssetManager.Load<Texture2D>(EastWallTexturePath);
    public Texture2D SouthWallTexture => AssetManager.Load<Texture2D>(SouthWallTexturePath);
    public Texture2D WestWallTexture => AssetManager.Load<Texture2D>(WestWallTexturePath);
    public Texture2D FloorTexture => AssetManager.Load<Texture2D>(FloorTexturePath);
    public Texture2D CeilingTexture => AssetManager.Load<Texture2D>(CeilingTexturePath);

    public Vector3 Position { get; set; }
    public Walls Walls { get; set; } = Walls.None;

    private Mesh _horizontalPlane;
    private Mesh _verticalPlane;

    private Model _horizontalModel;
    private Matrix4x4 _transform;
    private  Model _verticalModel;

    public Cell() : this(0, 0) { }

    public Cell(int x, int y)
    {
        Position = new Vector3(x, 0, y);

        _horizontalPlane = Raylib.GenMeshPlane(1.0f, 1.0f, 1, 1);
        _verticalPlane = Raylib.GenMeshPlane(1.0f, 1.5f, 1, 1);
        _verticalModel = Raylib.LoadModelFromMesh(_verticalPlane);

        _transform = Matrix4x4.CreateRotationX(-MathF.PI / 2);
        _verticalModel.Transform = _transform;

        _horizontalModel = Raylib.LoadModelFromMesh(_horizontalPlane);
    }

    public BoundingBox[] GetWallColliders()
    {
        List<BoundingBox> colliders = new List<BoundingBox>();

        if (Walls.HasFlag(Walls.North))
        {
            colliders.Add(GetWallDirectionCollider(Walls.North));
        }
        if (Walls.HasFlag(Walls.East))
        {
            colliders.Add(GetWallDirectionCollider(Walls.East));
        }
        if (Walls.HasFlag(Walls.South))
        {
            colliders.Add(GetWallDirectionCollider(Walls.South));
        }
        if (Walls.HasFlag(Walls.West))
        {
            colliders.Add(GetWallDirectionCollider(Walls.West));
        }

        return colliders.ToArray();
    }

    public BoundingBox GetWallDirectionCollider(Walls wall)
    {
        const float thickness = 0.05f;
        const float height = 1.5f;

        Vector3 dir = GetDirection(wall);

        Vector3 center = Position + dir * 0.5f + Vector3.UnitY * (height * 0.5f);

        Vector3 halfExtents;

        if (Math.Abs(dir.X) > 0)
        {
            halfExtents = new Vector3(thickness / 2, height / 2, 0.5f);
        }
        else
        {
            halfExtents = new Vector3(0.5f, height / 2, thickness / 2);
        }

        return new BoundingBox(center - halfExtents, center + halfExtents);
    }

    private static Vector3 GetDirection(Walls wall) => wall switch
    {
        Walls.North => Directions.Forward,
        Walls.East => Directions.Left,
        Walls.South => Directions.Backward,
        Walls.West => Directions.Right,
        _ => Vector3.Zero
    };

    public void Render()
    {
        RenderFloor();
        RenderCeiling();

        if (Walls.HasFlag(Walls.North))
        {
            RenderWall(0f);
        }
        if (Walls.HasFlag(Walls.East))
        {
            RenderWall(90f);
        }
        if (Walls.HasFlag(Walls.South))
        {
            RenderWall(180f);
        }
        if (Walls.HasFlag(Walls.West))
        {
            RenderWall(270f);
        }
    }

    public void RenderBounds(Color cellColor, Color boundColor)
    {
        // walls which may not be visible to the editor camera
        foreach (BoundingBox collider in GetWallColliders())
        {
            Raylib.DrawBoundingBox(collider, boundColor);
        }

        // entire cell bounds
        Raylib.DrawCubeWires(Position + Vector3.UnitY* 0.75f, 1, 1.5f, 1, cellColor);
    }

    private void RenderWall(float rotation)
    {
        Texture2D texture = NorthWallTexture;
        Vector3 offset = Vector3.Zero;
        Color color = Color.White;

        switch (rotation)
        {
            case 0f:
                offset = Directions.Forward;
                color = Color.Red;
                texture = NorthWallTexture;
                break;
            case 90f:
                offset = Directions.Left;
                color = Color.Green;
                texture = EastWallTexture;
                rotation += 180f;
                break;
            case 180f:
                offset = Directions.Backward;
                color = Color.Blue;
                texture = SouthWallTexture;
                break;
            case 270f:
                offset = Directions.Right;
                color = Color.Yellow;
                texture = WestWallTexture;
                rotation += 180f;
                break;
        }

        unsafe
        {
            _verticalModel.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Texture = texture;
        }

        Raylib.DrawModelEx(_verticalModel, Position + (offset * 0.5f) + (Vector3.UnitY * 0.75f), Vector3.UnitY, rotation, Vector3.One, Color.White);
    }
    private void RenderCeiling()
    {
        unsafe
        {
            _horizontalModel.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Texture = CeilingTexture;
        }

        Raylib.DrawModelEx(_horizontalModel, Position + (Directions.Up * 1.5f), Vector3.UnitX, 180f, Vector3.One, Color.White);
    }
    private void RenderFloor()
    {
        unsafe
        {
            _horizontalModel.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Texture = FloorTexture;
        }

        Raylib.DrawModelEx(_horizontalModel, Position, Vector3.UnitX, 0f, Vector3.One, Color.White);
    }

    public override string ToString()
    {
        return Position.ToString();
    }
}

[Flags]
public enum Walls
{
    None = 0,
    North = 1 << 0,
    East = 1 << 1,
    South = 1 << 2,
    West = 1 << 3
}