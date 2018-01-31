using System.Collections.Generic;

using SFML.Graphics;
using TiledSharp;

namespace Tiler
{
	public class TileSet
	{
		private static List<TileSet> sets = new List<TileSet>();

		public static TileSet GetTileSetForTile(int tileGID)
		{
			foreach (var set in sets)
			{
				if (tileGID >= set.FirstGID && tileGID < set.FirstGID + set.TileCount)
					return set;
			}

			return null;
		}

		public readonly string Name;
		public readonly int FirstGID;
		public readonly int TileWidth;
		public readonly int TileHeight;
		public readonly int TileCount;
		public readonly int Columns;
		public readonly Texture Texture;
		public readonly RenderStates RenderStates;

		public TileSet(TmxTileset tileSet)
		{
			Name = tileSet.Name;
			FirstGID = tileSet.FirstGid;
			TileWidth = tileSet.TileWidth;
			TileHeight = tileSet.TileHeight;
			TileCount = tileSet.TileCount ?? 0;
			Columns = tileSet.Columns ?? 0;

			// Dont add the tileset if it has no tiles
			if (TileCount <= 0)
				return;

			// Dont add the tileset if a set with the same name exists
			if (sets.Find(s => s.Name == Name) != null)
				return;

			Texture = new Texture(tileSet.Image.Source);
			RenderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, Texture, null);

			sets.Add(this);
		}
	}
}
