using System;
using System.Collections.Generic;
using System.Reflection;

using SFML.Graphics;

namespace Tiler {
	public struct WorldProps {
		public float AtmosFriction;
		public bool IsSolid;
	}
	
	public static class World {
		internal static readonly Dictionary<string, Type> ValidSpawnableEntityTypes = new Dictionary<string, Type>();

		public static Map Map;

		public static List<Entity> Entities = new List<Entity>();

		static World() {
			foreach (var type in Assembly.GetAssembly(typeof(Entity)).GetTypes()) {
				if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Entity))) {
					if (Attribute.GetCustomAttribute(type, typeof(SpawnableAttribute)) is null)
						continue;

					if ((Attribute.GetCustomAttribute(type, typeof(SpawnableAttribute)) as SpawnableAttribute).Spawnable == false)
						continue;

					if (type.Assembly == Assembly.GetExecutingAssembly()) {
						ValidSpawnableEntityTypes.Add(type.Name, type);
					} else {
						ValidSpawnableEntityTypes.Add(type.FullName, type);
					}
				}
			}
		}

		public static void Draw(Window target) {
			if (!(Map is null))
				target.Draw(Map);

			foreach (var entity in Entities) {
				//target.Draw(entity);
				entity.Draw(target.RenderWindow, RenderStates.Default);
			}
		}
	}
}
