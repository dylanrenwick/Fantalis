namespace Fantalis.Core.Net;

public class ConnectionDataEventArgs : ConnectionEventArgs
{
    public int ByteCount { get; }
    public byte[] Data { get; }

    public ConnectionDataEventArgs(Connection connection, int byteCount, byte[] data)
        : base(connection)
    {
        ByteCount = byteCount;
        Data = data;
    }
}
