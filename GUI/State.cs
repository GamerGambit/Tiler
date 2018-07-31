using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI
{
	internal static class State
	{
		public static List<Control> Roots = new List<Control>();

		public static bool HandleMousePressed(Glfw3.Glfw.MouseButton mouseButton)
		{
			for (var index = Roots.Count - 1; index >= 0; --index)
			{
				if (Roots[index].HandledMousePressed(mouseButton))
					return true;
			}

			return false;
		}
		public static bool HandleMouseReleased(Glfw3.Glfw.MouseButton mouseButton)
		{
			for (var index = Roots.Count - 1; index >= 0; --index)
			{
				if (Roots[index].HandledMouseReleased(mouseButton))
					return true;
			}

			return false;
		}
		public static bool HandleMouseScroll()
		{
			for (var index = Roots.Count - 1; index >= 0; --index)
			{
				if (Roots[index].HandledMouseScroll())
					return true;
			}

			return false;
		}
		public static bool HandleKeyPressed(Glfw3.Glfw.KeyCode key)
		{
			for (var index = Roots.Count - 1; index >= 0; --index)
			{
				if (Roots[index].HandledKeyPress(key))
					return true;
			}

			return false;
		}
		public static bool HandleKeyReleased(Glfw3.Glfw.KeyCode key)
		{
			for (var index = Roots.Count - 1; index >= 0; --index)
			{
				if (Roots[index].HandledKeyReleased(key))
					return true;
			}

			return false;
		}

		public static Vector2f GetRelativeOffset(RenderTarget target)
		{
			var view = target.GetView();
			return view.Center - view.Size / 2;
		}

		public static void Update(TimeSpan deltaTime)
		{
			for (var index = Roots.Count - 1; index >= 0; --index)
			{
				Roots[index].Update(deltaTime);
			}
		}

		public static void Draw(Window window)
		{
			var offset = GetRelativeOffset(window.RenderWindow);

			var transform = Transform.Identity;
			transform.Translate(offset);

			var states = RenderStates.Default;
			states.Transform *= transform;

			for (var index = 0; index < Roots.Count; ++index)
			{
				window.RenderWindow.Draw(Roots[index], states);
			}
		}
	}
}
