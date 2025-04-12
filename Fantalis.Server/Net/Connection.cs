using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Fantalis.Core.Logging;

namespace Fantalis.Server.Net;

public class Connection
{
    private readonly Guid _id = Guid.NewGuid();
    
    private readonly Logger _logger;
    private readonly Socket _client;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    private readonly byte[] _buffer = new byte[1024];
    
    public NetworkServer Server { get; set; }
    
    public ConnectionState State { get; private set; }

    public Connection(Logger logger, NetworkServer server, Socket client)
    {
        _logger = logger.SubLogger(_id.ToString());
        _client = client;
        
        Server = server;
    }

    public async Task Listen()
    {
        try
        {
            while (State != ConnectionState.Disconnected)
            {
                if (!_client.Connected)
                {
                    await Disconnect();
                    break;
                }

                if (_client.Available == 0)
                {
                    await Task.Delay(15, _cancellationTokenSource.Token);
                    continue;
                }

                int bytesRead = await _client.ReceiveAsync(_buffer, SocketFlags.None, _cancellationTokenSource.Token);
                if (bytesRead == 0)
                {
                    await Disconnect();
                    break;
                }

                await ProcessData(bytesRead);
            }
        }
        catch (IOException)
        {
            await Disconnect();
        }
        catch (OperationCanceledException)
        {
            await Disconnect();
        }
    }

    public async Task Disconnect()
    {
        State = ConnectionState.Disconnected;
        await _cancellationTokenSource.CancelAsync();
        _client.Close();
    }

    private async Task ProcessData(int bytesRead)
    {
        ConnectionDataHandler handler = GetHandler();
        await handler.HandleData(_buffer, bytesRead);
    }

    private ConnectionDataHandler GetHandler()
    {
        return State switch
        {
            ConnectionState.Connected => new ConnectedHandler(this, _logger),
            ConnectionState.Verified => new VerifiedHandler(this, _logger),
            ConnectionState.InGame => throw new NotImplementedException(),
            ConnectionState.Disconnected => throw new InvalidOperationException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

}