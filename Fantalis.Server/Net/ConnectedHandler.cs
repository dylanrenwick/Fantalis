using System.Threading.Tasks;

using Fantalis.Core.Logging;

namespace Fantalis.Server.Net;

public class ConnectedHandler : ConnectionDataHandler
{
    public ConnectedHandler(Connection connection, Logger logger)
        : base(connection, logger) { }


    public override async Task HandleData(byte[] buffer, int bytesRead)
    {
        
    }
}