using System.Numerics;

using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	public abstract class Entity : Transformable, Drawable
	{
		// INFO: Not required, besides, not the right place to put it
		//private static uint UniqueIDCounter = 0;
		//public readonly uint UniqueID = UniqueIDCounter++;

		public new Vector2 Position
		{
			get => new Vector2(base.Position.X, base.Position.Y);
			set
			{
				PhysicsBody.AABB.Position = value;
				base.Position = new Vector2f(value.X, value.Y);
			}
		}

		public Physics.Body PhysicsBody { get; set; }

		public Entity()
		{
			PhysicsBody = new Physics.Body()
			{
				AABB = new Physics.AABB()
				{
					Size = new Vector2(32, 32)
				},
				Mass = 1
			};
		}

		public abstract void Draw(RenderTarget target, RenderStates states);
	}
}
