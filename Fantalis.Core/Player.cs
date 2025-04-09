using Fantalis.Core.Math;

namespace Fantalis.Core;

/// <summary>
/// Provides an interface to control the player.
/// 
/// The client will use this class to pass user inputs to the world,
/// and will sync the player state with the server.
/// 
/// The server will use this class to control the player entity,
/// and will sync the player state back to the client.
/// </summary>
public class Player
{
    public string PlayerId { get; internal set; }
    public string RoomId { get; internal set; }

    public Vector2 Input { get; set; } = Vector2.ZERO;

    public Player(string playerId, string roomId)
    {
        PlayerId = playerId;
        RoomId = roomId;
    }
}