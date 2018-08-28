using System.Numerics;

namespace Tiler.Physics
{
	public struct Body
	{
		//public AABB AABB;
		public int Mass;

		public Vector2 Size;
		public Vector2 Position;
		public Vector2 Velocity;
		public Vector2 Acceleration;
		//public Vector2 Momentum { get => Mass * Velocity; }

		public static Body Create(float Size = 32, int Mass = 1) {
			return new Body() {
				Mass = Mass,
				Size = new Vector2(Size)
			};
		}
	}
}
