using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Escape;

public class Tile
{
    public Transform Transform { get; set; } = new Transform();

    TileRenderer _renderer;

    public void LoadContent(ContentManager content, GraphicsDevice device)
    {
        _renderer = new TileRenderer(device);

        _renderer.CeilingTexture = content.Load<Texture2D>("Textures/FloorMetal");
        _renderer.FloorTexture = content.Load<Texture2D>("Textures/FloorMetal");
        _renderer.WallTexture = content.Load<Texture2D>("Textures/FloorMetal");

        Transform.Scale = Vector3.One * 2f;
    }

    public void Draw(GameTime gameTime, Camera camera)
    {
        _renderer.Draw(gameTime, Transform, camera);
    }

}
