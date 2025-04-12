using System;
using System.Threading;
using System.Threading.Tasks;

using Fantalis.Core;
using Fantalis.Core.Logging;
using Fantalis.Server.Net;

namespace Fantalis.Server;

public class FantalisServer
{
    private const int UpdateInterval = 1000 / 20;
    
    private readonly string _rootPath;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly NetServer _networkServer;

    private Logger _logger;
    private Task? _networkTask;
    
    public FantalisServer(string rootPath, Logger defaultLogger)
    {
        _rootPath = rootPath
            ?? throw new ArgumentNullException(nameof(rootPath));

        _logger = defaultLogger;
        // TODO: Read config from rootPath
        
        _networkServer = new NetServer(_logger);
    }
    
    public async Task Start()
    {
        _logger.Log("Initializing server...");
        
        GameCore gameCore = new(_rootPath, _logger.WithName("Game"));
        gameCore.Initialize();
        _logger.Log("Game Core initialized.");

        _networkServer.ClientConnected += OnClientConnect;
        _networkServer.ClientDisconnected += OnClientDisconnect;
        _networkTask = _networkServer.Start(8888);
        _logger.Log("Network server started.");

        await RunServerLoopAsync(gameCore, _cancellationTokenSource.Token);
    }
    
    public async Task Stop()
    {
        await _cancellationTokenSource.CancelAsync();
        _cancellationTokenSource.Dispose();

        await _networkServer.Stop();
        // Wait for network server to stop
        if (_networkTask is not null)
        {
            await _networkTask;
        }
    }
    
    private async Task RunServerLoopAsync(GameCore gameCore, CancellationToken cancellationToken)
    {
        DateTime lastUpdate = DateTime.UtcNow;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            DateTime currentTime = DateTime.UtcNow;
            double deltaTime = (currentTime - lastUpdate).TotalSeconds;
            lastUpdate = currentTime;
            
            gameCore.Update(deltaTime);
            
            var elapsed = (int)(DateTime.UtcNow - currentTime).TotalMilliseconds;
            if (elapsed < UpdateInterval)
            {
                await Task.Delay(UpdateInterval - elapsed, cancellationToken);
            }
        }
    }

    private void OnClientConnect(object? _, ClientConnectEventArgs args)
    {
        // Expect that client will next send login details,
        // to be compared against SQLite database
    }

    private void OnClientDisconnect(object? _, ClientConnectEventArgs args)
    {

    }
}