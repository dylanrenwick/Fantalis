using System;
using System.Net.Sockets;
using System.Threading;

using Fantalis.Core.Logging;

namespace Fantalis.Core.Net;

public class Connection
{
    public event EventHandler? Disconnected;
    public event EventHandler<ConnectionDataEventArgs>? DataAvailable;

    public bool IsVerified { get; set; } = false;

    public readonly Guid Id = Guid.NewGuid();
    
    public readonly Logger Logger;

    private readonly Socket _client;
    private readonly Thread _thread;

    public Connection(Logger logger, Socket client)
    {
        Logger = logger.SubLogger(Id.ToString());
        _client = client;
        _thread = new Thread(BeginListen)
        {
            IsBackground = true,
            Name = $"Connection {Id}"
        };
    }

    public void Disconnect()
    {
        if (!_client.Connected)
        {
            return;
        }

        Logger.Log("Disconnecting...");
        // Shutdown to complete pending sends before cancelling
        _client.Shutdown(SocketShutdown.Both);
        _client.Close();

        _thread.Interrupt();

        Disconnected?.Invoke(this, EventArgs.Empty);
    }

    public void StartListening()
    {
        Logger.Log("Starting to listen for data...");
        _thread.Start();
    }

    public void Send(byte[] data)
    {
        if (!_client.Connected)
        {
            return;
        }

        try
        {
            _client.Send(data);
        }
        catch (SocketException e)
        {
            Logger.Log($"Socket error: {e.SocketErrorCode} {e.Message}");
            Disconnect();
        }
    }
    
    public int Read(ref byte[] buffer)
        => Read(ref buffer, 0, buffer.Length);
    public int Read(ref byte[] buffer, int offset, int count)
    {
        if (!_client.Connected)
        {
            throw new InvalidOperationException("Client is not connected.");
        }

        try
        {
            int bytesRead = _client.Receive(buffer, offset, count, SocketFlags.None);
            if (bytesRead == 0)
            {
                Logger.Log("Received 0 bytes on read, closing connection.");
                Disconnect();
                return 0;
            }

            return bytesRead;
        }
        catch (SocketException e)
        {
            Logger.Log($"Socket error: {e.SocketErrorCode} {e.Message}");
            Disconnect();
            return 0;
        }
    }

    private void BeginListen()
    {
        try
        {
            ListenForData();
        }
        catch (SocketException e)
        {
            Logger.Log($"Socket error: {e.SocketErrorCode} {e.Message}");
        }
        catch (ThreadInterruptedException)
        {
            Logger.Log("Thread is cancelling.");
        }
        finally
        {
            Disconnect();
        }
    }

    private void ListenForData()
    {
        while (_client.Connected)
        {
            if (_client.Available > 0)
            {
                DataAvailable?.Invoke(this, new ConnectionDataEventArgs(_client.Available));
            }

            Thread.Sleep(15);
        }
    }
}