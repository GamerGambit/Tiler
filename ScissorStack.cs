using System.Collections.Generic;

using SFML.Graphics;

namespace Tiler
{
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
			UtilsDrawing.SetScissor(renderTarget, top.Left, top.Top, top.Width, top.Height);
		}

		public static Stack<IntRect> Stack = new Stack<IntRect>();

		public static void Push(RenderTarget renderTarget, IntRect rect)
		{
			if (Stack.Count == 0)
			{
				Stack.Push(rect);
			}
			else
			{
				var top = Stack.Peek();

				rect.Left = Clamp(rect.Left, top.Left, top.Left + top.Width);
				rect.Top = Clamp(rect.Top, top.Top, top.Top + top.Height);
				rect.Width = Clamp(rect.Width, 0, top.Left + top.Width - rect.Left);
				rect.Height = Clamp(rect.Height, 0, top.Top + top.Height - rect.Top);

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

		public static void PushScissor(this RenderTarget target, IntRect rect)
		{
			Push(target, rect);
		}

		public static void PopScissor(this RenderTarget target)
		{
			Pop(target);
		}
	}
}
