using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;
using TiledSharp;

namespace Tiler
{
	public class MapLayer : Drawable
	{
		public enum Type : int
		{
			Space,
			Floors,
			Walls
		}

		public static bool TryParseLayerType(string layerName, out Type output)
		{
			return Enum.TryParse(layerName, true, out output);
		}

		private bool dirty = true;
		private MapChunk chunk;
		private List<Tuple<VertexArray, RenderStates>> Meshes = new List<Tuple<VertexArray, RenderStates>>();
		private List<int> TileIDs = new List<int>();

		private void Rebuild()
		{
			Meshes.Clear();

			for (var index = 0; index < TileIDs.Count; ++index)
			{
				var tileID = TileIDs[index];
				var tile = World.Tiles.Find(t => t.GID == tileID);

				if (tile is null || tile.GID == 0)
					continue;

				var tileset = tile.TileSet;
				var mesh = Meshes.Find(t => t.Item2.Texture == tile.TileSet.Texture);

				if (mesh is null)
				{
					mesh = new Tuple<VertexArray, RenderStates>(new VertexArray(PrimitiveType.Quads), tile.TileSet.RenderStates);
					Meshes.Add(mesh);
				}
				
				var tilePosition = new Vector2i(index % chunk.Rectangle.Width, index / chunk.Rectangle.Height);
				var spriteIndex = tile.GID - tile.TileSet.FirstGID;
				var worldPos = new Vector2f(tilePosition.X * tileset.TileWidth, tilePosition.Y * tileset.TileHeight); // For some stupid reason, SFML doesnt provide vector multiplication even between vectors of the same types
				worldPos += new Vector2f(chunk.Rectangle.Left, chunk.Rectangle.Top); // Offset the Tile's world position by the Chunk's position
				var texCoords = new Vector2f((spriteIndex % tileset.Columns) * tileset.TileWidth, (spriteIndex / tileset.Columns) * tileset.TileHeight);

				var vertexArray = mesh.Item1;

				// Top Left
				vertexArray.Append(new Vertex()
				{
					Color = Color.White,
					Position = worldPos,
					TexCoords = texCoords
				});

				// Top Right
				vertexArray.Append(new Vertex()
				{
					Color = Color.White,
					Position = worldPos + new Vector2f(tileset.TileWidth, 0),
					TexCoords = texCoords + new Vector2f(tileset.TileWidth, 0)
				});

				// Bottom Right
				vertexArray.Append(new Vertex()
				{
					Color = Color.White,
					Position = worldPos + new Vector2f(tileset.TileWidth, tileset.TileHeight),
					TexCoords = texCoords + new Vector2f(tileset.TileWidth, tileset.TileHeight)
				});

				// Bottom Left
				vertexArray.Append(new Vertex()
				{
					Color = Color.White,
					Position = worldPos + new Vector2f(0, tileset.TileHeight),
					TexCoords = texCoords + new Vector2f(0, tileset.TileHeight)
				});
			}
		}

		public MapLayer(MapChunk chunk)
		{
			this.chunk = chunk;
		}

		public void AddTileID(int tileID)
		{
			TileIDs.Add(tileID);
			dirty = true;
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			if (dirty)
			{
				Rebuild();
				dirty = false;
			}

			foreach (var tex in Meshes)
			{
				target.Draw(tex.Item1, tex.Item2);
			}
		}
	}
}
