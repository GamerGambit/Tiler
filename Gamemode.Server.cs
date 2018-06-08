namespace Tiler
{
	public abstract partial class Gamemode
	{
		public virtual void PlayerConnected(/*IPAddress ip, ushort port*/)
		{
			// NOP
		}

		public virtual void PlayerDisconnected(Player ply)
		{
			// NOP
		}

		public virtual (bool, string) PlayerAuth(/*IPAddress ip, ushort port, string username, string passwd*/)
		{
			return (true, null);
		}

		public virtual void DoPlayerDeath(Player ply, Entity attacker/*, DMGInfo info*/)
		{
			// NOP
		}

		public virtual void EntityTakeDamage(Entity ent/*, DMGInfo info*/)
		{
			// NOP
		}

		public virtual bool PlayerCanPickupItem(Player ply, Entity item)
		{
			return true;
		}

		public virtual bool PlayerCanPickupWeapon(Player ply, Weapon weapon)
		{
			return true;
		}

		public virtual string PlayerSay(Player ply, string text, bool teamOnly)
		{
			return text;
		}

		public virtual void PlayerDeathThink(Player ply)
		{
			// NOP
		}

		// TODO: distinguish between clicking on an item in-hand, clicking an item onto the player, and clicking the/a player with an item in-hand.
		public virtual bool PlayerUse(Player ply, Entity ent)
		{
			return true;
		}

		public virtual void PlayerDeath(Player ply, Entity inflictor, Player attacker)
		{
			// NOP
		}

		public virtual void PlayerInitialSpawn(Player ply)
		{
			// NOP
		}

		public virtual void PlayerSpawnAsSpectator(Player ply)
		{
			// NOP
		}

		public virtual void PlayerSpawn(Player ply)
		{
			// NOP
		}

		public virtual void PlayerLoadout(Player ply)
		{
			// NOP
		}

		public virtual bool IsSpawnPointSuitable(Player ply, Entity spawnPoint, bool makeSuitable)
		{
			return true;
		}

		// TODO: return an Entity to use as a spawn point
		public virtual void PlayerSelectSpawn(Player ply)
		{
			// NOP
		}

		// TODO: return an Entity to use as a spawn point
		public virtual void PlayerSelectTeamSpawn(Player ply, int teamID)
		{
			// NOP
		}

		public virtual void PlayerWeaponEquipped(Player ply, Weapon weapon)
		{
			// NOP
		}

		public virtual void ScalePlayerDamage(Player ply/*, DMGInfo info*/)
		{
			// NOP
		}

		public virtual bool PlayerCanEnterVehicle(Player ply, Vehicle vehicle)
		{
			return true;
		}

		public virtual bool PlayerCanExitVehicle(Player ply, Vehicle vehicle)
		{
			return true;
		}

		public virtual void PlayerEnteredVehicle(Player ply, Vehicle vehicle)
		{
			// NOP
		}

		public virtual void PlayerExitedVehicle(Player ply, Vehicle vehicle)
		{
			// NOP
		}

		public virtual bool PlayerCanJoinTeam(Player ply, int teamID)
		{
			// TODO: add a time delay between team switches
			return ply.TeamID != teamID;
		}

		public virtual void PlayerRequestTeam(Player ply, int teamID)
		{
			if (!IsTeamBased)
				return;

			if (!TeamManager.GetTeamByID(teamID).Joinable)
				return;

			if (!PlayerCanJoinTeam(ply, teamID))
				return;

			PlayerJoinTeam(ply, teamID);
		}

		public virtual void PlayerJoinTeam(Player ply, int teamID)
		{
			var oldTeamID = ply.TeamID;

			ply.TeamID = teamID;
			// TODO: set last team switch time to now

			OnPlayerJoinTeam(ply, oldTeamID, teamID);
		}

		public virtual void OnPlayerJoinTeam(Player ply, int oldTeamID, int newTeamID)
		{
			// NOP
		}

		/*
		// TODO: revisit when networking is implemented
		public virtual bool PlayerCanSeePlayersChat(string text, bool teamOnly, Player speaker, Player listener)
		{
			if (teamOnly)
				return speaker.TeamID == listener.TeamID;

			return true;
		}

		public virtual bool PlayerCanHearPlayersVoice(Player speaker, Player listener)
		{
			return true;
		}
		*/
	}
}
