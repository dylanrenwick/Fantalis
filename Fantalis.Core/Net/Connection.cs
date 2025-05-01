using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Fantalis.Core.Logging;

namespace Fantalis.Core.Net;

public class Connection
{
    public event EventHandler<ConnectionEventArgs>? Disconnected;
    public event EventHandler<ConnectionDataEventArgs>? DataReceived;

    private readonly Guid _id = Guid.NewGuid();
    
    private readonly Logger _logger;
    private readonly Socket _client;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    private readonly byte[] _buffer = new byte[1024];

    public Connection(Logger logger, Socket client)
    {
        _logger = logger.SubLogger(_id.ToString());
        _client = client;
    }

    public async Task Disconnect()
    {
        _logger.Log("Disconnecting...");
        // Shutdown to complete pending sends before cancelling
        _client.Shutdown(SocketShutdown.Both);

        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();
        _client.Close();

        Disconnected?.Invoke(this, new ConnectionEventArgs(this));
    }

    public async Task StartListening(CancellationToken serverToken)
    {
        try
        {
            await ListenForData(serverToken);
        }
        catch (SocketException e)
        {
            _logger.Log($"Socket error: {e.SocketErrorCode} {e.Message}");
        }
        catch (IOException e)
        {
            _logger.Log($"IO error: {e.Message}");
        }
        catch (OperationCanceledException)
        {
            _logger.Log("Thread is cancelling.");
        }
        finally
        {
            await Disconnect();
        }
    }

    private async Task ListenForData(CancellationToken serverToken)
    {
        CancellationToken token = _cancellationTokenSource.Token;

        while (_client.Connected && !serverToken.IsCancellationRequested)
        {
            if (_client.Available == 0)
            {
                await Task.Delay(15, token);
                continue;
            }

            int bytesRead = await _client.ReceiveAsync(_buffer, SocketFlags.None, token);
            if (bytesRead == 0)
            {
                break;
            }

            _logger.Log($"Received {bytesRead} bytes");

            DataReceived?.Invoke(this, new ConnectionDataEventArgs(this, bytesRead, _buffer));
        }
    }
}