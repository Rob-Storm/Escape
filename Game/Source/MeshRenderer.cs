using Raylib_cs;
using System.Numerics;

namespace Game;

public class MeshRenderer : RenderComponent
{
    public Model Model { get; set; }

    public override void Render(Camera camera, Transform transform)
    {
        unsafe
        {
            Model.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Texture = Texture;
        }

        Raylib.DrawModelEx(Model, transform.Position, Directions.Up, transform.Rotation.ToEulerAngles().Y, transform.Scale, Color.White);
    }
}
