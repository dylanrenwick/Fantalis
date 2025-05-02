using System;

using Riptide;

namespace Fantalis.Server.Net;

public class PlayerAuthenticatedEventArgs : EventArgs
{
    public readonly Connection Client;
    public readonly string PlayerId;
    public readonly string RoomId;

    public PlayerAuthenticatedEventArgs(Connection connection, string playerId, string roomId)
    {
        Client = connection;
        PlayerId = playerId;
        RoomId = roomId;
    }
}