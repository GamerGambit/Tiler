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

	public abstract class Entity
	{
		Dictionary<string, object> Components = new Dictionary<string, object>();

		public virtual object GetComponent(string Name)
		{
			if (Components.ContainsKey(Name))
				return Components[Name];
			return null;
		}

		public virtual object GetComponent(EntityComponents Component)
		{
			return GetComponent(Component.ToString());
		}

		public virtual T GetComponent<T>(string Name)
		{
			return (T)GetComponent(Name);
		}

		public virtual T GetComponent<T>(EntityComponents Component)
		{
			return GetComponent<T>(Component.ToString());
		}

		public virtual void SetComponent(string Name, object Val)
		{
			if (Components.ContainsKey(Name))
				Components.Remove(Name);

			if (Val != null)
				Components.Add(Name, Val);
		}

		public virtual void SetComponent<T>(string Name, T Val)
		{
			SetComponent(Name, (object)Val);
		}

		public virtual void SetComponent(EntityComponents Component, object Val)
		{
			SetComponent(Component.ToString(), Val);
		}

		public virtual void SetComponent<T>(EntityComponents Component, T Val)
		{
			SetComponent(Component.ToString(), (object)Val);
		}

		public virtual void Draw(RenderTarget target, RenderStates states)
		{
			if (GetComponent(EntityComponents.PhysicsBody) is Physics.Body B)
			{
				states.Transform.Translate(new Vector2f(B.Position.X, B.Position.Y));
			}

			target.Draw((Drawable)GetComponent(EntityComponents.GraphicsBody), states);
		}
	}
}
