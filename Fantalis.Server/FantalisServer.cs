using System;
using System.Threading;

using Riptide;

using Fantalis.Core.Logging;
using Fantalis.Server.Net;

namespace Fantalis.Server;

public class FantalisServer
{
    private const int UpdateInterval = 1000 / 20;

    private readonly NetServer _networkServer;
    private readonly AuthService _authService;
    private readonly GameServer _gameServer;
    private readonly Thread _serverThread;
    private readonly Logger _logger;

    private bool _isRunning = false;

    public FantalisServer(string rootPath, Logger defaultLogger)
    {
        _logger = defaultLogger;

        _networkServer = new NetServer(_logger.WithName("Net"));
        _authService = new AuthService(_logger.WithName("Auth"));
        _gameServer = new GameServer(rootPath, _logger.WithName("Game"));

        _serverThread = new Thread(RunServerLoop);
    }
    
    public void Start()
    {
        _logger.Log("Initializing server...");
        _isRunning = true;

        _gameServer.Start();
        _logger.Log("Game Core initialized.");

        _networkServer.ConnectionVerified += HandleNewConnection;
        _authService.PlayerAuthenticated += _gameServer.HandleNewPlayer;

        _networkServer.MessageReceived += _authService.HandleMessageReceived;
        _networkServer.MessageReceived += _gameServer.HandleMessageReceived;
        _networkServer.ClientDisconnected += _authService.HandleClientDisconnect;
        _networkServer.ClientDisconnected += _gameServer.HandleClientDisconnect;

        // TODO: Read config from rootPath
        _networkServer.Start(8888);
        _logger.Log("Network server started.");

        _serverThread.Start();
    }

    public void Stop()
    {
        _logger.Log("Stopping server...");
        _isRunning = false;

        _networkServer.Stop();
        _serverThread.Interrupt();

        _logger.Log("Server stopped. Goodbye.");
    }
    
    private void RunServerLoop()
    {
        DateTime lastUpdate = DateTime.UtcNow;

        while (_isRunning)
        {
            try
            {
                DateTime currentTime = DateTime.UtcNow;
                double deltaTime = (currentTime - lastUpdate).TotalSeconds;
                lastUpdate = currentTime;

                Update(deltaTime);

                var elapsed = (int)(DateTime.UtcNow - currentTime).TotalMilliseconds;
                if (elapsed < UpdateInterval)
                {
                    Thread.Sleep(UpdateInterval - elapsed);
                }
            }
            catch (ThreadInterruptedException)
            {
                // Thread was interrupted, exit the loop
                break;
            }
        }
    }

    private void Update(double deltaTime)
    {
        _networkServer.Update();
        _gameServer.Update(deltaTime);
    }

    private void HandleNewConnection(object? _, ServerConnectedEventArgs e)
    {
        _authService.Add(e.Client);
    }
}