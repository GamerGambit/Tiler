using Tiler;

namespace Client
{
	class TestGamemode : Gamemode
	{
		public override bool IsTeamBased { get; protected set; } = true;

		public override void PlayerInitialSpawn(Player ply)
		{
			ply.TeamID = Program.TestTeam;
		}
	}
}
