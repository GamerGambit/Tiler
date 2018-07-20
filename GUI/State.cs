using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI
{
	internal static class State
	{
		public static List<Control> Roots = new List<Control>();

		public static Vector2f GetRelativeOffset(RenderTarget target)
		{
			var view = target.GetView();
			return view.Center - view.Size / 2;
		}

		public static void Draw(Window window)
		{
			var offset = GetRelativeOffset(window.RenderWindow);

			var transform = Transform.Identity;
			transform.Translate(offset);

			var states = RenderStates.Default;
			states.Transform *= transform;

			UtilsDrawing.EnableScissor(true);

			foreach (var root in Roots)
			{
				window.RenderWindow.Draw(root, states);
			}

			UtilsDrawing.SetScissor(window.RenderWindow, (int)offset.X, (int)offset.Y, (int)window.RenderWindow.Size.X, (int)window.RenderWindow.Size.Y);
			UtilsDrawing.EnableScissor(false);
		}
	}
}
