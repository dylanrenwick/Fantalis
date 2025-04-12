using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Apos.Gui;

namespace Fantalis.Client.Scenes;

public abstract class Scene
{
    protected FantalisGame Game { get; }

    public Scene(FantalisGame game)
    {
        Game = game;
    }

    public virtual void Start() { }

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    public abstract void DrawGui(GameTime gameTime, IMGUI gui);
}
