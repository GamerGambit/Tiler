using System;

using TiledSharp;

namespace Tiler
{
	public class MapTile
	{
		public static bool operator==(MapTile left, MapTile right)
		{
			//if (ReferenceEquals(left, right))
				//return true;

			if (left is null || right is null)
				return false;

			return left.TileSet == right.TileSet &&
				left.Index == right.Index &&
				left.HorizontalFlip == right.HorizontalFlip &&
				left.VerticalFlip == right.VerticalFlip &&
				left.DiagonalFlip == right.DiagonalFlip;
		}

		public static bool operator!=(MapTile left, MapTile right)
		{
			return !(left == right);
		}

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
			if (obj is null)
				return false;

			var other = obj as MapTile;

			if (other is null)
				return false;

			return other.TileSet == TileSet &&
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
