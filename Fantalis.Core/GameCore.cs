using System.Collections.Generic;
using System.Linq;

using Fantalis.Core.Logging;
using Fantalis.Core.Mapping;
using Fantalis.Core.Systems;

namespace Fantalis.Core;

public class GameCore
{
    private readonly string _rootFilePath;

    private readonly Logger _logger;
    private readonly SystemGroup _systems;
    private readonly Dictionary<string, Room> _rooms;

    private readonly Dictionary<string, Player> _players;

    public GameCore(string rootPath, Logger logger)
    {
        _rootFilePath = rootPath;
        _logger = logger;
        
        _players = [];
        _rooms = [];

        _systems = new SystemGroup(
            logger,
            "root",
            new PlayerSystem(logger, _players)
        );
    }

    public void Initialize()
    {
        _logger.Log("Initializing game core...");

        RoomLoader loader = new(_logger.WithName("Loader"), _rootFilePath);
        List<Room>? rooms = loader.LoadRoomData();
        if (rooms is null)
        {
            _logger.Log("Failed to load room data.");
            throw new System.Exception("Failed to load room data.");
        }
        _logger.Log($"Loaded {rooms.Count} rooms.");
        
        _logger.Log("Initializing systems...");
        _systems.Initialize();
    }

    public void BeginRun()
    {
        _systems.BeginRun();
    }

    public void Update(double deltaTime)
    {
        var rooms = _players.Values
            .Select(p => p.RoomId)
            .Distinct()
            .Select(id => _rooms.GetValueOrDefault(id))
            .Cast<Room>();

        foreach (Room room in rooms)
        {
            _systems.BeforeUpdate(room.World, deltaTime);
            _systems.Update(deltaTime);
            _systems.AfterUpdate(deltaTime);
        }
    }

    /// <summary>
    /// Creates a new Player object and adds it to the game core.
    ///
    /// The server will load this from the database each time a new connection logs in.
    /// The client will fetch these values from the server.
    /// </summary>
    /// <param name="playerId"></param>
    /// <param name="roomId"></param>
    /// <returns></returns>
    public Player? AddPlayer(string playerId, string roomId)
    {
        if (_players.ContainsKey(playerId))
        {
            // TODO: Log a warning
            return null;
        }
        if (!_rooms.ContainsKey(roomId))
        {
            // TODO: Log a warning
            return null;
        }

        Player player = new(playerId, roomId);
        _players.Add(playerId, player);
        return player;
    }
}
