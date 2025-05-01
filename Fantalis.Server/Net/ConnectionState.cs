using Fantalis.Core.Net;

namespace Fantalis.Server.Net;

public class ConnectionState
{
    public required Connection Connection { get; init; }
    public bool IsVerified { get; set; }
}