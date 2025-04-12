using System.Threading.Tasks;

using Fantalis.Core.Logging;

namespace Fantalis.Server.Net;

public abstract class ConnectionDataHandler
{
    protected Connection Connection { get; init; }
    protected Logger Logger { get; init; }
    
    public ConnectionDataHandler(Connection connection, Logger logger)
    {
        Connection = connection;
        Logger = logger;
    }
    
    public abstract Task HandleData(byte[] buffer, int bytesRead);
}