namespace Fantalis.Server.Net;

public enum ConnectionState
{
    // Connected, but no further communication
    Connected,
    // Game client version has been verified
    Verified,
    // Client is in game
    InGame,
    // Connection has been disconnected
    Disconnected,
}