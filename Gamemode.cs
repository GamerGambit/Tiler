namespace Tiler
{
	abstract partial class Gamemode
	{
		public virtual void Initialize()
		{
			// NOP
		}

		public virtual void OnMapLoaded()
		{
			// NOP
		}

		public virtual bool PlayerShouldTakeDamage(/*Player ply, Entity attacker*/)
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

		public virtual void OnEntityCreated(/*Entity ent*/)
		{
			// NOP
		}

		public virtual bool ShouldCollide(/*Entity ent1, Entity ent2*/)
		{
			return true;
		}

		public virtual void OnShutdown()
		{
			// NOP
		}
	}
}
