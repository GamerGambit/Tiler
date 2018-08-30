using System.Collections.Generic;
using System.Numerics;

using Glfw3;

using SFML.Graphics;

using Tiler;

namespace Client
{
	class TestGamemode : Gamemode
	{
		private const float rateOfAcceleration = 500.0f;

		public override bool IsTeamBased { get; protected set; } = true;

		public int TestTeamID;

		public override void CreateTeams()
		{
			TestTeamID = TeamManager.AddTeam("Team Test", Color.Blue, new List<string>() { "PlayerSpawn" });
		}

		public override void PlayerInitialSpawn(Player ply)
		{
			ply.TeamID = TestTeamID;
		}

		public override void RegisterTileTypes()
		{
			// Index 1
			TileProperties.Register(new TileProperties()
			{
				Filename = "floor.png",
				Name = "Floor",
				PlayerAcceleration = 8.0f,
				Friction = 0.82f,
				IsSolid = false
			});

			// Index 2
			TileProperties.Register(new TileProperties()
			{
				Filename = "wall.png",
				Name = "Wall",
				IsSolid = true
			});

			// Index 3
			TileProperties.Register(new TileProperties()
			{
				Filename = "slime.png",
				Name = "Slime",
				PlayerAcceleration = 1.0f,
				Friction = 1.0f
			});
		}

		public override void SetupMove(Player ply, ref MoveData mv)
		{
			if (Tiler.Input.Manager.GetState(Glfw.KeyCode.F1).WasPressed)
			{
				World.Map.SetTileAtWorldPosition(ply.CenterPosition, 1);
			}
			else if (Tiler.Input.Manager.GetState(Glfw.KeyCode.F2).WasPressed)
			{
				World.Map.SetTileAtWorldPosition(ply.CenterPosition, 0);
			}

			if (Tiler.Input.Manager.GetState(Glfw.KeyCode.W).IsDown)
			{
				mv.Keys |= MoveData.InKeys.MoveForward;
			}

			if (Tiler.Input.Manager.GetState(Glfw.KeyCode.A).IsDown)
			{
				mv.Keys |= MoveData.InKeys.MoveLeft;
			}

			if (Tiler.Input.Manager.GetState(Glfw.KeyCode.S).IsDown)
			{
				mv.Keys |= MoveData.InKeys.MoveBackward;
			}

			if (Tiler.Input.Manager.GetState(Glfw.KeyCode.D).IsDown)
			{
				mv.Keys |= MoveData.InKeys.MoveRight;
			}

			var acceleration = new Vector2();

			if (mv.Keys.HasFlag(MoveData.InKeys.MoveForward))
			{
				acceleration.Y -= 1;
			}

			if (mv.Keys.HasFlag(MoveData.InKeys.MoveBackward))
			{
				acceleration.Y += 1;
			}

			if (mv.Keys.HasFlag(MoveData.InKeys.MoveLeft))
			{
				acceleration.X -= 1;
			}

			if (mv.Keys.HasFlag(MoveData.InKeys.MoveRight))
			{
				acceleration.X += 1;
			}

			if (acceleration.X == 0 && acceleration.Y == 0)
				return;

			mv.Acceleration = Vector2.Normalize(acceleration) * rateOfAcceleration;
		}
	}
}