using System;

using SFML.Graphics;
using SFML.System;
using TiledSharp;

namespace Tiler
{
	public class MapChunk : Drawable
	{
		private MapLayer[] Layers = new MapLayer[Enum.GetNames(typeof(MapLayer.Type)).Length];

		public readonly IntRect Rectangle;

		public MapChunk(string mapFile, Vector2i position)
		{
			// Initialize all layers
			for (var index = 0; index < Layers.Length; ++index)
			{
				Layers[index] = new MapLayer(this);
			}

			// Load map file
			var map = new TmxMap(mapFile);

			Rectangle = new IntRect(position, new Vector2i(map.Width, map.Height));

			// Add tilesets
			foreach (var tileset in map.Tilesets)
			{
				// constructor automatically adds it to static list of TileSets
				new TileSet(tileset);
			}
			
			foreach (var tmxLayer in map.Layers)
			{
				// Ignore layers that dont have enums
				if (MapLayer.TryParseLayerType(tmxLayer.Name, out MapLayer.Type layerType) == false)
					continue;

				var layer = Layers[(int)layerType];

				foreach (var tmxTile in tmxLayer.Tiles)
				{
					// If the tile doesnt exist, add it to `World`
					if (World.Tiles.Find(t => t.GID == tmxTile.Gid) == null)
					{
						World.Tiles.Add(new MapTile(tmxTile));
					}

					layer.AddTileID(tmxTile.Gid);
				}
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
