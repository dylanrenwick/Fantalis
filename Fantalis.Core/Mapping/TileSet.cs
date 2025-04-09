using System;
using System.Collections.Generic;

namespace Fantalis.Core.Mapping;

[Serializable]
public class TileSet
{
    public required string Name { get; init; }

    public required Dictionary<string, Tile> Tiles { get; init; }
}