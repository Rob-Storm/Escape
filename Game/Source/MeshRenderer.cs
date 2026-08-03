using Raylib_cs;
using System.Numerics;

namespace Game;

public class MeshRenderer : RenderComponent
{
    private Model _model;
    public Model Model { get => _model; set => _model = value; }

    public override void Render(Camera camera, Transform transform)
    {
        unsafe
        {
            Model.Materials[0].Maps[(int)MaterialMapIndex.Diffuse].Texture = Texture;
        }

        _model.Transform = Matrix4x4.CreateScale(transform.Scale) * Matrix4x4.CreateFromQuaternion(transform.Rotation);

        Raylib.DrawModel(Model, transform.Position, 1f, Color.White);
    }

    public override void DebugRender(Camera camera, Transform transform)
    {
        Raylib.DrawModelWires(Model, transform.Position, 1f, Color.Orange);
    }
}
