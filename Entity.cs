using SFML.Graphics;

namespace Tiler
{
	public abstract class Entity : Transformable, Drawable
	{
		public abstract void Draw(RenderTarget target, RenderStates states);

		protected virtual void Initialize()
		{
			// NOP
		}


	}
}
