using System;
using System.Collections.Generic;

namespace Tiler.GUI
{
	internal static class State
	{
		public static List<Control> Roots = new List<Control>();
		public static Control FocusedControl = null;

		public static bool Focused { get => FocusedControl?.HasFocus ?? false; }

		public static bool HandleMousePressed(MouseButtonEventArgs e)
		{
			for (var index = Roots.Count - 1; index >= 0; --index)
			{
				if (Roots[index].HandledMousePressed(e))
					return true;
			}

			return false;
		}
		public static bool HandleMouseReleased(MouseButtonEventArgs e)
		{
			for (var index = Roots.Count - 1; index >= 0; --index)
			{
				if (Roots[index].HandledMouseReleased(e))
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
		public static bool HandleKeyPressed(KeyEventArgs e)
		{
			for (var index = Roots.Count - 1; index >= 0; --index)
			{
				if (Roots[index].HandledKeyPress(e))
					return true;
			}

			return false;
		}
		public static bool HandleKeyReleased(KeyEventArgs e)
		{
			for (var index = Roots.Count - 1; index >= 0; --index)
			{
				if (Roots[index].HandledKeyReleased(e))
					return true;
			}

			return false;
		}
		public static bool HandleTextInput(UnicodeInputEventArgs e)
		{
			for (var index = Roots.Count - 1; index >= 0; --index)
			{
				if (Roots[index].HandledTextInput(e))
					return true;
			}

			return false;
		}

		public static void Update(TimeSpan deltaTime)
		{
			for (var index = Roots.Count - 1; index >= 0; --index)
			{
				Roots[index].Update(deltaTime);
			}

			if (!(FocusedControl is null) && !FocusedControl.HasFocus || Roots.Count == 0)
			{
				FocusedControl = null;
			}
		}

		public static void Draw(Window window)
		{
			for (var index = 0; index < Roots.Count; ++index)
			{
				window.RenderWindow.Draw(Roots[index]);
			}
		}
	}
}
