using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Tiler.Importers
{
	public abstract class Importer
	{
		public virtual bool CanLoad(string filePath)
		{
			return CanLoadExtension(Path.GetExtension(filePath));
		}

		public abstract bool CanLoadExtension(string extension);
	}

	public abstract class Importer<T> : Importer
	{
		public abstract T Load(string filePath);
	}

	public static class Importers
	{
		private static List<Importer> AllImporters = new List<Importer>();

		static Importers()
		{
			// @todo ship the engine with an `Application`/`Program` class that does this.
			RegisterAll(Assembly.GetExecutingAssembly());
		}

		public static void Register(Type importerType)
		{
			foreach (var importer in AllImporters)
			{
				if (importer.GetType() == importerType)
					throw new Exception($"Importer type already exists ({importerType})");
			}

			AllImporters.Add((Importer)Activator.CreateInstance(importerType));
		}

		public static void RegisterAll()
		{
			RegisterAll(Assembly.GetExecutingAssembly());
		}

		public static void RegisterAll(Assembly assembly)
		{
			foreach (var type in assembly.GetTypes())
			{
				if (!type.IsAbstract && !type.IsInterface && type.IsSubclassOf(typeof(Importer)))
				{
					Register(type);
				}
			}
		}

		public static Importer<T> Get<T>(string filePath)
		{
			foreach (var importer in GetAll<T>())
			{
				if (importer.CanLoad(filePath))
					return importer;
			}

			throw new Exception($"No Importer has been registed for filepath: {filePath}");
		}

		public static IEnumerable<Importer<T>> GetAll<T>()
		{
			foreach (var importer in AllImporters)
			{
				var casted = importer as Importer<T>;

				if (!(casted is null))
					yield return casted;
			}
		}

		public static T Load<T>(string filePath)
		{
			return Get<T>(filePath).Load(filePath);
		}
	}
}
