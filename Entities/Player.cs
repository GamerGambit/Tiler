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
				return GetComponent<Physics.Body>(EntityComponents.PhysicsBody).Center;
			}
		}

		public Player()
		{
			SetComponent(EntityComponents.PhysicsBody, new Physics.Body(Size: PlayerSize));

			SetComponent(EntityComponents.GraphicsBody, new RectangleShape(new Vector2f(PlayerSize, PlayerSize))
			{
				FillColor = Color.Blue
			});
		}
	}
}