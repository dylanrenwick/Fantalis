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
    public event EventHandler<ClientConnectEventArgs>? ClientConnected;
    public event EventHandler<ClientConnectEventArgs>? ClientDisconnected;

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly List<Connection> _connections = [];
    
    private readonly Logger _logger;

    private bool _isRunning = false;
    private TcpListener? _listener;

    public byte[] ProtocolVersion { get; private set; } = [];
    
    public NetServer(Logger logger)
    {
        _logger = logger;
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
            ProtocolVersion = NetPacket.GetProtocolVersion();

            _listener = new(IPAddress.Any, port);
            _listener.Start();

            await ListenForConnections(_cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task Stop()
    {
        _logger.Log("Stopping...");
        _isRunning = false;

        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();

        _listener?.Stop();
        _listener?.Dispose();
    }
    
    private async Task ListenForConnections(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                Socket client = await _listener!.AcceptSocketAsync(token);
                _logger.Log($"New connection from {client.RemoteEndPoint}:{client.LocalEndPoint}");
                _ = HandleNewConnection(client, token);
            }
            catch (OperationCanceledException)
            {
                // Server is stopping
                break;
            }
        }
    }

    private async Task HandleNewConnection(Socket client, CancellationToken token)
    {
        Connection connection = new(_logger.WithName("Connect"), this, client);
        connection.Disconnected += (_, args) => ClientDisconnected?.Invoke(this, args);
        connection.StateChanged += OnConnectionStateChange;

        await connection.StartListening(token);
    }

    private void OnConnectionStateChange(object? _, ClientConnectEventArgs args)
    {
        Connection conn = args.Connection;
        if (conn.State == ConnectionState.Verified)
        {
            ClientConnected?.Invoke(this, args);
        }
    }
}