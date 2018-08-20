using System.Collections.Generic;
using System.Numerics;

using SFML.Graphics;
using SFML.System;

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

		public override void CreateUserCommand(UserCommand ucmd)
		{
			if (Tiler.Input.Manager.GetState(Glfw3.Glfw.KeyCode.W).IsDown)
				ucmd.Keys |= InKeys.MoveForward;

			if (Tiler.Input.Manager.GetState(Glfw3.Glfw.KeyCode.A).IsDown)
				ucmd.Keys |= InKeys.MoveLeft;

			if (Tiler.Input.Manager.GetState(Glfw3.Glfw.KeyCode.S).IsDown)
				ucmd.Keys |= InKeys.MoveBackward;

			if (Tiler.Input.Manager.GetState(Glfw3.Glfw.KeyCode.D).IsDown)
				ucmd.Keys |= InKeys.MoveRight;

			if (Tiler.Input.Manager.GetState(Glfw3.Glfw.KeyCode.Space).IsDown)
				ucmd.Keys |= InKeys.Jump;
		}

		public override void SetupMove(Player ply, UserCommand ucmd, MoveData mv)
		{
			mv.Origin = ply.Position;

			var acceleration = new Vector2();

			if (ucmd.Keys.HasFlag(InKeys.MoveForward))
				acceleration.Y -= 1;

			if (ucmd.Keys.HasFlag(InKeys.MoveBackward))
				acceleration.Y += 1;

			if (ucmd.Keys.HasFlag(InKeys.MoveLeft))
				acceleration.X -= 1;

			if (ucmd.Keys.HasFlag(InKeys.MoveRight))
				acceleration.X += 1;

			if (acceleration.X == 0 && acceleration.Y == 0)
				return;

			var norm = Vector2.Normalize(acceleration) * rateOfAcceleration;
			mv.Acceleration = new Vector2f(norm.X, norm.Y);
		}
	}
}
