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
    public Cell Cell { get; set; }

    [HideProperty]
    public List<BoxCollider> Colliders { get; private set; }

    public CellCollider(Cell cell)
    {
        Cell = cell;

        Colliders = new List<BoxCollider>();
    }

    public override void Update(Transform transform)
    {
        foreach (BoxCollider collider in Colliders)
        {
            collider.Update(new Transform(Cell.Position, Quaternion.Identity, Vector3.One));
        }
    }

    public override void DebugDraw()
    {
        foreach(BoxCollider collider in Colliders)
        {
            collider.DebugDraw();
        }
    }

    public void BuildCollider()
    {
        Colliders.Clear();

        foreach (BoundingBox boundingBox in Cell.GetWallColliders())
        {
            Colliders.Add(BoxCollider.FromBoundingBox(boundingBox, CollisionChannel.WorldStatic));
        }
    }
}
