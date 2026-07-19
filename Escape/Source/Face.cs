using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Escape;

public struct Face
{
    public VertexBuffer VertexBuffer { get; private set; }
    public IndexBuffer IndexBuffer { get; private set; }

    VertexPositionTexture[] _vertices;
    short[] _indices;

    public Face(FaceSide side, GraphicsDevice graphicsDevice)
    {
        _vertices = new VertexPositionTexture[4];

        _vertices[0] = new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0), new Vector2(1, 0));
        _vertices[1] = new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(0, 0));
        _vertices[2] = new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0), new Vector2(1, 1));
        _vertices[3] = new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0), new Vector2(0, 1));

        VertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, _vertices.Length, BufferUsage.WriteOnly);
        VertexBuffer.SetData<VertexPositionTexture>(_vertices);

        _indices = new short[6];
        _indices[0] = 0;
        _indices[1] = 1;
        _indices[2] = 2;
        _indices[3] = 2;
        _indices[4] = 1;
        _indices[5] = 3;

        IndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, _indices.Length, BufferUsage.WriteOnly);
        IndexBuffer.SetData<short>(_indices);

        Rotate(side);
    }

    private void Rotate(FaceSide side)
    {
        Matrix rotationMatrix;
        Vector3 translation = Vector3.Zero;

        switch (side)
        {
            case FaceSide.Front:
                rotationMatrix = Matrix.Identity;
                translation = Vector3.Forward;
                break;
            case FaceSide.Back:
                rotationMatrix = Matrix.CreateRotationY(MathHelper.Pi);
                translation = Vector3.Backward;
                break;
            case FaceSide.Top:
                rotationMatrix = Matrix.CreateRotationX(MathHelper.PiOver2);
                translation = Vector3.Up;
                break;
            case FaceSide.Bottom:
                rotationMatrix = Matrix.CreateRotationX(-MathHelper.PiOver2);
                translation = Vector3.Down;
                break;
            case FaceSide.Left:
                rotationMatrix = Matrix.CreateRotationY(-MathHelper.PiOver2);
                translation = Vector3.Left;
                break;
            case FaceSide.Right:
                rotationMatrix = Matrix.CreateRotationY(MathHelper.PiOver2);
                translation = Vector3.Right;
                break;
            default:
                rotationMatrix = Matrix.Identity;
                break;
        }

        for (int i = 0; i < _vertices.Length; i++)
        {
            Vector3 position = _vertices[i].Position;
            position = Vector3.Transform(_vertices[i].Position, rotationMatrix);

            _vertices[i].Position = position;
        }
    }

    public short[] GetIndices(short stride)
    {
        short[] indexOffsets = _indices;

        for (int i = 0; i < indexOffsets.Length; i++)
        {
            indexOffsets[i] += stride;
        }

        return indexOffsets;
    }
}

public enum FaceSide
{
    Front,
    Back,
    Top,
    Bottom,
    Left,
    Right
};