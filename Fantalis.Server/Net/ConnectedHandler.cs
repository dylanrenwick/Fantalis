using System.Threading.Tasks;

using Fantalis.Core.Logging;

namespace Fantalis.Server.Net;

public class ConnectedHandler : ConnectionDataHandler
{
    public ConnectedHandler(Connection connection, Logger logger)
        : base(connection, logger) { }

    public override async Task<ConnectionState> HandleData(byte[] buffer, int bytesRead)
    {
        byte[] protocolVersion = Connection.Server.ProtocolVersion;
        if (bytesRead != protocolVersion.Length)
        {
            Logger.Log("Protocol version length mismatch");
            return ConnectionState.Disconnected;
        }

        for (int i = 0; i < bytesRead; i++)
        {
            if (buffer[i] != protocolVersion[i])
            {
                Logger.Log($"Protocol version mismatch at index {i}");
                return ConnectionState.Disconnected;
            }
        }

        // Version was a match, connection is verified.
        Logger.Log($"Protocol version verified.");
        return ConnectionState.Verified;
    }
}