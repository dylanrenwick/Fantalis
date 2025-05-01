using System;

namespace Fantalis.Core.Net;

public class ConnectionEventArgs : EventArgs
{
    public Connection Connection { get; }

    public ConnectionEventArgs(Connection connection)
    {
        Connection = connection;
    }
}