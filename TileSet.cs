using System;
using System.Collections.Generic;

using SFML.Graphics;
using TiledSharp;

namespace Tiler
{
	public class TileSet
	{
		public static readonly List<TileSet> Sets = new List<TileSet>();

		public static void AddTileSet(TmxTileset tileset)
		{
			if (tileset.TileCount <= 0)
				return;
			
			if (Sets.Find(s => s.Name == tileset.Name) is null)
			{
				Sets.Add(new TileSet(tileset));
			}
		}

		public readonly string Name;
		public readonly int TileWidth;
		public readonly int TileHeight;
		public readonly int TileCount;
		public readonly int Columns;
		public readonly Texture Texture;
		public readonly RenderStates RenderStates;

		private TileSet(TmxTileset tileSet)
		{
			Name = tileSet.Name;
			TileWidth = tileSet.TileWidth;
			TileHeight = tileSet.TileHeight;
			TileCount = tileSet.TileCount ?? 0;
			Columns = tileSet.Columns ?? 0;

			Texture = new Texture(tileSet.Image.Source);
			RenderStates = new RenderStates(BlendMode.Alpha, Transform.Identity, Texture, null);
		}

		public override bool Equals(object obj)
		{
			var other = obj as TileSet;

			if (other is null)
				return false;

			return other.Name == Name;
		}

		public override int GetHashCode()
		{
			return ValueTuple.Create(Name, TileWidth, TileHeight, TileCount, Columns).GetHashCode();
		}
	}
}
