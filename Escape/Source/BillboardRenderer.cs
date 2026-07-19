using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Escape;

public class BillboardRenderer : RenderComponent
{
    private Face _face;
    public BillboardRenderer(GraphicsDevice device) : base(device)
    {
        _face = new Face(FaceSide.Front, device);

        _basicEffect.TextureEnabled = true;

        _basicEffect.LightingEnabled = false;
        _basicEffect.VertexColorEnabled = false;

        RasterizerState rasterizerState = new RasterizerState();
        rasterizerState.CullMode = CullMode.None;
        _device.RasterizerState = rasterizerState;
        _device.SamplerStates[0] = SamplerState.PointClamp;
        _device.BlendState = BlendState.AlphaBlend;
    }

    public override void Draw(GameTime gameTime, Transform transform, Camera camera)
    {
        _basicEffect.World = Matrix.CreateScale(1f) * Matrix.CreateConstrainedBillboard(transform.Position, camera.Transform.Position, Vector3.UnitY, null, null);

        _basicEffect.View = camera.GetViewMatrix();
        _basicEffect.Projection = camera.GetProjectionMatrix();

        _basicEffect.Texture = Texture;
        _basicEffect.DiffuseColor = Vector3.One;

        _device.SetVertexBuffer(_face.VertexBuffer);
        _device.Indices = _face.IndexBuffer;

        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
        }
    }
}
