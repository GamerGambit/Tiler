using System;

namespace Tiler
{
	public class SpawnableAttribute : Attribute
	{
		public bool Spawnable { get; set; }

		public SpawnableAttribute(bool spawnable)
		{
			Spawnable = spawnable;
		}
	}
}
