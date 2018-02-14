using System;

namespace Tiler
{
	public class MapEditableAttribute : Attribute
	{
		public bool MapEditable { get; set; }

		public MapEditableAttribute(bool mapEditable)
		{
			MapEditable = mapEditable;
		}
	}
}
