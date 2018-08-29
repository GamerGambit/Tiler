using System.Collections.Generic;
using System.Numerics;

using SFML.Graphics;
using SFML.System;

namespace Tiler {
	public struct TilePhysics {
		public float PlayerAcceleration;
		public float Friction;
	}

	public class Map : Drawable {
		public enum TileType {
			Space,
			Floor,
			Wall
		}

		public readonly Vector2i TileSize = new Vector2i(32, 32);

		public Vector2i Size;
		//public List<TileType> TileIDs = new List<TileType>();
		public TileType[] TileIDs;

		private Texture TextureAtlas;
		private VertexArray VertexArray = new VertexArray(PrimitiveType.Quads);

		public Map() {
			TextureAtlas = new Texture((uint)TileSize.X * 3, (uint)TileSize.Y);
			TextureAtlas.Update(new Image("space.png"), 0, 0);
			TextureAtlas.Update(new Image("floor.png"), (uint)TileSize.X, 0);
			TextureAtlas.Update(new Image("wall.png"), (uint)TileSize.X * 2, 0);
		}

		public void Rebuild() {
			for (var index = 0; index < TileIDs.Length; ++index) {
				var tileID = (int)TileIDs[index];

				var worldPosition = new Vector2f((index % Size.X) * TileSize.X, (index / Size.X) * TileSize.Y);

				VertexArray.Append(new Vertex() {
					Position = worldPosition,
					TexCoords = new Vector2f(TileSize.X * tileID, 0),
					Color = Color.White
				});

				VertexArray.Append(new Vertex() {
					Position = worldPosition + new Vector2f(TileSize.X, 0),
					TexCoords = new Vector2f(TileSize.X * tileID + TileSize.X, 0),
					Color = Color.White
				});

				VertexArray.Append(new Vertex() {
					Position = worldPosition + new Vector2f(TileSize.X, TileSize.Y),
					TexCoords = new Vector2f(TileSize.X * tileID + TileSize.X, TileSize.Y),
					Color = Color.White
				});

				VertexArray.Append(new Vertex() {
					Position = worldPosition + new Vector2f(0, TileSize.Y),
					TexCoords = new Vector2f(TileSize.X * tileID, TileSize.Y),
					Color = Color.White
				});
			}
		}

		public TileType GetTileTypeAtWorldPosition(Vector2 worldpos) {
			int TileX = (int)(worldpos.X / TileSize.X);
			int TileY = (int)(worldpos.Y / TileSize.Y);

			if (TileX < 0 || TileX >= Size.X || TileY < 0 || TileY >= Size.Y)
				return TileType.Space;

			int Idx = TileY * Size.X + TileX;
			return TileIDs[Idx];
		}

		public IEnumerable<TileType> GetTileTypeAtWorldPosition(IEnumerable<Vector2> Positions) {
			foreach (var Pos in Positions)
				yield return GetTileTypeAtWorldPosition(Pos);
		}

		public IEnumerable<TileType> GetTileTypeAtWorldPosition(params Vector2[] Positions) {
			return GetTileTypeAtWorldPosition(Positions);
		}

		public void Draw(RenderTarget target, RenderStates states) {
			states.Texture = TextureAtlas;
			target.Draw(VertexArray, states);
		}
	}
}
