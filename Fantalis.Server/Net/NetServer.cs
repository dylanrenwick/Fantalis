using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Fantalis.Core.Logging;
using Fantalis.Core.Net;

namespace Fantalis.Server.Net;

public class NetServer
{
    public event EventHandler<ConnectionEventArgs>? NewConnection;
    public event EventHandler<ConnectionEventArgs>? Disconnect;
    public event EventHandler<ConnectionDataEventArgs>? DataReceived;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly HashSet<Connection> _connections = [];
    
    private readonly Socket _listener;
    private readonly Logger _logger;
    private readonly Lock _lock;

    private bool _isRunning = false;
    
    public NetServer(Logger logger)
    {
        _logger = logger;
        _listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
    }
    
    public async Task Start(int port)
    {
        if (_isRunning)
        {
            return;
        }
        
        _logger.Log("Starting");
        _isRunning = true;

        try
        {
            _listener.Bind(new IPEndPoint(IPAddress.Any, port));
            // TODO: Read config from file
            _listener.Listen(10);

            await ListenForConnections(_cancellationTokenSource.Token);
        }
        catch (SocketException e)
        {
            _logger.Log("Socket error:");
            _logger.Log(e.ToString());
            throw;
        }
    }
    
    public async Task Stop()
    {
        _logger.Log("Stopping...");
        _isRunning = false;

        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();

        _listener.Close();
        _listener.Dispose();
    }
    
    private async Task ListenForConnections(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                Socket socket = await _listener.AcceptAsync(token);
                _logger.Log($"New connection from {socket.RemoteEndPoint}:{socket.LocalEndPoint}");
                _ = HandleNewConnection(socket, token);
            }
            catch (OperationCanceledException)
            {
                // Server is stopping
                break;
            }
        }
    }

    private async Task HandleNewConnection(Socket socket, CancellationToken token)
    {
        Connection connection = new(_logger.WithName("Connect"), socket);
        connection.Disconnected += (_, args) => HandleDisconnect(args);
        connection.DataReceived += DataReceived;

        lock (_lock)
        {
            _connections.Add(connection);
        }

        NewConnection?.Invoke(this, new ConnectionEventArgs(connection));

        await connection.StartListening(token);
    }

    private void HandleDisconnect(ConnectionEventArgs args)
    {
        lock (_lock)
        {
            _connections.Remove(args.Connection);
        }

        Disconnect?.Invoke(this, args);
    }
}