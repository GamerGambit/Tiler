using System.Collections.Generic;

using SFML.Graphics;
using TiledSharp;

namespace Tiler
{
    public class TileSet
    {
        private static List<TileSet> sets = new List<TileSet>();

        public static TileSet GetTileSetForTile(TmxLayerTile tile)
        {
            foreach(var set in sets)
            {
                if (tile.Gid >= set.FirstGID && tile.Gid < set.FirstGID + set.TileCount)
                    return set;
            }

            return null;
        }

        public string Name { get; private set; }
        public int FirstGID { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public int TileCount { get; private set; }
        public int Columns { get; private set; }
        public Texture Texture { get; private set; }

        public TileSet(TmxTileset tileSet)
        {
            Name = tileSet.Name;
            FirstGID = tileSet.FirstGid;
            TileWidth = tileSet.TileWidth;
            TileHeight = tileSet.TileHeight;
            TileCount = tileSet.TileCount ?? 0;
            Columns = tileSet.Columns ?? 0;

            if (TileCount <= 0)
                return;

            Texture = new Texture(tileSet.Image.Source);

            sets.Add(this);
        }
    }
}
