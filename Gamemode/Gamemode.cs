using System;
using System.Numerics;

namespace Tiler
{
	public abstract partial class Gamemode
	{
		public virtual bool IsTeamBased { get; protected set; } = false;

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

		public virtual void RegisterTileTypes()
		{
			// NOP
		}

		public virtual bool IsSolid(TileProperties tileProperties)
		{
			return tileProperties.IsSolid;
		}

		public virtual void GetWorldProperties(Vector2 WorldPosition, out WorldProps WorldProperties)
		{
			var tilePropertiesIndex = World.Map.GetTileAtWorldPosition(WorldPosition);
			var tileProperties = TileProperties.GetByIndex(tilePropertiesIndex);

			WorldProperties = new WorldProps
			{

				// Check if tile solid
				// TODO: Check if any other static physics objects in the way
				IsSolid = IsSolid(tileProperties),

				// Get atmosphere friction
				AtmosFriction = 0.987f
			};

			// space is at index 0
			if (tilePropertiesIndex == 0)
			{
				WorldProperties.AtmosFriction = 1;
			}

		}

		public virtual void Move(Player ply, MoveData mv, TimeSpan deltaTime)
		{
			float Dt = (float)deltaTime.TotalSeconds;
			var Body = ply.GetComponent<ECS.Components.PhysicsBody>().Value;

			// Movement properties depend on the current tile the player is standing on
			var Phys = TileProperties.GetByIndex(World.Map.GetTileAtWorldPosition(Body.Center));

			// Apply other forces, like air friction
			GetWorldProperties(Body.Center, out WorldProps Props);
			Phys.Friction *= Props.AtmosFriction;

			// Predict movement this frame
			Body.Acceleration = (mv.Acceleration * Phys.PlayerAcceleration);
			Vector2 PredictedPos = Body.PredictStepPosition(Dt, Phys.Friction);

			//> If predicted movement collides with any world geometry
			// then cancel any acceleration and velocity.
			//> There are two separate checks for horizontal and vertical movement
			// so sliding across walls still works
			IfCollidesMultiply(Body, new Vector2(PredictedPos.X, Body.Position.Y), new Vector2(0, 1));
			IfCollidesMultiply(Body, new Vector2(Body.Position.X, PredictedPos.Y), new Vector2(1, 0));

			// Perform the actual step
			Body.Step(Dt, Phys.Friction);

			// EVENTUALLY:
			// If the player gets stuck in geometry somehow, any collision checks should
			// return TRUE here, which means we should rewind the player position until it's not stuck in the geometry
			// anymore. Ray casting from current stuck position to previous position.
			// Do not do this until the hypothetical bug actually shows up
		}

		bool IfCollidesMultiply(Physics.Body B, Vector2 PredictedPos, Vector2 Mul)
		{
			foreach (var Vert in new Physics.AABB(PredictedPos, B.Size).Vertices)
			{
				GetWorldProperties(Vert, out WorldProps Props);

				if (Props.IsSolid)
				{
					B.Acceleration *= Mul;
					B.Velocity *= Mul;
					return true;
				}
			}

			return false;
		}
	}
}