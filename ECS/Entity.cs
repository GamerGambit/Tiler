using System;
using System.Collections.Generic;

namespace Tiler.ECS
{
	public abstract class Entity
	{
		internal Dictionary<string, Component> components = new Dictionary<string, Component>();
		internal List<WeakReference<System>> systems = new List<WeakReference<System>>();

		public T AddComponent<T>(T component) where T : Component
		{
			return AddComponent(component, typeof(T).ToString());
		}

		public T AddComponent<T>(T component, string name) where T : Component
		{
			components.Add(name, component);
			return component;
		}

		public T AddComponent<T>() where T : Component
		{
			return AddComponent<T>(typeof(T).ToString());
		}

		public T AddComponent<T>(string name) where T : Component
		{
			var instance = (T)Activator.CreateInstance(typeof(T), new object[] { this });
			components.Add(name, instance);
			return instance;
		}

		public T AddComponent<T>(params object[] args) where T : Component
		{
			return AddComponent<T>(typeof(T).ToString(), args);
		}

		public T AddComponent<T>(string name, params object[] args) where T : Component
		{
			var newArgs = new object[args.Length + 1];
			newArgs.SetValue(this, 0);
			args.CopyTo(newArgs, 1);

			var instance = (T)Activator.CreateInstance(typeof(T), newArgs);
			components.Add(name, instance);
			return instance;
		}

		public T RemoveComponent<T>() where T : Component
		{
			return RemoveComponent<T>(typeof(T).ToString());
		}

		public T RemoveComponent<T>(string name) where T : Component
		{
			if (!components.ContainsKey(name))
				return null;

			var component = components[name];
			components.Remove(name);

			// Since this can change `systems`, cache the list into an array
			foreach (var systemref in systems.ToArray())
			{
				if (systemref.TryGetTarget(out var system))
				{
					system.EntityRemovedComponent(this, component);
				}
				else
				{
					// dead ref?
					systems.Remove(systemref);
				}
			}

			return (T)component;
		}

		public bool HasComponent<T>() where T : Component
		{
			return HasComponent(typeof(T).ToString());
		}

		public bool HasComponent(string name)
		{
			return components.ContainsKey(name);
		}

		public T GetComponent<T>() where T : Component
		{
			return GetComponent<T>(typeof(T).ToString());
		}

		public T GetComponent<T>(string name) where T : Component
		{
			if (!HasComponent(name))
				throw new Exception($"Entity does not contain component \"{name}\"");

			return (T)components[name];
		}

		public bool HasComponentEnabled<T>() where T : Component
		{
			return HasComponentEnabled(typeof(T).ToString());
		}

		public bool HasComponentEnabled(string name)
		{
			return HasComponent(name) && components[name].Enabled;
		}
	}
}
