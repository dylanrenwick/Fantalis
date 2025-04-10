using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Fantalis.Core.Systems;

namespace Fantalis.Core;

public class GameCore
{
    private readonly string _rootFilePath;

    private readonly SystemGroup _systems;
    private readonly ReadOnlyDictionary<string, Room> _rooms;

    private readonly Dictionary<string, Player> _players;

    /// <summary>
    /// Internal constructor. Use GameCore.Create to instantiate.
    /// </summary>
    /// <param name="rootPath"></param>
    /// <param name="rooms"></param>
    private GameCore(string rootPath, IEnumerable<Room> rooms)
    {
        _rootFilePath = rootPath;
        
        _rooms = new ReadOnlyDictionary<string, Room>(
            rooms.ToDictionary(r => r.RoomId)
        );

        _players = [];

        _systems = new SystemGroup(
            "root",
            new PlayerSystem(_players)
        );
    }

    public void Initialize()
    {
        // Initialize the game core here
        _systems.Initialize();
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

    public static GameCore Create(string rootPath)
    {
        // TODO: Read rooms from file
        List<Room> rooms = [];
        return new(rootPath, rooms);
    }
}
