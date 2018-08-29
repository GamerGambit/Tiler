using SFML.Graphics;
using SFML.System;

namespace Tiler {
	public class Player : Entity {
		const float PlayerSize = 20;

		public int TeamID = -1;

		public Player() {
			SetComponent(EntityComponents.PhysicsBody, new Physics.Body(Size: PlayerSize));

			SetComponent(EntityComponents.GraphicsBody, new RectangleShape(new Vector2f(PlayerSize, PlayerSize)) {
				FillColor = Color.Blue
			});
		}
	}
}