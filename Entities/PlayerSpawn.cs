using System.Numerics;

using SFML.Graphics;
using SFML.System;

namespace Tiler {
	[Spawnable]
	public class PlayerSpawn : Entity {
		public Vector2 SpawnPosition {
			get {
				return GetComponent<Physics.Body>(EntityComponents.PhysicsBody).Position;
			}

			set {
				GetComponent<Physics.Body>(EntityComponents.PhysicsBody).Position = value;
			}
		}

		public PlayerSpawn() {
			SetComponent(EntityComponents.GraphicsBody, new RectangleShape(new Vector2f(32, 32)) {
				FillColor = new Color(255, 160, 0, 128)
			});

			SetComponent(EntityComponents.PhysicsBody, new Physics.Body());
			SpawnPosition = Vector2.Zero;
		}

		public PlayerSpawn(float X, float Y) : this() {
			SpawnPosition = new Vector2(X, Y);
		}
	}
}