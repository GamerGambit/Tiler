using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

using Tiler;

namespace Client
{
	class TestGamemode : Gamemode
	{
		public override bool IsTeamBased { get; protected set; } = true;

		public int TestTeamID;

		public override void CreateTeams()
		{
			TestTeamID = TeamManager.AddTeam("Team Test", Color.Blue, new List<string>() { "PlayerSpawnEntity" });
		}

		public override void PlayerInitialSpawn(Player ply)
		{
			ply.TeamID = TestTeamID;
		}

		public override void CreateUserCommand(UserCommand ucmd)
		{
			if (Tiler.Input.Manager.GetInputState(Keyboard.Key.W).IsDown)
				ucmd.Keys |= InKeys.MoveForward;

			if (Tiler.Input.Manager.GetInputState(Keyboard.Key.A).IsDown)
				ucmd.Keys |= InKeys.MoveLeft;

			if (Tiler.Input.Manager.GetInputState(Keyboard.Key.S).IsDown)
				ucmd.Keys |= InKeys.MoveBackward;

			if (Tiler.Input.Manager.GetInputState(Keyboard.Key.D).IsDown)
				ucmd.Keys |= InKeys.MoveRight;

			if (Tiler.Input.Manager.GetInputState(Keyboard.Key.Space).IsDown)
				ucmd.Keys |= InKeys.Jump;
		}

		public override void SetupMove(Player ply, UserCommand ucmd, MoveData mv)
		{
			mv.Origin = ply.Position;

			var velocity = new Vector2f();

			if (ucmd.Keys.HasFlag(InKeys.MoveForward))
				velocity.Y -= 1;

			if (ucmd.Keys.HasFlag(InKeys.MoveBackward))
				velocity.Y += 1;

			if (ucmd.Keys.HasFlag(InKeys.MoveLeft))
				velocity.X -= 1;

			if (ucmd.Keys.HasFlag(InKeys.MoveRight))
				velocity.X += 1;

			// TODO: this is the movement speed (100.0f), so make it legit at some point
			velocity.X *= 100.0f;
			velocity.Y *= 100.0f;

			mv.Velocity = velocity;
		}
	}
}
