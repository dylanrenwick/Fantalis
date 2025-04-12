using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Apos.Gui;

using Fantalis.Core;

namespace Fantalis.Client.Scenes;

public class GameScene : Scene
{
    private readonly GameCore _world;

    public GameScene(FantalisGame game, GameCore world) 
        : base(game)
    {
        _world = world;
    }

    public override void Start()
    {
        _world.Initialize();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {

    }

    public override void DrawGui(GameTime gameTime, IMGUI gui)
    {

    }

    public override void Update(GameTime gameTime)
    {
        _world.Update(gameTime.ElapsedGameTime.TotalSeconds);
    }
}
