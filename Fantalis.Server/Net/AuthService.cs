using System;
using System.Collections.Generic;

using Riptide;

using Fantalis.Core;
using Fantalis.Core.Logging;
using Fantalis.Core.Net;

namespace Fantalis.Server.Net;

public class AuthService
{
    public event EventHandler<PlayerAuthenticatedEventArgs>? PlayerAuthenticated;

    private readonly HashSet<ushort> _pendingConnections = [];
    private readonly Logger _logger;

    public AuthService(Logger logger)
    {
        _logger = logger;
    }

    public void Add(Connection conn)
    {
        _pendingConnections.Add(conn.Id);
    }

    public void HandleMessageReceived(object? _, MessageReceivedEventArgs e)
    {
        Connection client = e.FromConnection;
        if (_pendingConnections.Remove(client.Id))
        {
            string playerId = Guid.NewGuid().ToString();
            _logger.Log($"Client {client.Id} authenticated as player {playerId}.");
            PlayerAuthenticated?.Invoke(this, new PlayerAuthenticatedEventArgs(
                client, playerId, "room-1"
            ));
        }
    }

    public void HandleClientDisconnect(object? _, ServerDisconnectedEventArgs e)
    {
        Connection client = e.Client;
        if (_pendingConnections.Remove(client.Id))
        {
            _logger.Log($"Client {client.Id} disconnected before authentication.");
        }
    }
}

