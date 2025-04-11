using System;

using Arch.Core;

using Fantalis.Core.Logging;

namespace Fantalis.Core.Systems;

public abstract class GameSystem
{
    private World? _world;
    
    public virtual World World
    {
        get => _world ?? throw new InvalidOperationException("World is not set.");
        set => _world = value;
    }
    
    protected readonly Logger Logger;

    public GameSystem(Logger logger)
    {
        Logger = logger;
    }
    
    public virtual void Initialize() { }
    public virtual void BeginRun() { }
    
    public virtual void BeforeUpdate(double deltaTime) { }
    public virtual void Update(double deltaTime) { }
    public virtual void AfterUpdate(double deltaTime) { }
}