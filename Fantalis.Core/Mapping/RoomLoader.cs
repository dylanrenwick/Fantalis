using Fantalis.Core.Logging;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Fantalis.Core.Mapping;

public class RoomLoader
{
    private readonly Logger _logger;
    private readonly string _rootPath;

    public RoomLoader(Logger logger, string rootPath)
    {
        _logger = logger;
        _rootPath = rootPath;
    }

    public List<Room>? LoadRoomData()
    {
        TileRegistry? registry = LoadTileRegistry();
        if (registry is null)
        {
            return null;
        }

        List<Room>? roomData = LoadMapData(registry);
        if (roomData is null)
        {
            return null;
        }

        return roomData;
    }

    private TileRegistry? LoadTileRegistry()
    {
        List<TileSet> tileSets = [];
        string tilesPath = Path.Combine(_rootPath, "tiles");

        if (!Directory.Exists(tilesPath))
        {
            _logger.Log($"Tiles directory not found: {tilesPath}");
            return null;
        }

        foreach (string file in Directory.GetFiles(tilesPath, "*.json"))
        {
            string jsonContent = File.ReadAllText(file);
            TileSet? tileSet = JsonSerializer.Deserialize<TileSet>(jsonContent);
            if (tileSet is null)
            {
                _logger.Log($"Failed to load tile set from {file}");
                continue;
            }
            tileSets.Add(tileSet);
        }

        return new(tileSets);
    }

    private List<Room>? LoadMapData(TileRegistry registry)
    {
        string roomsPath = Path.Combine(_rootPath, "rooms");

        if (!Directory.Exists(roomsPath))
        {
            _logger.Log($"Rooms directory not found: {roomsPath}");
            return null;
        }

        List<Room> rooms = [];

        foreach (string file in Directory.GetFiles(roomsPath, "*.json"))
        {
            string jsonContent = File.ReadAllText(file);
            MapData? mapData = JsonSerializer.Deserialize<MapData>(jsonContent);
            if (mapData is null)
            {
                _logger.Log($"Failed to load map data from {file}");
                continue;
            }
            string fileName = Path.GetFileNameWithoutExtension(file);
            rooms.Add(new Room()
            {
                MapData = mapData,
                RoomId = fileName,
                TileRegistry = registry
            });
        }

        return rooms;
    }
}
