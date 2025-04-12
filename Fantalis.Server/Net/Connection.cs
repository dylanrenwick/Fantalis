using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Fantalis.Core.Logging;

namespace Fantalis.Server.Net;

public class Connection
{
    public event EventHandler<ClientConnectEventArgs>? Disconnected;
    public event EventHandler<ClientConnectEventArgs>? StateChanged;

    private readonly Guid _id = Guid.NewGuid();
    
    private readonly Logger _logger;
    private readonly Socket _client;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    private readonly byte[] _buffer = new byte[1024];
    
    public NetServer Server { get; }

    private ConnectionState _state;
    public ConnectionState State
    {
        get => _state;
        private set
        {
            if (_state == value)
            {
                return;
            }

            _state = value;
            StateChanged?.Invoke(this, new(this));
        }
    }

    public Connection(Logger logger, NetServer server, Socket client)
    {
        _logger = logger.SubLogger(_id.ToString());
        _client = client;
        
        Server = server;
    }

    public async Task Disconnect()
    {
        _logger.Log("Disconnecting...");
        State = ConnectionState.Disconnected;
        // Shutdown to complete pending sends before cancelling
        _client.Shutdown(SocketShutdown.Both);

        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();
        _client.Close();

        Disconnected?.Invoke(this, new ClientConnectEventArgs(this));
    }

    public async Task StartListening(CancellationToken serverToken)
    {
        _ = VerificationTimeout(30);

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

        while (State != ConnectionState.Disconnected && !serverToken.IsCancellationRequested)
        {
            if (!_client.Connected)
            {
                break;
            }

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

            await ProcessData(bytesRead);
        }
    }

    private async Task ProcessData(int bytesRead)
    {
        ConnectionDataHandler handler = GetHandler();
        State = await handler.HandleData(_buffer, bytesRead);
    }

    private async Task VerificationTimeout(int seconds)
    {
        await Task.Delay(seconds * 1000);
        
        if (State == ConnectionState.Connected)
        {
            _logger.Log($"Verification timed out after {seconds} seconds. Disconnecting.");
            await Disconnect();
        }
        else _logger.Log($"Verification timer cancelled. State: {State}");
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