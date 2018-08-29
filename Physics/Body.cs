using System.Numerics;
using System;

namespace Tiler.Physics {
	public class Body {
		//public AABB AABB;
		public int Mass;

		public Vector2 Size;
		public Vector2 Position;
		public Vector2 Velocity;
		public Vector2 Acceleration;
		//public Vector2 Momentum { get => Mass * Velocity; }

		public float MaxVelocity;
		public float DefaultFriction;

		public AABB AABB => new AABB(Position, Size);

		public Vector2 Center => Position + (Size / 2);

		public Body(float Size = 32, int Mass = 1, float MaxVelocity = float.MaxValue, float DefaultFriction = 1.0f) {
			this.MaxVelocity = MaxVelocity;
			this.Mass = Mass;
			this.Size = new Vector2(Size);
			this.DefaultFriction = DefaultFriction;
		}

		public void Step(float Dt, float Friction) {
			PredictStep(Dt, Friction, ref Acceleration, ref Position, ref Velocity);
		}
		
		public void PredictStep(float Dt, float Friction, ref Vector2 Acceleration, ref Vector2 Position, ref Vector2 Velocity) {
			// Accelerate
			Velocity = Velocity + (Acceleration * Dt);

			// Clamp velocity to max speed
			if (Velocity.LengthSquared() >= (MaxVelocity * MaxVelocity))
				Velocity = Vector2.Normalize(Velocity) * MaxVelocity;

			// If there is any velocity
			if (!(Velocity.X == 0 && Velocity.Y == 0)) {
				// Apply frictions
				Velocity *= DefaultFriction * Friction;

				// Update position
				Position += Velocity * Dt;
			}
		}

		public Vector2 PredictStepPosition(float Dt, float Friction) {
			Vector2 Accel = Acceleration;
			Vector2 Pos = Position;
			Vector2 Vel = Velocity;

			PredictStep(Dt, Friction, ref Accel, ref Pos, ref Vel);

			return Pos;
		}
	}
}