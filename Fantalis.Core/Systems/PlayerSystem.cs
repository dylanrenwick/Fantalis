using System.Collections.Generic;

using Arch.Core;

using Fantalis.Core.Components;
using Fantalis.Core.Math;

namespace Fantalis.Core.Systems;

public class PlayerSystem : GameSystem
{
    private readonly IReadOnlyDictionary<string, Player> _players;
    
    public PlayerSystem(IReadOnlyDictionary<string, Player> players)
    {
        _players = players;
    }
    
    public override void Update(double deltaTime)
    {
        var playerQuery = new QueryDescription().WithAll<Position, PlayerFlag>();
        World.Query(
            in playerQuery,
            (Entity _, ref Position pos, ref PlayerFlag playerFlag) =>
            {
                if (!_players.TryGetValue(playerFlag.Id, out Player? player))
                {
                    // TODO: log warning
                    return;
                }
                
                Vector3 position = pos.Pos;
                position += player.Input;
                pos.Pos = position;
            }
        );
    }
}