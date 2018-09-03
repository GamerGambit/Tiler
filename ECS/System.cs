using System;
using System.Collections.Generic;

namespace Tiler.ECS
{
	public abstract class System : IUpdatable
	{
		public bool Enabled { get; set; } = true;

		private HashSet<WeakReference<Entity>> entities = new HashSet<WeakReference<Entity>>();

		protected readonly Type[] requiredTypes;

		internal void EntityRemovedComponent(Entity entity, Component component)
		{
			foreach (var type in requiredTypes)
			{
				if (type == component.GetType())
				{
					UnregisterEntity(entity);
					return;
				}
			}
		}

		private bool AppliesToEntity(Entity entity)
		{
			if (entity.components.Count == 0)
				return false;

			foreach (var type in requiredTypes)
			{
				bool found = false;

				foreach (var pair in entity.components)
				{
					if (pair.Value.GetType() == type)
					{
						found = true;
						break;
					}
				}

				if (!found)
					return false;
			}

			return true;
		}

		void RegisterEntity(Entity entity)
		{
			var weakref = new WeakReference<Entity>(entity);

			// Since `AppliesToEntity` can be expensive for large `requiredTypes` and/or entities with lots of components,
			// check if the hashset contains the reference first
			if (entities.Contains(weakref))
				return;

			if (!AppliesToEntity(entity))
				return;

			entities.Add(weakref);
			entity.systems.Add(new WeakReference<System>(this));
		}

		void UnregisterEntity(Entity entity)
		{
			entities.Remove(new WeakReference<Entity>(entity));
			entity.systems.Remove(new WeakReference<System>(this));
		}

		public void Update(TimeSpan deltaTime)
		{
			if (!Enabled)
				return;

			// remove dead references
			entities.RemoveWhere(r => r.TryGetTarget(out var e) == false);

			foreach (var entityref in entities)
			{
				if (entityref.TryGetTarget(out var entity))
				{
					UpdateEntity(entity, deltaTime);
				}
			}
		}

		protected virtual void UpdateEntity(Entity entity, TimeSpan deltaTime)
		{
			// NOP
		}
	}
}
