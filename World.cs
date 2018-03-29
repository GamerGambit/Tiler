using System;
using System.Collections.Generic;
using System.Reflection;

using SFML.Graphics;
using SFML.System;
using TiledSharp;

namespace Tiler
{
	public static class World// : Drawable
	{
		public const int MaxChunks = 100;

		internal static readonly Dictionary<string, Type> ValidSpawnableEntityTypes = new Dictionary<string, Type>();
		internal static readonly List<Entity> Entities = new List<Entity>();

		private static int numChunks = 0;
		private static Map.Chunk[] Chunks = new Map.Chunk[MaxChunks];

		private static void SetInstancePropertyFromString(object instance, PropertyInfo propertyInfo, string value)
		{
			switch (Type.GetTypeCode(propertyInfo.PropertyType))
			{
			case TypeCode.Boolean:
				{
					if (bool.TryParse(value, out bool _out))
					{
						propertyInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Byte:
				{
					if (byte.TryParse(value, out byte _out))
					{
						propertyInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Char:
				{
					if (char.TryParse(value, out char _out))
					{
						propertyInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Double:
				{
					if (double.TryParse(value, out double _out))
					{
						propertyInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Int16:
				{
					if (short.TryParse(value, out short _out))
					{
						propertyInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Int32:
				{
					if (int.TryParse(value, out int _out))
					{
						propertyInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Int64:
				{
					if (long.TryParse(value, out long _out))
					{
						propertyInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.SByte:
				{
					if (sbyte.TryParse(value, out sbyte _out))
					{
						propertyInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Single:
				{
					if (float.TryParse(value, out float _out))
					{
						propertyInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.String:
				propertyInfo.SetValue(instance, value);
				break;

			case TypeCode.UInt16:
				{
					if (ushort.TryParse(value, out ushort _out))
					{
						propertyInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.UInt32:
				{
					if (uint.TryParse(value, out uint _out))
					{
						propertyInfo.SetValue(value, _out);
					}

					break;
				}

			case TypeCode.UInt64:
				{
					if (ulong.TryParse(value, out ulong _out))
					{
						propertyInfo.SetValue(instance, _out);
					}

					break;
				}
			}
		}

		private static void SetInstanceFieldFromString(object instance, FieldInfo fieldInfo, string value)
		{
			switch (Type.GetTypeCode(fieldInfo.FieldType))
			{
			case TypeCode.Boolean:
				{
					if (bool.TryParse(value, out bool _out))
					{
						fieldInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Byte:
				{
					if (byte.TryParse(value, out byte _out))
					{
						fieldInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Char:
				{
					if (char.TryParse(value, out char _out))
					{
						fieldInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Double:
				{
					if (double.TryParse(value, out double _out))
					{
						fieldInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Int16:
				{
					if (short.TryParse(value, out short _out))
					{
						fieldInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Int32:
				{
					if (int.TryParse(value, out int _out))
					{
						fieldInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Int64:
				{
					if (long.TryParse(value, out long _out))
					{
						fieldInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.SByte:
				{
					if (sbyte.TryParse(value, out sbyte _out))
					{
						fieldInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.Single:
				{
					if (float.TryParse(value, out float _out))
					{
						fieldInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.String:
				fieldInfo.SetValue(instance, value);
				break;

			case TypeCode.UInt16:
				{
					if (ushort.TryParse(value, out ushort _out))
					{
						fieldInfo.SetValue(instance, _out);
					}

					break;
				}

			case TypeCode.UInt32:
				{
					if (uint.TryParse(value, out uint _out))
					{
						fieldInfo.SetValue(value, _out);
					}

					break;
				}

			case TypeCode.UInt64:
				{
					if (ulong.TryParse(value, out ulong _out))
					{
						fieldInfo.SetValue(instance, _out);
					}

					break;
				}
			}
		}

		public static List<Map.Tile> Tiles { get; internal set; } = new List<Map.Tile>();

		static World()
		{
			foreach (var type in Assembly.GetAssembly(typeof(Entity)).GetTypes())
			{
				if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Entity)))
				{
					if (Attribute.GetCustomAttribute(type, typeof(SpawnableAttribute)) == null)
						continue;

					if ((Attribute.GetCustomAttribute(type, typeof(SpawnableAttribute)) as SpawnableAttribute).Spawnable == false)
						continue;

					if (type.Assembly == Assembly.GetExecutingAssembly())
					{
						ValidSpawnableEntityTypes.Add(type.Name, type);
					}
					else
					{
						ValidSpawnableEntityTypes.Add(type.FullName, type);
					}
				}
			}
		}

		public static void LoadChunk(string filename, Vector2i position)
		{
			Chunks[numChunks++] = new Map.Chunk(filename, position);
		}

		internal static void SpawnEntity(TmxObject obj, Vector2i positionOffset)
		{
			if (ValidSpawnableEntityTypes.ContainsKey(obj.Type) == false)
				return;

			var InstanceType = ValidSpawnableEntityTypes[obj.Type];

			var instance = Activator.CreateInstance(InstanceType) as Entity;
			instance.Position = new Vector2f((int)obj.X, (int)obj.Y) + new Vector2f(positionOffset.X, positionOffset.Y);

			// Check if the instance type's properties exist in the object's properties
			foreach (var instanceProperty in InstanceType.GetRuntimeProperties())
			{
				bool mapEditable = false;

				foreach (var propertyAttribute in instanceProperty.GetCustomAttributes(true))
				{
					var mapEditableAttribute = propertyAttribute as MapEditableAttribute;

					if (mapEditableAttribute is null)
						continue;

					mapEditable = true;
					break;
				}

				if (!mapEditable)
					continue;

				if (!obj.Properties.ContainsKey(instanceProperty.Name))
					continue;

				SetInstancePropertyFromString(instance, instanceProperty, obj.Properties[instanceProperty.Name]);
			}

			// Check if the instance type's fields exist in the object's properties
			foreach (var instanceField in InstanceType.GetRuntimeFields())
			{
				bool mapEditable = false;

				foreach (var fieldAttribute in instanceField.GetCustomAttributes(true))
				{
					var mapEditableAttribute = fieldAttribute as MapEditableAttribute;

					if (mapEditableAttribute is null)
						continue;

					mapEditable = true;
					break;
				}

				if (!mapEditable)
					continue;

				if (!obj.Properties.ContainsKey(instanceField.Name))
					continue;

				SetInstanceFieldFromString(instance, instanceField, obj.Properties[instanceField.Name]);
			}

			Entities.Add(instance);
		}

		public static void Draw(RenderTarget target)
		{
			foreach (var chunk in Chunks)
			{
				if (chunk is null)
					continue;

				target.Draw(chunk);
			}

			foreach (var entity in Entities)
			{
				target.Draw(entity);
			}
		}
	}
}
