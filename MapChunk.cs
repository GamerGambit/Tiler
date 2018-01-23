using System.Collections.Generic;

using SFML.Graphics;
using TiledSharp;

namespace Tiler
{
    public class MapChunk : Drawable
    {
        public List<MapLayer> Layers { get; private set; } = new List<MapLayer>();
        public void Load(string mapFile)
        {
            var map = new TmxMap(mapFile);

            foreach(var tileset in map.Tilesets)
            {
                // constructor automatically adds it to static list of TileSets
                new TileSet(tileset);
            }

            foreach (var tmxLayer in map.Layers)
            {
                /*
                if (layer.Visible == false)
                    continue;
                */

                var layer = new MapLayer();

                foreach (var tile in tmxLayer.Tiles)
                {
                    if (tile.Gid == 0)
                        continue;

                    layer.AddTile(tile);
                }

                Layers.Add(layer);
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            foreach (var layer in Layers)
            {
                target.Draw(layer, states);
            }
        }
    }
}
