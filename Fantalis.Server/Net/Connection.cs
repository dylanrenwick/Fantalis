using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Fantalis.Server.Net;

public class Connection
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    private readonly byte[] _buffer = new byte[1024];
    
    public ConnectionState State { get; private set; }

    public Connection(TcpClient client)
    {
        _client = client;
        _stream = client.GetStream();
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

                int bytesRead = await _stream.ReadAsync(_buffer, _cancellationTokenSource.Token);
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
        _stream.Close();
    }

    private async Task ProcessData(int bytesRead)
    {
        
    }

    public enum ConnectionState
    {
        // Connected, but no further communication
        Connected,
        // Game client version has been verified
        Verified,
        // Client has signed in
        SignedIn,
        // Client is in game
        InGame,
        // Connection has been disconnected
        Disconnected,
    }
}