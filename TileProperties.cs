using System.Collections.Generic;

namespace Tiler
{
	public struct TileProperties
	{
		#region static
		private static List<TileProperties> propertyList = new List<TileProperties>();

		static TileProperties()
		{
			Register(new TileProperties()
			{
				Filename = "space.png",
				Name = "Space",
				PlayerAcceleration = 0,
				Friction = 1,
				IsSolid = false
			});
		}

		public static int Register(TileProperties properties)
		{
			propertyList.Add(properties);
			return propertyList.Count - 1;
		}

		public static TileProperties GetByIndex(int index)
		{
			return propertyList[index];
		}

		public static TileProperties GetByName(string name)
		{
			return propertyList.Find(tp => tp.Name == name);
		}
		#endregion

		public string Filename;
		public string Name;
		public float PlayerAcceleration;
		public float Friction;
		public bool IsSolid;
	}
}
