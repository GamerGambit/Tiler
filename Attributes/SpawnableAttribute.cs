using System;

namespace Tiler
{
	public class SpawnableAttribute : Attribute
	{
		public readonly bool Spawnable = true;

		public SpawnableAttribute()
		{
			// NOP
		}

		public SpawnableAttribute(bool spawnable)
		{
			Spawnable = spawnable;
		}
	}
}
