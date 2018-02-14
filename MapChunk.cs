using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;
using TiledSharp;

namespace Tiler
{
	public class MapChunk : Drawable
	{
		private MapLayer[] Layers = new MapLayer[Enum.GetNames(typeof(MapLayer.Type)).Length];
		private List<Tuple<int, TileSet>> Tilesets = new List<Tuple<int, TileSet>>();

		public readonly IntRect Rectangle; // Position and size

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
				TileSet.AddTileSet(tileset);

				Tilesets.Add(new Tuple<int, TileSet>(tileset.FirstGid, TileSet.Sets.Find(s => s.Name == tileset.Name)));
			}
			
			foreach (var tmxLayer in map.Layers)
			{
				// Ignore layers that dont have enums
				if (!Enum.TryParse(tmxLayer.Name, true, out MapLayer.Type layerType))
					continue;

				var layer = Layers[(int)layerType];

				foreach (var tmxTile in tmxLayer.Tiles)
				{
					if (tmxTile.Gid == 0)
					{
						layer.AddTileID(-1);
						continue;
					}

					var (tilesetGID, tileset) = Tilesets.Find(s => tmxTile.Gid >= s.Item1 && tmxTile.Gid < s.Item2.TileCount + s.Item1);

					if (tileset is null)
						continue;
					
					var newTile = new MapTile(tileset, tmxTile.Gid - tilesetGID, tmxTile);

					// If the tile doesnt exist, add it to `World`
					if (World.Tiles.Find(t => t == newTile) is null)
					{
						World.Tiles.Add(newTile);
					}

					layer.AddTileID(World.Tiles.FindIndex(t => t == newTile));
				}
			}
			
			foreach (var group in map.ObjectGroups)
			{
				foreach (var obj in group.Objects)
				{
					World.SpawnEntity(obj, new Vector2i(Rectangle.Left, Rectangle.Top));
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
