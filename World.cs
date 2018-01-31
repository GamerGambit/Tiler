using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	public static class World// : Drawable
	{
		public const int MaxChunks = 100;

		private static int numChunks = 0;
		private static MapChunk[] Chunks = new MapChunk[MaxChunks];
		public static List<MapTile> Tiles { get; internal set; } = new List<MapTile>();

		public static void LoadChunk(string filename, Vector2i position)
		{
			Chunks[numChunks++] = new MapChunk(filename, position);
		}

		public static void Draw(RenderTarget target)
		{
			foreach (var chunk in Chunks)
			{
				if (chunk is null)
					continue;

				target.Draw(chunk);
			}
		}
	}
}
