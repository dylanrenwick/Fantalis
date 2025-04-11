using System;

namespace Fantalis.Server.Net;

public class ClientConnectEventArgs : EventArgs
{
    public Connection Connection { get; }

    public ClientConnectEventArgs(Connection connection)
    {
        Connection = connection;
    }
}