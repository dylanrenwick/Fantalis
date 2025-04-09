using System.Collections.Generic;

namespace Fantalis.Core.Mapping;

public class MapData
{
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required int Depth { get; init; }
    
    public required int[] TileData { get; init; }
    
    public required Dictionary<int, string> TileMapping { get; init; }
    
    public string this[int x, int y, int z]
        => TileMapping[TileData[x + y * Width + z * Width * Height]];
}