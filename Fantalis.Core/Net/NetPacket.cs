using System;
using System.Reflection;
using System.Text;

namespace Fantalis.Core.Net;

public struct NetPacket
{
    public static byte[] GetProtocolVersion()
    {
        // Fetch AssemblyName of Fantalis.Core
        Type packetType = typeof(NetPacket);
        Assembly coreAssembly = packetType.Assembly;
        AssemblyName assemblyName = coreAssembly.GetName();
        
        // Obtain bytes of version and public key
        var version = assemblyName.Version!.ToString();
        byte[] versionBytes = Encoding.ASCII.GetBytes(version);
        byte[] tokenBytes = assemblyName.GetPublicKeyToken()!;
        
        var bytes = new byte[versionBytes.Length + tokenBytes.Length];
        versionBytes.CopyTo(bytes, 0);
        tokenBytes.CopyTo(bytes, versionBytes.Length);

        return bytes;
    }
}