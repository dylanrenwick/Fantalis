using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Fantalis.Core.Logging;

namespace Fantalis.Server.Net;

public class NetworkServer
{
    public event EventHandler<ClientConnectEventArgs>? ClientConnected;
    public event EventHandler<ClientConnectEventArgs>? ClientDisconnected;

    private bool _isRunning = false;
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly List<Connection> _connections = [];
    
    private readonly Logger _logger;
    
    public NetworkServer(Logger logger)
    {
        _logger = logger;
    }
    
    public async Task Start(int port)
    {
        if (_isRunning)
        {
            return;
        }
        
        _isRunning = true;

        try
        {
            TcpListener listener = new(IPAddress.Any, port);
            listener.Start();

            _ = ListenForConnections(listener, _cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task Stop()
    {
        
    }
    
    private async Task ListenForConnections(TcpListener listener, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                TcpClient client = await listener.AcceptTcpClientAsync(token);
                _ = HandleNewConnection(client, token);
            }
            catch (OperationCanceledException)
            {
                // Server is stopping
                break;
            }
        }
    }

    private async Task HandleNewConnection(TcpClient client, CancellationToken token)
    {
        Connection connection = new(client);
        ClientConnected?.Invoke(this, new ClientConnectEventArgs(connection));
        
        await connection.Listen();
    }
}