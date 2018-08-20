using System.Numerics;

namespace Tiler.Physics
{
	public struct Body
	{
		public AABB AABB;
		public uint Mass;
		public Vector2 Velocity;
		public Vector2 Acceleration;
		public Vector2 Momentum { get => Mass * Velocity; }
	}
}
