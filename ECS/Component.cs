namespace Tiler.ECS
{
	public abstract class Component
	{
		private readonly Entity owningEntity = null;

		public bool Enabled { get; set; } = true;

		public Component(Entity owningEntity)
		{
			this.owningEntity = owningEntity;
		}
	}
}
