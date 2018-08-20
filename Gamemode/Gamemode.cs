using System;
using System.Numerics;

using SFML.System;

namespace Tiler
{
	public abstract partial class Gamemode
	{
		private const float MaxPlayerVelocity = 100.0f;

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

		public virtual void SetupMove(Player ply, UserCommand ucmd, MoveData mv)
		{
			// NOP
		}

		public virtual void Move(Player ply, MoveData mv, TimeSpan deltaTime)
		{
			ply.Velocity += mv.Acceleration * (float)deltaTime.TotalSeconds;

			var vec2Velocity = new Vector2(ply.Velocity.X, ply.Velocity.Y);

			#region air resistance
			if (mv.Acceleration.X == 0 && mv.Acceleration.Y == 0)
			{
				/*
				var airVelocity = -vec2Velocity * Utils.Clamp(AirResistance, 0, 1);
				vec2Velocity += airVelocity;
				*/
				vec2Velocity = Vector2.Zero;
			}
			#endregion

			#region velocity clamping
			if (vec2Velocity.Length() > MaxPlayerVelocity)
			{
				var norm = Vector2.Normalize(vec2Velocity) * MaxPlayerVelocity;
				ply.Velocity = new Vector2f(norm.X, norm.Y);
			}
			else
			{
				ply.Velocity = new Vector2f(vec2Velocity.X, vec2Velocity.Y);
			}
			#endregion

			ply.Position += ply.Velocity * (float)deltaTime.TotalSeconds;
		}
	}
}
