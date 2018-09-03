using System;
using System.Collections.Generic;

namespace Tiler.ECS
{
	public abstract class System : IUpdatable
	{
		public bool Enabled { get; set; } = true;

		private HashSet<Entity> entities = new HashSet<Entity>();

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

		public void RegisterEntity(Entity entity)
		{
			// Since `AppliesToEntity` can be expensive for large `requiredTypes` and/or entities with lots of components,
			// check if the hashset contains the entity first
			if (entities.Contains(entity))
				return;

			if (!AppliesToEntity(entity))
				return;

			entities.Add(entity);
			entity.systems.Add(this);
		}

		public void UnregisterEntity(Entity entity)
		{
			entities.Remove(entity);
			entity.systems.Remove(this);
		}

		public void Update(TimeSpan deltaTime)
		{
			if (!Enabled)
				return;

			foreach (var entity in entities)
			{
				UpdateEntity(entity, deltaTime);
			}
		}

		protected virtual void UpdateEntity(Entity entity, TimeSpan deltaTime)
		{
			// NOP
		}
	}
}
