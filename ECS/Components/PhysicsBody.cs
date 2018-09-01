namespace Tiler.ECS.Components
{
	public class PhysicsBody : Component
	{
		public Physics.Body Value = new Physics.Body();

		public PhysicsBody(Entity owningEntity) : base(owningEntity)
		{
			// NOP
		}
	}
}
