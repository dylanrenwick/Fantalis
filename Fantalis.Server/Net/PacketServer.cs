using System;
using System.Collections.Generic;
using System.Threading;

using Fantalis.Core.Logging;
using Fantalis.Core.Net;

namespace Fantalis.Server.Net;

public class PacketServer
{
    private readonly NetServer _netServer;
    private readonly Logger _logger;

    private readonly Lock _lock = new();
    private readonly HashSet<NetClient> _clients = [];

    public PacketServer(Logger logger)
    {
        _logger = logger;
        _netServer = new NetServer(_logger.WithName("Net"));
    }

    public void Start()
    {
        _logger.Log("Starting packet server...");

        _netServer.ConnectionVerified += HandleNewClient;
        // TODO: Read port from config
        _netServer.Start(8888);
    }

    private void HandleNewClient(object? sender, EventArgs e)
    {
        if (sender is not Connection connection)
        {
            _logger.Log("Invalid sender in HandleNewClient.");
            return;
        }

        connection.Logger.Log("New client connected");
        NetClient client = new(connection);

        lock (_lock)
        {
            _clients.Add(client);
        }
    }
}
