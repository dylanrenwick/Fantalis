using System;

namespace Fantalis.Core.Net;

public class ConnectionDataEventArgs : EventArgs
{
    public int ByteCount { get; }

    public ConnectionDataEventArgs(int byteCount)
    {
        ByteCount = byteCount;
    }
}
