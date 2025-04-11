using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Fantalis.Core;
using Fantalis.Core.Logging;

namespace Fantalis.Client;

public class FantalisGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly Logger _logger;
    
    private SpriteBatch? _spriteBatch;
    private GameCore _world;

    public FantalisGame(Logger logger)
    {
        _graphics = new GraphicsDeviceManager(this);
        _logger = logger;

        _world = new GameCore(".", _logger.WithName("Game"));
        
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _world.Initialize();
        
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void BeginRun()
    {
        _world.BeginRun();
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
