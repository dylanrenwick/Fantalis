using Riptide;

using Fantalis.Core;

namespace Fantalis.Server.Net;

public class PlayerClient
{
    public Player Player { get; init; }
    public Connection Connection { get; init; }

    public PlayerClient(Player player, Connection conn)
    {
        Player = player;
        Connection = conn;
    }
}
