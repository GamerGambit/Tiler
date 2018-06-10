using System.Collections.Generic;

using SFML.Graphics;

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
	}
}
