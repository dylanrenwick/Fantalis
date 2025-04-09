using System.Collections.Generic;
using System.Linq;

namespace Fantalis.Core.Mapping;

public class TileRegistry
{
    private readonly Dictionary<string, TileSet> _tileSets;

    public TileRegistry(params IEnumerable<TileSet> tileSets)
        : this(tileSets.ToDictionary(set => set.Name)) { }
    public TileRegistry(Dictionary<string, TileSet> tileSets)
    {
        _tileSets = tileSets;
    }

    public Tile? GetTile(string name)
    {
        string[] nameParts = name.Split(':');
        if (nameParts.Length != 2 || !_tileSets.TryGetValue(nameParts[0], out TileSet? tileSet))
        {
            return null;
        }

        return tileSet.Tiles.GetValueOrDefault(nameParts[1]);
    }
}