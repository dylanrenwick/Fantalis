using System;
using System.IO;

namespace Fantalis.Core.Net;

public class PacketSerializer
{
    public static byte[] Serialize(NetPacket packet)
    {
        // TODO: Read max buffer size from config
        MemoryStream stream = new(new byte[1024-2]);
        stream.Position = 2; // Skip length bytes
        // TODO: Handle multi-packet serialization

        byte[] typeBytes = BitConverter.GetBytes((ushort)packet.Type);
        stream.Write(typeBytes);

        ushort dataLength = (ushort)(stream.Position - 2);
        byte[] data = stream.ToArray();
        BitConverter.GetBytes(dataLength).CopyTo(data, 0);
        return data;
    }

    public static NetPacket? Deserialize(byte[] data)
    {
        MemoryStream stream = new(data);
        ushort typeVal = BitConverter.ToUInt16(data, 0);

    }
}
