using System.Numerics;

namespace Tiler.Physics
{
	public class AABB
	{
		public Vector2 Position;
		public Vector2 Size;

		public bool Contains(AABB other)
		{
			if (
				other.Position.X >= Position.X && other.Position.X <= Position.X + Size.X &&
				other.Position.Y >= Position.Y && other.Position.Y <= Position.Y + Size.Y
				)
				return true;

			return false;
		}
	}
}
