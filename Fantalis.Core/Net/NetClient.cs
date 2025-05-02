using Riptide;

namespace Fantalis.Core.Net;

public class NetClient
{
    public Player Player { get; init; }
    public Connection Connection { get; init; }

    public NetClient(Player player, Connection conn)
    {
        Player = player;
        Connection = conn;
    }
}
