using System.Collections.Generic;
using System.Linq;

using Arch.Core;

namespace Fantalis.Core.Systems;

public class SystemGroup : GameSystem
{
    public readonly string Name;
    private readonly List<GameSystem> _systems = [];

    public override World World
    {
        get => base.World;
        set
        {
            base.World = value;
            foreach (GameSystem system in _systems)
            {
                system.World = value;
            }
        }
    }

    public SystemGroup(string name, params IEnumerable<GameSystem> systems)
    {
        Name = name;

        if (systems.TryGetNonEnumeratedCount(out int count))
        {
            _systems.Capacity = count;
        }

        foreach (GameSystem system in systems)
        {
            Add(system);
        }
    }

    public SystemGroup Add(GameSystem system)
    {
        _systems.Add(system);
        return this;
    }

    public SystemGroup Add<T>()
        where T : GameSystem, new()
    {
        return Add(new T());
    }

    public override void Initialize()
    {
        foreach (GameSystem system in _systems)
        {
            system.Initialize();
        }
    }

    public void BeforeUpdate(World world, double deltaTime)
    {
        base.World = world;
        foreach (GameSystem system in _systems)
        {
            if (system is SystemGroup group)
            {
                group.BeforeUpdate(world, deltaTime);
                continue;
            }
            
            system.World = world;
            system.BeforeUpdate(deltaTime);
        }
    }
    public override void BeforeUpdate(double deltaTime)
    {
        foreach (GameSystem system in _systems)
        {
            system.BeforeUpdate(deltaTime);
        }
    }
    
    public override void Update(double deltaTime)
    {
        foreach (GameSystem system in _systems)
        {
            system.Update(deltaTime);
        }
    }

    public override void AfterUpdate(double deltaTime)
    {
        foreach (GameSystem system in _systems)
        {
            system.AfterUpdate(deltaTime);
        }
    }
}