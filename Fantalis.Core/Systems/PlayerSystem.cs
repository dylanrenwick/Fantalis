using System.Collections.Generic;

using Arch.Core;

using Fantalis.Core.Components;
using Fantalis.Core.Logging;
using Fantalis.Core.Math;

namespace Fantalis.Core.Systems;

public class PlayerSystem : GameSystem
{
    private readonly IReadOnlyDictionary<string, Player> _players;
    private readonly QueryDescription _playerQuery;
    
    public PlayerSystem(Logger logger, IReadOnlyDictionary<string, Player> players)
        : base(logger.SubLogger(nameof(PlayerSystem)))
    {
        _players = players;
        _playerQuery = new QueryDescription()
            .WithAll<Position, PlayerFlag>();
    }

    public override void Initialize()
    {
        Logger.Log("Initializing player system...");
    }
    
    public override void Update(double deltaTime)
    {
        World.Query(
            in _playerQuery,
            (Entity _, ref Position pos, ref PlayerFlag playerFlag) =>
            {
                if (!_players.TryGetValue(playerFlag.Id, out Player? player))
                {
                    Logger.Log("Entity exists with player Id {playerFlag.Id}, but no player was found.");
                    return;
                }
                
                Vector3 position = pos.Pos;
                position += player.Input;
                pos.Pos = position;
            }
        );
    }
}