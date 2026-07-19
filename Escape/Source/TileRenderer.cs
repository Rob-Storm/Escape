using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Escape;

public class TileRenderer : RenderComponent
{
    private Face _ceiling, _floor;
    private Face[] _walls;

    private float _padding = 1.5f;

    public Texture2D CeilingTexture, FloorTexture, WallTexture;

    public TileRenderer(GraphicsDevice device) : base(device)
    {
        _ceiling = new Face(FaceSide.Top, device);
        _floor = new Face(FaceSide.Bottom, device);

        _walls = new Face[4]
        {
            new Face(FaceSide.Front, device),
            new Face(FaceSide.Back, device), 
            new Face(FaceSide.Left, device), 
            new Face(FaceSide.Right, device)
        };

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
        Transform ceilingTransform = transform.Copy();
        ceilingTransform.Rotate(Vector3.Up * MathHelper.ToRadians(90f));
        ceilingTransform.Translate(Vector3.Up * _padding);

        Transform floorTransform = transform.Copy();
        floorTransform.Rotate(Vector3.Down * MathHelper.ToRadians(90f));
        floorTransform.Translate(Vector3.Down * _padding);

        DrawFace(_ceiling, CeilingTexture, ceilingTransform, camera);
        DrawFace(_floor, FloorTexture, floorTransform, camera);

        for (int i = 0; i < _walls.Length; i++)
        {
            Transform wallTransform = transform.Copy();
            switch(i)
            {
                case 0:
                    wallTransform.Rotate(Vector3.Forward * MathHelper.ToRadians(90f));
                    wallTransform.Translate(Vector3.Forward * _padding);
                    break;
                case 1:
                    wallTransform.Rotate(Vector3.Backward * MathHelper.ToRadians(90f));
                    wallTransform.Translate(Vector3.Backward * _padding);
                    break;
                case 2:
                    wallTransform.Rotate(Vector3.Left * MathHelper.ToRadians(90f));
                    wallTransform.Translate(Vector3.Left * _padding);
                    break;
                case 3:
                    wallTransform.Rotate(Vector3.Right * MathHelper.ToRadians(90f));
                    wallTransform.Translate(Vector3.Right * _padding);
                    break;
            }

            DrawFace(_walls[i], WallTexture, wallTransform, camera);
        }
    }

    private void DrawFace(Face face, Texture2D texture, Transform transform, Camera camera)
    {
        _basicEffect.World = Matrix.CreateScale(transform.Scale * 1.5f) * Matrix.CreateFromQuaternion(transform.Rotation) * Matrix.CreateTranslation(transform.Position);
        _basicEffect.View = camera.GetViewMatrix();
        _basicEffect.Projection = camera.GetProjectionMatrix();

        _basicEffect.Texture = texture;
        _basicEffect.DiffuseColor = Vector3.One;

        _device.SetVertexBuffer(face.VertexBuffer);
        _device.Indices = face.IndexBuffer;

        foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
        }
    }
}