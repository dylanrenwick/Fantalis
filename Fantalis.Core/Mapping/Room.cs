using Arch.Core;

namespace Fantalis.Core.Mapping;

/// <summary>
/// This class represents a room in the world.
/// It contains the entities in the room, and the map data.
/// 
/// It is responsible for handling its own Arch World.
/// </summary>
public class Room
{
    public required string RoomId { get; init; }
    public required MapData MapData { get; init; }
    public required TileRegistry TileRegistry { get; init; }

    public readonly World World = World.Create();
}