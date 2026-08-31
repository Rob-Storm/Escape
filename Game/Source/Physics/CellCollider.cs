using Game.LevelEditor;
using Raylib_cs;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Game.Physics;

/// <summary>
/// A composite collider made of at least 2 box colliders (floor and ceiling), with the rest made procedurally based on the cell's wall flags
/// </summary>
public class CellCollider : Collider
{
    [HideProperty]
    [JsonIgnore]
    public Cell? Cell { get; }

    [HideProperty]
    public List<BoxCollider> Colliders { get; private set; }

    public CellCollider(Cell cell)
    {
        Cell = cell;

        Colliders = new List<BoxCollider>();

        if(Cell != null)
        {
            cell.OnWallFlagsChanged += BuildCollider;
            BuildCollider();
        }

    }

    public override void Update(Transform transform)
    {
        foreach (BoxCollider collider in Colliders)
        {
            collider.Update(new Transform(Cell.Position, Quaternion.Identity, Vector3.One));
        }
    }

    public override void DebugDraw(Transform transform)
    {
        foreach(BoxCollider collider in Colliders)
        {
            Raylib.DrawBoundingBox(collider.BoundingBox, Color);
        }
    }

    private void BuildCollider()
    {
        // keep the floor and ceiling
        if(Colliders.Count > 2)
        {
            Colliders.RemoveRange(2, Colliders.Count - 2);
        }

        foreach (BoundingBox boundingBox in Cell.GetWallColliders())
        {
            Colliders.Add(BoxCollider.FromBoundingBox(boundingBox, CollisionChannel.WorldStatic));
        }
    }
}
