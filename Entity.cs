using SFML.Graphics;

namespace Tiler
{
	public abstract class Entity : Transformable, Drawable
	{
		private static uint UniqueIDCounter = 0;

		public readonly uint UniqueID = UniqueIDCounter++;

		public abstract void Draw(RenderTarget target, RenderStates states);
	}
}
