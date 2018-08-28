using SFML.Graphics;
using SFML.System;

namespace Tiler {
	public class Player : Entity {
		public int TeamID = -1;

		public Player() {
			SetComponent(EntityComponents.PhysicsBody, Physics.Body.Create());

			SetComponent(EntityComponents.GraphicsBody, new RectangleShape(new Vector2f(32, 32)) {
				FillColor = Color.Blue
			});
		}
	}
}
