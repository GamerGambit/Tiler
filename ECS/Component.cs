namespace Tiler.ECS
{
	public class Component
	{
		private readonly Entity owningEntity = null;

		public bool IsEnabled = true;

		public Component(Entity owningEntity)
		{
			this.owningEntity = owningEntity;
		}
	}
}
