using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	public enum EntityComponents
	{
		Null = 0,
		PhysicsBody,
		GraphicsBody,
	}

	public abstract class Entity : ECS.Entity
	{
		public virtual void Draw(RenderTarget target, RenderStates states)
		{
			if (!HasComponentEnabled<ECS.Components.GraphicsBody>())
				return;

			if (HasComponentEnabled<ECS.Components.PhysicsBody>())
			{
				var physicsBody = GetComponent<ECS.Components.PhysicsBody>().Value;
				states.Transform.Translate(new Vector2f(physicsBody.Position.X, physicsBody.Position.Y));
			}

			target.Draw(GetComponent<ECS.Components.GraphicsBody>().Value, states);
		}
	}
}
