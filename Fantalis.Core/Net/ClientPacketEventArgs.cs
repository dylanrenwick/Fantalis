using System;

namespace Fantalis.Core.Net;

public class ClientPacketEventArgs : EventArgs
{
    public NetClient Client { get; }
    public NetPacket Packet { get; }

    public ClientPacketEventArgs(NetClient client, NetPacket packet)
    {
        Client = client;
        Packet = packet;
    }
}