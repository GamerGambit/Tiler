using System.Numerics;

using SFML.Graphics;
using SFML.System;

namespace Tiler {
	[Spawnable]
	public class PlayerSpawn : Entity {
		Vector2 spawnPosition;

		public Vector2 SpawnPosition {
			get {
				return spawnPosition;
			}

			set {
				spawnPosition = value;

				Physics.Body Body = GetComponent<Physics.Body>(EntityComponents.PhysicsBody);
				Body.Position = value;
				SetComponent(EntityComponents.PhysicsBody, Body);
			}
		}

		public PlayerSpawn() {
			SetComponent(EntityComponents.GraphicsBody, new RectangleShape(new Vector2f(32, 32)) {
				FillColor = new Color(255, 160, 0, 128)
			});

			SetComponent(EntityComponents.PhysicsBody, Physics.Body.Create());
			SpawnPosition = Vector2.Zero;
		}

		public PlayerSpawn(float X, float Y) : this() {
			SpawnPosition = new Vector2(X, Y);
		}
	}
}
