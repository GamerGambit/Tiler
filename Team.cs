using System.Collections.Generic;
using System.Collections.ObjectModel;
using SFML.Graphics;

namespace Tiler
{
	// TODO: Add a way to get a player count for a team

	public static class TeamManager
	{
		private static int teamCount = 0;
		private static List<Team> teams = new List<Team>();

		public static int AddTeam(string name, Color color, List<string> spawnPointNames = null, bool joinable = true)
		{
			var team = new Team()
			{
				TeamID = ++teamCount,
				Name = name,
				Color = color,
				SpawnPointNames = spawnPointNames,
				Joinable = joinable
			};

			teams.Add(team);

			return team.TeamID;
		}

		public static Team GetTeamByID(int teamID)
		{
			if (!teams.Exists(t => t.TeamID == teamID))
				throw new KeyNotFoundException();

			return teams.Find(t => t.TeamID == teamID);
		}
	}

	public struct Team
	{
		public int TeamID { get; internal set; }
		public string Name { get; internal set; }
		public Color Color { get; internal set; }
		public List<string> SpawnPointNames { get; internal set; }
		public bool Joinable { get; internal set; }

		public override bool Equals(object other)
		{
			return other is Team && ((Team)other).TeamID == TeamID;
		}

		public override int GetHashCode()
		{
			return 1866874249 + TeamID.GetHashCode();
		}

		public static bool operator==(Team team1, Team team2)
		{
			return team1.TeamID == team2.TeamID;
		}

		public static bool operator!=(Team team1, Team team2)
		{
			return team1.TeamID != team2.TeamID;
		}
	}
}
