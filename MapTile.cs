using SFML.System;
using TiledSharp;

namespace Tiler
{
	public class MapTile
	{
		public readonly int GID;
		public readonly bool HorizontalFlip;
		public readonly bool VerticalFlip;
		public readonly bool DiagonalFlip;
		public readonly TileSet TileSet;

		public MapTile(TmxLayerTile tile)
		{
			GID = tile.Gid;
			HorizontalFlip = tile.HorizontalFlip;
			VerticalFlip = tile.VerticalFlip;
			DiagonalFlip = tile.DiagonalFlip;
			TileSet = TileSet.GetTileSetForTile(GID);
		}
	}
}
