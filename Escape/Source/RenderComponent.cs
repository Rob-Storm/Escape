using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Escape;

public abstract class RenderComponent
{
    public Texture2D Texture { get; set; }

    protected GraphicsDevice _device;
    protected BasicEffect _basicEffect;

    public RenderComponent(GraphicsDevice device)
    {
        _device = device;
        _basicEffect = new BasicEffect(device);
    }

    public virtual void Update(GameTime gameTime)
    {

    }

    public abstract void Draw(GameTime gameTime, Transform transform, Camera camera);
}
