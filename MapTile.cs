using System;
using TiledSharp;

namespace Tiler
{
	public class MapTile
	{
		public readonly int Index;
		public readonly bool HorizontalFlip;
		public readonly bool VerticalFlip;
		public readonly bool DiagonalFlip;
		public readonly TileSet TileSet;

		public MapTile(TileSet tileSet, int index, TmxLayerTile tile)
		{
			Index = index;
			HorizontalFlip = tile.HorizontalFlip;
			VerticalFlip = tile.VerticalFlip;
			DiagonalFlip = tile.DiagonalFlip;
			TileSet = tileSet;
		}

		public override bool Equals(object obj)
		{
			var other = obj as MapTile;

			if (other is null)
				return false;

			return
				other.TileSet == TileSet &&
				other.Index == Index &&
				other.HorizontalFlip == HorizontalFlip &&
				other.VerticalFlip == VerticalFlip &&
				other.DiagonalFlip == DiagonalFlip;
		}

		public override int GetHashCode()
		{
			return ValueTuple.Create(Index, HorizontalFlip, VerticalFlip, DiagonalFlip, TileSet).GetHashCode();
		}
	}
}
