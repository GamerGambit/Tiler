using System;
using System.Collections.Generic;
using SFML.Graphics;
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

		private static void Apply(RenderTarget renderTarget)
		{
			if (Stack.Count == 0)
			{
				UtilsDrawing.EnableScissor(false);
				return;
			}

			var top = Stack.Peek();
			UtilsDrawing.EnableScissor(true);
			UtilsDrawing.SetScissor(renderTarget, top.Position.X, top.Position.Y, top.Size.X, top.Size.Y);
		}

		public static Stack<ScissorRect> Stack = new Stack<ScissorRect>();

		public static void Push(RenderTarget renderTarget, ScissorRect rect)
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
					Clamp(rect.Size.X, 0, top.Position.X + top.Size.X - rect.Position.X),
					Clamp(rect.Size.Y, 0, top.Position.Y + top.Size.Y - rect.Position.Y)
				);

				Stack.Push(rect);
			}

			Apply(renderTarget);
		}

		public static void Pop(RenderTarget renderTarget)
		{
			if (Stack.Count == 0)
				return;

			Stack.Pop();
			Apply(renderTarget);
		}
	}
}
