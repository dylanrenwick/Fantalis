using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Fantalis.Core;

namespace Fantalis.Client;

public class FantalisGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    
    private SpriteBatch? _spriteBatch;
    private GameCore? _world;

    public FantalisGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _world = GameCore.Create(@"C:\Users\User\Documents\Fantalis\Fantalis.Core\Worlds\TestWorld");
    }

    protected override void Update(GameTime gameTime)
    {
        if (_world is null)
        {
            Exit();
            return;
        }

        _world.Update(gameTime.ElapsedGameTime.TotalSeconds);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }
}
