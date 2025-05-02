using System;
using System.Collections.Generic;

using Riptide;

using Fantalis.Core;
using Fantalis.Core.Logging;
using Fantalis.Server.Net;

namespace Fantalis.Server;

public class GameServer
{
    private readonly Logger _logger;
    private readonly GameCore _game;

    private readonly Dictionary<ushort, PlayerClient> _clients = [];

    public GameServer(string rootPath, Logger logger)
    {
        _logger = logger;
        _game = new GameCore(rootPath, _logger.SubLogger("Core"));
    }

    public void Start()
    {
        _game.Initialize();
    }

    public void Update(double deltaTime)
    {
        _game.Update(deltaTime);
    }

    public void HandleNewPlayer(object? _, PlayerAuthenticatedEventArgs e)
    {
        _logger.Log($"Connection {e.Client.Id} connected as {e.PlayerId} in {e.RoomId}");
        Player? player = _game.AddPlayer(e.PlayerId, e.RoomId);
        if (player is null)
        {
            _logger.Log($"Failed to add client to game world.");

            return;
        }

        PlayerClient client = new(player, e.Client);
        _clients.Add(e.Client.Id, client);
    }

    public void HandleClientDisconnect(object? _, ServerDisconnectedEventArgs e)
    {
        if (_clients.Remove(e.Client.Id))
        {
            _logger.Log($"Client {e.Client.Id} disconnected.");
        }
    }

    public void HandleMessageReceived(object? _, MessageReceivedEventArgs e)
    {
        if (!_clients.ContainsKey(e.FromConnection.Id))
        {
            return;
        }

        throw new NotImplementedException();
    }
}
