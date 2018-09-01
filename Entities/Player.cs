using System.Numerics;

using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	public class Player : Entity
	{
		const float PlayerSize = 20;
		public int TeamID = -1;

		public Vector2 CenterPosition
		{
			get
			{
				return GetComponent<ECS.Components.PhysicsBody>().Value.Center;
			}
		}

		public Player()
		{
			var physicsBody = AddComponent<ECS.Components.PhysicsBody>();
			physicsBody.Value = new Physics.Body(Size: PlayerSize);

			var graphicsBody = AddComponent<ECS.Components.GraphicsBody>(new RectangleShape(new Vector2f(PlayerSize, PlayerSize))
			{
				FillColor = Color.Blue
			});
		}
	}
}