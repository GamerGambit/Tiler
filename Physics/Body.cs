using System.Numerics;
using System;

namespace Tiler.Physics {
	public struct Body {
		//public AABB AABB;
		public int Mass;

		public Vector2 Size;
		public Vector2 Position;
		public Vector2 Velocity;
		public Vector2 Acceleration;
		//public Vector2 Momentum { get => Mass * Velocity; }

		public float MaxVelocity;

		public Body Step(float Dt, float Friction) {
			Velocity += Acceleration * Dt;

			// Clamp velocity
			if (Velocity.LengthSquared() >= (MaxVelocity * MaxVelocity))
				Velocity = Vector2.Normalize(Velocity) * MaxVelocity;

			if (!(Velocity.X == 0 && Velocity.Y == 0))
				Position += Velocity * Dt;
			return this;
		}

		public static Body Create(float Size = 32, int Mass = 1) {
			Body B = new Body();
			B.MaxVelocity = float.MaxValue;
			B.Mass = Mass;
			B.Size = new Vector2(Size);
			return B;
		}
	}
}
