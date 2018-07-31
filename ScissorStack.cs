using System;
using System.Collections.Generic;

using SFML.System;

namespace Tiler
{
	public struct ScissorRect
	{
		public Vector2i Position;
		public Vector2i Size;
	}

	public static class ScissorStack
	{
		private static int Clamp(int value, int min, int max)
		{
			if (value < min)
				return min;

			if (value > max)
				return max;

			return value;
		}

		private static void Apply()
		{
			if (Stack.Count == 0)
			{
				UtilsDrawing.EnableScissor(false);
				return;
			}

			if (RenderTarget is null)
				throw new Exception("RenderTarget not set");

			var top = Stack.Peek();
			UtilsDrawing.EnableScissor(true);
			UtilsDrawing.SetScissor(RenderTarget, top.Position.X, top.Position.Y, top.Size.X, top.Size.Y);
		}

		public static SFML.Graphics.RenderTarget RenderTarget { get; set; } = null;
		public static Stack<ScissorRect> Stack = new Stack<ScissorRect>();

		public static void Push(ScissorRect rect)
		{
			if (Stack.Count == 0)
			{
				Stack.Push(rect);
			}
			else
			{
				var top = Stack.Peek();
				rect.Position = new Vector2i(
					Clamp(rect.Position.X, top.Position.X, top.Position.X + top.Size.X),
					Clamp(rect.Position.Y, top.Position.Y, top.Position.Y + top.Size.Y)
				);
				rect.Size = new Vector2i(
					Math.Min(rect.Size.X, top.Position.X + top.Size.X - rect.Position.X),
					Math.Min(rect.Size.Y, top.Position.Y + top.Size.Y - rect.Position.Y)
				);

				Stack.Push(rect);
			}

			Apply();
		}

		public static void Pop()
		{
			if (Stack.Count == 0)
				return;

			Stack.Pop();
			Apply();
		}
	}
}
