using System;

namespace Tiler
{
	public class MapEditableAttribute : Attribute
	{
		public readonly bool MapEditable = true;

		public MapEditableAttribute()
		{
			// NOP
		}

		public MapEditableAttribute(bool mapEditable)
		{
			MapEditable = mapEditable;
		}
	}
}
