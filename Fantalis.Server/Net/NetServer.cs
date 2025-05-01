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
    public event EventHandler? ConnectionVerified;
    public event EventHandler? Disconnect;
    public event EventHandler<ConnectionDataEventArgs>? DataReceived;

    private bool _isRunning = false;

    private readonly HashSet<Connection> _connections = [];

    private readonly Thread _listenThread;
    private readonly Socket _listener;
    private readonly Logger _logger;
    private readonly Lock _lock = new();

    private readonly byte[] _protocolVersion;

    // TODO: Read from config
    private readonly int _timeout = 30_000;
    
    public NetServer(Logger logger)
    {
        _logger = logger;
        _listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
        _listenThread = new Thread(BeginListen);

        try
        {
            _protocolVersion = NetPacket.GetProtocolVersion();
        }
        catch (Exception e)
        {
            _logger.Log("Failed to get protocol version:");
            _logger.Log(e.ToString());
            throw;
        }
    }

    /// <summary>
    /// Starts the server on the specified port.
    /// </summary>
    /// <param name="port">The port to listen for connections on</param>
    public void Start(int port)
    {
        if (_isRunning)
        {
            return;
        }

        _logger.Log("Starting");
        _isRunning = true;

        _listenThread.Start(port);
    }

    /// <summary>
    /// Stops the server and closes all connections.
    /// </summary>
    public void Stop()
    {
        _logger.Log("Stopping...");
        _isRunning = false;

        _listener.Close();
        _listener.Dispose();
    }

    /// <summary>
    /// Begins listening for connections on the specified port.
    /// 
    /// This method is called internally by the server's listen thread.
    /// </summary>
    /// <param name="portNum">The port to listen for connections on as an int</param>
    private void BeginListen(object? portNum)
    {
        if (portNum is not int port)
        {
            _logger.Log($"Invalid port number: {portNum ?? "NULL"}");
            return;
        }

        try
        {
            _listener.Bind(new IPEndPoint(IPAddress.Any, port));
            // TODO: Read config from file
            _listener.Listen(10);

            ListenForConnections();
        }
        catch (SocketException e)
        {
            _logger.Log("Socket error:");
            _logger.Log(e.ToString());
            throw;
        }
    }
    
    private void ListenForConnections()
    {
        while (_isRunning)
        {
            try
            {
                Socket socket = _listener.Accept();
                _logger.Log($"New connection from {socket.RemoteEndPoint}:{socket.LocalEndPoint}");
                HandleNewConnection(socket);
            }
            catch (OperationCanceledException)
            {
                // Server is stopping
                break;
            }
        }
    }

    private void HandleNewConnection(Socket socket)
    {
        Connection connection = new(_logger.WithName("Connect"), socket);
        connection.Disconnected += HandleDisconnect;
        connection.DataAvailable += UnverifiedDataAvailable;

        lock (_lock)
        {
            _connections.Add(connection);
        }

        connection.StartListening();
    }

    private void UnverifiedDataAvailable(object? sender, ConnectionDataEventArgs e)
    {
        if (sender is not Connection connection)
        {
            return;
        }

        if (!connection.IsVerified)
        {
            byte[] versionBuffer = new byte[_protocolVersion.Length];
            connection.Read(ref versionBuffer, 0, e.ByteCount);
            connection.IsVerified = VerifyConnection(connection, versionBuffer);

            if (connection.IsVerified)
            {
                ConnectionVerified?.Invoke(connection, EventArgs.Empty);
            }
            else
            {
                connection.Logger.Log("Invalid protocol version, disconnecting...");
                connection.Disconnect();
            }
        }

        connection.DataAvailable -= UnverifiedDataAvailable;
        StartVerificationTimeout(connection);
    }

    private void StartVerificationTimeout(Connection connection)
    {
        _ = Task.Delay(_timeout).ContinueWith(_ =>
        {
            if (!connection.IsVerified)
            {
                connection.Logger.Log("Verification timed out");
                connection.Disconnect();
            }
        });
    }

    private bool VerifyConnection(Connection connection, byte[] data)
    {
        if (data.Length != _protocolVersion.Length)
        {
            connection.Logger.Log("Invalid protocol version length");
            return false;
        }

        for (int i = 0; i < _protocolVersion.Length; i++)
        {
            if (data[i] != _protocolVersion[i])
            {
                connection.Logger.Log("Invalid protocol version");
                return false;
            }
        }

        connection.Logger.Log("Connection verified");
        return true;
    }

    private void HandleDisconnect(object? sender, EventArgs e)
    {
        if (sender is not Connection connection)
        {
            return;
        }

        lock (_lock)
        {
            _connections.Remove(connection);
        }

        Disconnect?.Invoke(connection, e);
    }
}