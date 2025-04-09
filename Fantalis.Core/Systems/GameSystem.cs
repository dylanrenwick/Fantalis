using System;
using Arch.Core;

namespace Fantalis.Core.Systems;

public abstract class GameSystem : ISystem
{
    private World? _world;
    
    public virtual World World
    {
        get => _world ?? throw new InvalidOperationException("World is not set.");
        set => _world = value;
    }
    
    public virtual void Initialize() { }
    
    public virtual void BeforeUpdate(double deltaTime) { }
    public virtual void Update(double deltaTime) { }
    public virtual void AfterUpdate(double deltaTime) { }
}