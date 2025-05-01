using System;

using Fantalis.Core.Logging;

namespace Fantalis.Core.Net;

public class NetClient
{
    public event EventHandler<ClientPacketEventArgs>? PacketReceived;

    private readonly Logger _logger;
    private readonly Connection _connection;

    public NetClient(Connection connection)
    {
        _logger = connection.Logger;
        _connection = connection;
        _connection.DataAvailable += DataAvailable;
    }

    public void SendPacket(NetPacket packet)
    {
        byte[] data = PacketSerializer.Serialize(packet);
        _connection.Send(data);
    }

    private void DataAvailable(object? _, ConnectionDataEventArgs e)
    {
        byte[] lengthBuffer = new byte[sizeof(ushort)];
        int bytesRead = _connection.Read(ref lengthBuffer);
        if (bytesRead != sizeof(ushort))
        {
            _logger.Log("Failed to read packet length.");
            // TODO: Disconnect
            return;
        }

        ushort packetLength = BitConverter.ToUInt16(lengthBuffer, 0);
        byte[] dataBuffer = new byte[packetLength];
        bytesRead = _connection.Read(ref dataBuffer);
        if (bytesRead != packetLength)
        {
            _logger.Log("Failed to read packet data.");
            // TODO: Handle multi-packet messages
            return;
        }

        NetPacket? packet = PacketSerializer.Deserialize(dataBuffer);
        if (!packet.HasValue)
        {
            _logger.Log("Failed to parse packet data.");
            // TODO: Disconnect
            return;
        }

        PacketReceived?.Invoke(this, new ClientPacketEventArgs(this, packet.Value));
    }
}
