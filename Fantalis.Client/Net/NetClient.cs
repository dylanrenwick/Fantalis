using Riptide;
using RiptideClient = Riptide.Client;

namespace Fantalis.Client.Net;

public class NetClient
{
    private readonly RiptideClient _client;

    public NetClient()
    {
        _client = new RiptideClient();
    }

    public void Connect(string ip, ushort port)
    {
        _client.Connect($"{ip}:{port}");
    }

    public void Disconnect()
    {
        _client.Disconnect();
    }

    public void Update()
    {
        _client.Update();
    }

    public int Send(Message message, bool release = true)
    {
        return _client.Send(message, release);
    }
}
