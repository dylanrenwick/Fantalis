using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Riptide;
using Riptide.Utils;
using RiptideServer = Riptide.Server;

using Fantalis.Core.Logging;
using Fantalis.Core.Net;

namespace Fantalis.Server.Net;

public class NetServer
{
    public event EventHandler<ServerConnectedEventArgs>? ConnectionVerified;
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    public event EventHandler<ServerDisconnectedEventArgs>? ClientDisconnected;

    private readonly Logger _logger;
    private readonly RiptideServer _networkServer = new();
    private readonly HashSet<ushort> _pendingConnections = [];

    private readonly byte[] _protocolVersion;
    private readonly Message _handshakeMessage;

    public NetServer(Logger logger)
    {
        _logger = logger;
        RiptideLogger.Initialize(_logger.WithName("Riptide").Log, false);

        _protocolVersion = NetPacket.GetProtocolVersion();
        _handshakeMessage = Message.Create(MessageSendMode.Reliable, 1);
        _handshakeMessage.AddBytes(_protocolVersion, false);
    }

    public void Start(ushort port)
    {
        _networkServer.ClientConnected += HandleNewClient;
        _networkServer.MessageReceived += HandleMessageReceived;
        _networkServer.ClientDisconnected += HandleClientDisconnect;

        _networkServer.Start(port, 1000, 0, false);
    }

    public void Update()
    {
        _networkServer.Update();
    }

    public void Stop()
    {
        _networkServer.Stop();
    }

    private void HandleNewClient(object? _, ServerConnectedEventArgs e)
    {
        Connection client = e.Client;

        _pendingConnections.Add(client.Id);
        StartConnectionTimeout(client.Id);
    }

    private void HandleMessageReceived(object? _, MessageReceivedEventArgs e)
    {
        ushort connId = e.FromConnection.Id;
        if (_pendingConnections.Remove(connId))
        {
            if (VerifyHandshake(e.Message, connId))
            {
                _logger.Log($"Connection {connId} sent valid handshake.");
                _networkServer.Send(_handshakeMessage, connId, false);
                ConnectionVerified?.Invoke(this, new ServerConnectedEventArgs(e.FromConnection));
            }
            else
            {
                _logger.Log($"Connection {connId} sent invalid handshake.");
                _networkServer.DisconnectClient(connId);
            }
        }
        else
        {
            // Bubble up event for verified connections
            MessageReceived?.Invoke(this, e);
        }
    }

    private void HandleClientDisconnect(object? _, ServerDisconnectedEventArgs e)
    {
        ushort connId = e.Client.Id;
        if (_pendingConnections.Remove(connId))
        {
            _logger.Log($"Connection {connId} disconnected before handshake.");
        }
        else
        {
            // Bubble up event for verified connections
            ClientDisconnected?.Invoke(this, e);
        }
    }

    private void StartConnectionTimeout(ushort connId)
    {
        _ = Task.Delay(30_000).ContinueWith(_ =>
        {
            if (_pendingConnections.Remove(connId))
            {
                _logger.Log($"Connection {connId} timed out without sending handshake.");
                _networkServer.DisconnectClient(connId);
            }
        });
    }

    private bool VerifyHandshake(Message msg, ushort connId)
    {
        byte[] bytes = msg.GetBytes(_protocolVersion.Length);

        if (bytes.Length != _protocolVersion.Length)
        {
            _logger.Log($"Invalid handshake from client {connId}: {BitConverter.ToString(bytes)}");
            return false;
        }
        
        for (int i = 0; i < _protocolVersion.Length; i++)
        {
            if (bytes[i] != _protocolVersion[i])
            {
                _logger.Log($"Invalid protocol version from client {connId}: {BitConverter.ToString(bytes)}");
                return false;
            }
        }

        return true;
    }
}