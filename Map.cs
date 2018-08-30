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
		public enum TileType : int
		{
			Space,
			Floor,
			Wall,
			Slime,
		}

		public readonly Vector2i TileSize = new Vector2i(32, 32);

		public Vector2i Size;
		//public List<TileType> TileIDs = new List<TileType>();
		public TileType[] TileIDs;

		bool IsDirty;
		Vector2i TextureAtlasSize;
		Texture TextureAtlas;
		VertexArray VertexArray = new VertexArray(PrimitiveType.Quads);

		public Map()
		{
			TextureAtlasSize = TileSize * 32;
			TextureAtlas = new Texture((uint)TextureAtlasSize.X, (uint)TextureAtlasSize.Y);

			foreach (TileType TT in Enum.GetValues(typeof(TileType)))
			{
				string FileName = Path.Combine("data", "tiles", TT.ToString().ToLower() + ".png");

				if (File.Exists(FileName))
					SetTileTexture(new Image(FileName), TT);
			}
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

		public void SetTileTexture(Image Img, TileType TT)
		{
			SetTileTexture(Img, (int)TT);
		}

		public Vector2f GetTileUV(int TileIdx)
		{
			int X = TileIdx % TextureAtlasSize.X;
			int Y = TileIdx / TextureAtlasSize.Y;
			return new Vector2f(X * TileSize.X, Y * TileSize.Y);
		}

		public Vector2f GetTileUV(TileType TT)
		{
			return GetTileUV((int)TT);
		}

		public void SetMapSize(int Width, int Height)
		{
			Size = new Vector2i(Width, Height);
			TileIDs = new TileType[Width * Height];
			IsDirty = true;
		}

		void Rebuild()
		{
			if (!IsDirty)
				return;
			IsDirty = false;

			VertexArray.Clear();

			for (var index = 0; index < TileIDs.Length; ++index)
			{
				Vector2f TileUV = GetTileUV(TileIDs[index]);

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

		public void SetTile(int X, int Y, TileType TT)
		{
			if (X < 0 || Y < 0 || X >= Size.X || Y >= Size.Y)
				return;

			TileIDs[Y * Size.X + X] = TT;
			IsDirty = true;
		}

		public void SetTileAtWorldPosition(Vector2 WorldPos, TileType TT)
		{
			SetTile((int)(WorldPos.X / TileSize.X), (int)(WorldPos.Y / TileSize.Y), TT);
		}

		public TileType GetTileAtWorldPosition(Vector2 worldpos)
		{
			int TileX = (int)(worldpos.X / TileSize.X);
			int TileY = (int)(worldpos.Y / TileSize.Y);

			if (TileX < 0 || TileX >= Size.X || TileY < 0 || TileY >= Size.Y)
				return TileType.Space;

			int Idx = TileY * Size.X + TileX;
			return TileIDs[Idx];
		}

		public IEnumerable<TileType> GetTileAtWorldPosition(IEnumerable<Vector2> Positions)
		{
			foreach (var Pos in Positions)
				yield return GetTileAtWorldPosition(Pos);
		}

		public IEnumerable<TileType> GetTileAtWorldPosition(params Vector2[] Positions)
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
