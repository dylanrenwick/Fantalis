using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Apos.Gui;
using FontStashSharp;

using Fantalis.Core;
using Fantalis.Core.Logging;

namespace Fantalis.Client;

public class FantalisGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly Logger _logger;
    
    private SpriteBatch? _spriteBatch;
    private IMGUI? _ui;
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
        FontSystem fontSystem = new();
        fontSystem.AddFont(TitleContainer.OpenStream($"{Content.RootDirectory}/BitPotion.ttf"));

        GuiHelper.Setup(this, fontSystem);
        _ui = new IMGUI();
        GuiHelper.CurrentIMGUI = _ui;

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _world.Initialize();
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

        _ui!.UpdateStart(gameTime);

        

        _ui!.UpdateEnd(gameTime);
        GuiHelper.UpdateCleanup();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _ui!.Draw(gameTime);

        base.Draw(gameTime);
    }
}
