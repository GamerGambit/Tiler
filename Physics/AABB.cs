using System.Collections.Generic;
using System.Numerics;

namespace Tiler.Physics {
	public struct AABB {
		public Vector2 Position;
		public Vector2 Size;

		public Vector2 A => Position;
		public Vector2 B => Position + new Vector2(0, Size.Y);
		public Vector2 C => Position + Size;
		public Vector2 D => Position + new Vector2(Size.X, 0);

		public AABB(Vector2 Position, Vector2 Size) {
			this.Position = Position;
			this.Size = Size;
		}

		public IEnumerable<Vector2> GetVertices() {
			yield return A;
			yield return B;
			yield return C;
			yield return D;
		}

		public bool Collides(AABB Other) {
			return IsInside(Other.A) || IsInside(Other.B) || IsInside(Other.C) || IsInside(Other.D);
		}

		public bool IsInside(Vector2 Point) {
			if (Point.X >= Position.X && Point.X <= Position.X + Size.X)
				if (Point.Y >= Position.Y && Point.Y <= Position.Y + Size.Y)
					return true;

			return false;
		}
	}
}