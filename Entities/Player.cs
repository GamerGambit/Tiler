using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	public class Player : Entity
	{
		private Shape shape;

		public int TeamID = -1;

		public Player()
		{
			shape = new RectangleShape(new Vector2f(32, 32))
			{
				FillColor = Color.Blue
			};
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;
			target.Draw(shape, states);
		}
	}
}
