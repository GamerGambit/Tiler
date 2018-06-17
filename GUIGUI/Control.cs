using System;
using System.Collections.Generic;
using System.Numerics;

namespace GUIGUI
{
	public class Control
	{
		public List<Control> Children = new List<Control>();

		public Vector2 Position;
		public Vector2 Size;
		public byte R, G, B, A;

		public virtual void Draw(Painter P)
		{
			throw new InvalidOperationException();
		}
	}
}
