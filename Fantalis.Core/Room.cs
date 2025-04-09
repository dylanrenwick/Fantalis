using System;
using System.Text.Json.Serialization;
using Arch.Core;
using Fantalis.Core.Mapping;

namespace Fantalis.Core;

/// <summary>
/// This class represents a room in the world.
/// It contains the entities in the room, and the map data.
/// 
/// It is responsible for handling its own Arch World.
/// </summary>
[Serializable]
public class Room
{
    public required string RoomId { get; init; }
    public required MapData MapData { get; init; }
    public required TileRegistry TileRegistry { get; init; }

    [JsonIgnore]
    public readonly World World = World.Create();
}