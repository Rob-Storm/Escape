using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Escape;

public class Escape : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Camera _camera = new Camera();
    Entity entity = new Entity();
    Tile tile = new Tile();

    public Escape()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Escape";

        _graphics.PreferredBackBufferWidth = 1920;
        _graphics.PreferredBackBufferHeight = 1080;

        _graphics.ToggleFullScreen();
    }
    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _camera.LoadContent(Content, GraphicsDevice);
        entity.LoadContent(Content, GraphicsDevice);
        tile.LoadContent(Content, GraphicsDevice);

        _camera.Transform.Position = new Vector3(0f, 0.85f, 1f);
        entity.Transform.Position = new Vector3(0f, 0.5f, 0f);
        tile.Transform.Translate(Vector3.Up * 1.5f);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        entity.Update(gameTime);
        _camera.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        tile.Draw(gameTime, _camera);
        entity.Draw(gameTime, _camera);

        base.Draw(gameTime);
    }
}
