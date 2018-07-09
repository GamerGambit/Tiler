using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	public class Map : Drawable
	{
		public readonly Vector2i TileSize = new Vector2i(32, 32);

		public Vector2i Size;
		public List<int> TileIDs = new List<int>();

		private Texture TextureAtlas;
		private VertexArray VertexArray = new VertexArray(PrimitiveType.Quads);

		public Map()
		{
			TextureAtlas = new Texture((uint)TileSize.X * 3, (uint)TileSize.Y);
			TextureAtlas.Update(new Image("space.png"), 0, 0);
			TextureAtlas.Update(new Image("floor.png"), (uint)TileSize.X, 0);
			TextureAtlas.Update(new Image("wall.png"), (uint)TileSize.X * 2, 0);
		}

		public void Rebuild()
		{
			for (var index = 0; index < TileIDs.Count; ++index)
			{
				var tileID = TileIDs[index];
				var worldPosition = new Vector2f((index % Size.X) * TileSize.X, index / Size.X * TileSize.Y);

				VertexArray.Append(new Vertex()
				{
					Position = worldPosition,
					TexCoords = new Vector2f(TileSize.X * tileID, 0),
					Color = Color.White
				});

				VertexArray.Append(new Vertex()
				{
					Position = worldPosition + new Vector2f(TileSize.X, 0),
					TexCoords = new Vector2f(TileSize.X * tileID + TileSize.X, 0),
					Color = Color.White
				});

				VertexArray.Append(new Vertex()
				{
					Position = worldPosition + new Vector2f(TileSize.X, TileSize.Y),
					TexCoords = new Vector2f(TileSize.X * tileID + TileSize.X, TileSize.Y),
					Color = Color.White
				});

				VertexArray.Append(new Vertex()
				{
					Position = worldPosition + new Vector2f(0, TileSize.Y),
					TexCoords = new Vector2f(TileSize.X * tileID, TileSize.Y),
					Color = Color.White
				});
			}
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			states.Texture = TextureAtlas;
			target.Draw(VertexArray, states);
		}
	}
}
