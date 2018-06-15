using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	public abstract class Entity : Transformable, Drawable
	{
		// INFO: Not required, besides, not the right place to put it
		//private static uint UniqueIDCounter = 0;
		//public readonly uint UniqueID = UniqueIDCounter++;

		public Vector2f Velocity;

		public abstract void Draw(RenderTarget target, RenderStates states);
	}
}
