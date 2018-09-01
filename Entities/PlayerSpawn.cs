using System.Numerics;

using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	[Spawnable]
	public class PlayerSpawn : Entity
	{
		public Vector2 SpawnPosition
		{
			get => GetComponent<ECS.Components.PhysicsBody>().Value.Position;
			set => GetComponent<ECS.Components.PhysicsBody>().Value.Position = value;
		}

		public PlayerSpawn()
		{
			var graphicsBody = AddComponent<ECS.Components.GraphicsBody>(new RectangleShape(new Vector2f(32, 32))
			{
				FillColor = new Color(255, 160, 0, 128)
			});

			AddComponent<ECS.Components.PhysicsBody>();
			SpawnPosition = Vector2.Zero;
		}

		public PlayerSpawn(float X, float Y) : this()
		{
			SpawnPosition = new Vector2(X, Y);
		}
	}
}