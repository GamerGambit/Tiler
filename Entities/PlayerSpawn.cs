using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	[Spawnable]
	public class PlayerSpawn : Entity
	{
		[MapEditable]
		private string MapTest;

		private RectangleShape shape = new RectangleShape(new Vector2f(32, 32));

		public PlayerSpawn()
		{
			shape.FillColor = new Color(255, 160, 0, 128);
		}

		public PlayerSpawn(float X, float Y) : this()
		{
			Position = new Vector2f(X, Y);
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;
			target.Draw(shape, states);
		}
	}
}
