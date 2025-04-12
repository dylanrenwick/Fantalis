using System.Threading.Tasks;

using Fantalis.Core.Logging;

namespace Fantalis.Server.Net;

public class VerifiedHandler : ConnectionDataHandler
{
    public VerifiedHandler(Connection connection, Logger logger)
        : base(connection, logger) { }


    public override async Task<ConnectionState> HandleData(byte[] buffer, int bytesRead)
    {
        return ConnectionState.Verified;
    }
}