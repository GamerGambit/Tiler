using SFML.Graphics;

namespace Tiler.ECS.Components
{
	public class GraphicsBody : Component
	{
		public Drawable Value;

		public GraphicsBody(Entity owningEntity, Drawable drawable) : base(owningEntity)
		{
			Value = drawable;
		}
	}
}
