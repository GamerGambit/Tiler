using System;
using System.Numerics;

namespace Tiler
{
	public abstract partial class Gamemode
	{
		private const float MaxPlayerVelocity = 120.0f;

		public virtual bool IsTeamBased { get; protected set; } = false;
		public virtual float AirResistance { get; set; } = 0.01f;

		public virtual void Initialize()
		{
			// NOP
		}

		public virtual void OnMapLoaded()
		{
			// NOP
		}

		public virtual bool PlayerShouldTakeDamage(Player ply, Entity attacker)
		{
			return true;
		}

		public virtual void Think()
		{
			// NOP
		}

		public virtual void Tick()
		{
			// NOP
		}

		public virtual void OnEntityCreated(Entity ent)
		{
			// NOP
		}

		public virtual void CreateTeams()
		{
			// NOP
		}

		public virtual bool ShouldCollide(Entity ent1, Entity ent2)
		{
			return true;
		}

		public virtual void OnShutdown()
		{
			// NOP
		}

		public virtual void SetupMove(Player ply, ref MoveData mv)
		{
			// NOP
		}

		public virtual void Move(Player ply, MoveData mv, TimeSpan deltaTime)
		{
			var velocity = ply.PhysicsBody.Velocity + mv.Acceleration * (float)deltaTime.TotalSeconds;

			#region air resistance
			if (mv.Acceleration.X == 0 && mv.Acceleration.Y == 0)
			{
				var airVelocity = -velocity * Utils.Clamp(AirResistance, 0, 1);
				velocity += airVelocity;
			}
			#endregion

			#region velocity clamping
			if (velocity.Length() > MaxPlayerVelocity)
			{
				velocity = Vector2.Normalize(velocity) * MaxPlayerVelocity;
			}
			#endregion

			var pb = ply.PhysicsBody;
			pb.Acceleration = mv.Acceleration;
			pb.Velocity = velocity;
			ply.PhysicsBody = pb;

			#region collision detection
			var step = Vector2.Zero;
			var velocityNormal = (velocity.Length() == 0) ? Vector2.Zero : Vector2.Normalize(velocity);
			do
			{
				var tileType = World.Map.GetTileTypeAtWorldPosition(ply.Position + step);
				if (tileType == Map.TileType.Wall)
				{
					// TODO: react to collsion
					//velocity = Vector2.Zero;
					break;
				}

				step += velocityNormal;
			}
			while (ply.Position + step != ply.Position + velocity);
			#endregion

			ply.Position += velocity * (float)deltaTime.TotalSeconds;
		}
	}
}
