using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	public class Map : Drawable
	{
		public readonly Vector2i TileSize = new Vector2i(32, 32);

		public Vector2i Size;
		//public List<TileType> TileIDs = new List<TileType>();
		public int[] TilePropertyIndexes = new int[0];

		bool IsDirty;
		Vector2i TextureAtlasSize;
		Texture TextureAtlas;
		VertexArray VertexArray = new VertexArray(PrimitiveType.Quads);

		public Map()
		{
			TextureAtlasSize = TileSize * 32;
			TextureAtlas = new Texture((uint)TextureAtlasSize.X, (uint)TextureAtlasSize.Y);
		}

		public Map(int Width, int Height) : this()
		{
			SetMapSize(Width, Height);
		}

		public void SetTileTexture(Image Img, int X, int Y)
		{
			TextureAtlas.Update(Img, (uint)(X * TileSize.X), (uint)(Y * TileSize.Y));
		}

		public void SetTileTexture(Image Img, int TileIdx)
		{
			SetTileTexture(Img, TileIdx % TextureAtlasSize.X, TileIdx / TextureAtlasSize.Y);
		}

		public Vector2f GetTileUV(int TileIdx)
		{
			int X = TileIdx % TextureAtlasSize.X;
			int Y = TileIdx / TextureAtlasSize.Y;
			return new Vector2f(X * TileSize.X, Y * TileSize.Y);
		}

		public void SetMapSize(int Width, int Height)
		{
			Size = new Vector2i(Width, Height);
			TilePropertyIndexes = new int[Width * Height];
			IsDirty = true;
		}

		void Rebuild()
		{
			if (!IsDirty)
				return;

			IsDirty = false;

			TextureAtlas = new Texture((uint)TextureAtlasSize.X, (uint)TextureAtlasSize.Y);

			for (var index = 0; index < TilePropertyIndexes.Length; ++index)
			{
				var propertyIndex = TilePropertyIndexes[index];
				var props = TileProperties.GetByIndex(propertyIndex);
				var fileName = Path.Combine("data", "tiles", props.Filename);

				if (File.Exists(fileName))
				{
					SetTileTexture(new Image(fileName), propertyIndex);
				}
			}

			VertexArray.Clear();

			for (var index = 0; index < TilePropertyIndexes.Length; ++index)
			{
				Vector2f TileUV = GetTileUV(TilePropertyIndexes[index]);

				var worldPosition = new Vector2f((index % Size.X) * TileSize.X, (index / Size.X) * TileSize.Y);

				VertexArray.Append(new Vertex()
				{
					Position = worldPosition,
					TexCoords = new Vector2f(TileUV.X, 0),
					Color = Color.White
				});

				VertexArray.Append(new Vertex()
				{
					Position = worldPosition + new Vector2f(TileSize.X, 0),
					TexCoords = new Vector2f(TileUV.X + TileSize.X, TileUV.Y),
					Color = Color.White
				});

				VertexArray.Append(new Vertex()
				{
					Position = worldPosition + new Vector2f(TileSize.X, TileSize.Y),
					TexCoords = new Vector2f(TileUV.X + TileSize.X, TileUV.Y + TileSize.Y),
					Color = Color.White
				});

				VertexArray.Append(new Vertex()
				{
					Position = worldPosition + new Vector2f(0, TileSize.Y),
					TexCoords = new Vector2f(TileUV.X, TileUV.Y + TileSize.Y),
					Color = Color.White
				});
			}
		}

		public void SetTile(int X, int Y, int tilePropertiesIndex)
		{
			if (X < 0 || Y < 0 || X >= Size.X || Y >= Size.Y)
				return;

			TilePropertyIndexes[Y * Size.X + X] = tilePropertiesIndex;
			IsDirty = true;
		}

		public void SetTileAtWorldPosition(Vector2 WorldPos, int tilePropertiesIndex)
		{
			SetTile((int)(WorldPos.X / TileSize.X), (int)(WorldPos.Y / TileSize.Y), tilePropertiesIndex);
		}

		// Returns: Index to use with TileProperties static functions
		public int GetTileAtWorldPosition(Vector2 worldpos)
		{
			int TileX = (int)(worldpos.X / TileSize.X);
			int TileY = (int)(worldpos.Y / TileSize.Y);

			if (TileX < 0 || TileX >= Size.X || TileY < 0 || TileY >= Size.Y)
				return 0; // space is at index 0

			int Idx = TileY * Size.X + TileX;
			return TilePropertyIndexes[Idx];
		}

		public IEnumerable<int> GetTileAtWorldPosition(IEnumerable<Vector2> Positions)
		{
			foreach (var Pos in Positions)
				yield return GetTileAtWorldPosition(Pos);
		}

		public IEnumerable<int> GetTileAtWorldPosition(params Vector2[] Positions)
		{
			return GetTileAtWorldPosition(Positions);
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			Rebuild();

			states.Texture = TextureAtlas;
			target.Draw(VertexArray, states);
		}
	}
}
