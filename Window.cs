using System;
using System.Collections.Generic;
using System.Numerics;

using Glfw3;

using SFML.Graphics;

namespace Tiler
{
	public struct KeyEventArgs
	{
		public Glfw.KeyCode Key;
		public int ScanCode;
		public Glfw.KeyMods Modifiers;
	}

	public struct UnicodeInputEventArgs
	{
		public uint CodePoint;
		public Glfw.KeyMods Modifiers;
	}

	public struct MouseEventArgs
	{
		public double X;
		public double Y;
	}

	public struct MouseButtonEventArgs
	{
		public Glfw.MouseButton Button;
		public Glfw.KeyMods Modifiers;
	}

	public struct DroppedEventArgs
	{
		public string[] Paths;
	}

	public struct WindowMovedEventArgs
	{
		public int X;
		public int Y;
	}

	public struct WindowResizedEventArgs
	{
		public int Width;
		public int Height;
	}

	public class Window
	{
		private static Dictionary<Glfw.Window, Window> dictionary = new Dictionary<Glfw.Window, Window>();

		#region event wrapper functions
		private static void KeyCallback(Glfw.Window window, Glfw.KeyCode key, int scancode, Glfw.InputState state, Glfw.KeyMods mods)
		{
			var engineWindow = dictionary[window];
			if (state == Glfw.InputState.Press)
			{
				engineWindow.KeyPressed?.Invoke(engineWindow, new KeyEventArgs()
				{
					Key = key,
					ScanCode = scancode,
					Modifiers = mods
				});
			}
			else if (state == Glfw.InputState.Release)
			{
				engineWindow.KeyReleased?.Invoke(engineWindow, new KeyEventArgs()
				{
					Key = key,
					ScanCode = scancode,
					Modifiers = mods
				});
			}
		}
		private static void UnicodeCallback(Glfw.Window window, uint codepoint, Glfw.KeyMods mods)
		{
			var engineWindow = dictionary[window];
			engineWindow.UnicodeInput?.Invoke(engineWindow, new UnicodeInputEventArgs()
			{
				CodePoint = codepoint,
				Modifiers = mods
			});
		}
		private static void MouseMoveCallback(Glfw.Window window, double xpos, double ypos)
		{
			var engineWindow = dictionary[window];
			engineWindow.MouseMoved?.Invoke(engineWindow, new MouseEventArgs()
			{
				X = xpos,
				Y = ypos
			});
		}
		private static void MouseButtonCallback(Glfw.Window window, Glfw.MouseButton button, Glfw.InputState state, Glfw.KeyMods mods)
		{
			var engineWindow = dictionary[window];
			if (state == Glfw.InputState.Press)
			{
				engineWindow.MousePressed?.Invoke(engineWindow, new MouseButtonEventArgs()
				{
					Button = button,
					Modifiers = mods
				});
			}
			else if (state == Glfw.InputState.Release)
			{
				engineWindow.MouseReleased?.Invoke(engineWindow, new MouseButtonEventArgs()
				{
					Button = button,
					Modifiers = mods
				});
			}
		}
		private static void MouseScrollCallback(Glfw.Window window, double xpos, double ypos)
		{
			var engineWindow = dictionary[window];
			engineWindow.MouseScrolled?.Invoke(engineWindow, new MouseEventArgs()
			{
				X = xpos,
				Y = ypos
			});
		}
		private static void CursorEnterCallback(Glfw.Window window, bool entered)
		{
			var engineWindow = dictionary[window];
			(entered ? engineWindow.MouseEntered : engineWindow.MouseExited)?.Invoke(engineWindow, EventArgs.Empty);
		}
		private static void DropCallback(Glfw.Window window, int count, string[] paths)
		{
			var engineWindow = dictionary[window];
			engineWindow.Dropped?.Invoke(engineWindow, new DroppedEventArgs()
			{
				Paths = paths
			});
		}
		private static void WindowCloseCallback(Glfw.Window window)
		{
			var engineWindow = dictionary[window];
			engineWindow.Closed?.Invoke(engineWindow, EventArgs.Empty);
		}
		private static void WindowMoveCallback(Glfw.Window window, int xpos, int ypos)
		{
			var engineWindow = dictionary[window];
			engineWindow.Moved?.Invoke(engineWindow, new WindowMovedEventArgs()
			{
				X = xpos,
				Y = ypos
			});
		}
		private static void WindowResizedCallback(Glfw.Window window, int width, int height)
		{
			var engineWindow = dictionary[window];

			/*
			{
				var view = new View(engineWindow.RenderWindow.GetView());
				engineWindow.RenderWindow?.Dispose();
				engineWindow.RenderWindow = new RenderWindow(engineWindow.NativeHandle);
				engineWindow.RenderWindow.SetView(view);
			}
			*/

			engineWindow.Resized?.Invoke(engineWindow, new WindowResizedEventArgs()
			{
				Width = width,
				Height = height
			});
		}
		#endregion

		private Window CurrentWindow = null;
		private string _title;

		#region events
		public event EventHandler<KeyEventArgs> KeyPressed;
		public event EventHandler<KeyEventArgs> KeyReleased;
		public event EventHandler<UnicodeInputEventArgs> UnicodeInput;
		public event EventHandler<MouseEventArgs> MouseMoved;
		public event EventHandler<MouseButtonEventArgs> MousePressed;
		public event EventHandler<MouseButtonEventArgs> MouseReleased;
		public event EventHandler<MouseEventArgs> MouseScrolled;
		public event EventHandler MouseEntered;
		public event EventHandler MouseExited;
		public event EventHandler<DroppedEventArgs> Dropped;
		public event EventHandler Closed;
		public event EventHandler<WindowMovedEventArgs> Moved;
		public event EventHandler<WindowResizedEventArgs> Resized;
		#endregion

		#region public api
		public Glfw.Window GlfwWindow { get; private set; }
		public RenderWindow RenderWindow { get; private set; }

		public bool IsOpen { get => !Glfw.WindowShouldClose(GlfwWindow); }
		public Vector2 Size
		{
			get
			{
				Glfw.GetWindowSize(GlfwWindow, out var x, out var y);
				return new Vector2(x, y);
			}
			set
			{
				Glfw.SetWindowSize(GlfwWindow, (int)value.X, (int)value.Y);
			}
		}
		public Vector2 Position
		{
			get
			{
				Glfw.GetWindowPos(GlfwWindow, out var x, out var y);
				return new Vector2(x, y);
			}
			set
			{
				Glfw.SetWindowPos(GlfwWindow, (int)value.X, (int)value.Y);
			}
		}
		public string Title
		{
			get => _title;
			set
			{
				_title = value;
				Glfw.SetWindowTitle(GlfwWindow, _title);
			}
		}
		public IntPtr NativeHandle { get => Glfw.glfwGetWin32Window(GlfwWindow.Ptr); }

		public Window(int width, int height, string title)
		{
			// @todo style flags
			// @todo fullscreen support
			// @todo monitor support

			Glfw.DefaultWindowHints();

			// context hints
			Glfw.WindowHint(Glfw.Hint.ClientApi, Glfw.ClientApi.None);
			/*
			Glfw.WindowHint(Glfw.Hint.ClientApi, Glfw.ClientApi.OpenGL);
			Glfw.WindowHint(Glfw.Hint.ContextVersionMajor, 4);
			Glfw.WindowHint(Glfw.Hint.ContextVersionMinor, 5);
			Glfw.WindowHint(Glfw.Hint.OpenglForwardCompat, true);
			Glfw.WindowHint(Glfw.Hint.OpenglProfile, Glfw.OpenGLProfile.Core);
			*/

			Glfw.WindowHint(Glfw.Hint.Resizable, false);

			_title = title;
			GlfwWindow = Glfw.CreateWindow(width, height, title);

			if (!GlfwWindow)
				throw new Exception("Failed to create GLFW window");

			RenderWindow = new RenderWindow(NativeHandle);

			dictionary.Add(GlfwWindow, this);

			Glfw.SetKeyCallback(GlfwWindow, Utils.Pin<Glfw.KeyFunc>(KeyCallback));
			Glfw.SetCharModsCallback(GlfwWindow, Utils.Pin<Glfw.CharModsFunc>(UnicodeCallback));
			Glfw.SetCursorPosCallback(GlfwWindow, Utils.Pin<Glfw.CursorPosFunc>(MouseMoveCallback));
			Glfw.SetMouseButtonCallback(GlfwWindow, Utils.Pin<Glfw.MouseButtonFunc>(MouseButtonCallback));
			Glfw.SetScrollCallback(GlfwWindow, Utils.Pin<Glfw.CursorPosFunc>(MouseScrollCallback));
			Glfw.SetCursorEnterCallback(GlfwWindow, Utils.Pin<Glfw.CursorEnterFunc>(CursorEnterCallback));
			Glfw.SetDropCallback(GlfwWindow, Utils.Pin<Glfw.DropFunc>(DropCallback));
			Glfw.SetWindowCloseCallback(GlfwWindow, Utils.Pin<Glfw.WindowCloseFunc>(WindowCloseCallback));
			Glfw.SetWindowPosCallback(GlfwWindow, Utils.Pin<Glfw.WindowPosFunc>(WindowMoveCallback));
			Glfw.SetWindowSizeCallback(GlfwWindow, Utils.Pin<Glfw.WindowSizeFunc>(WindowResizedCallback));
		}

		~Window()
		{
			dictionary.Remove(GlfwWindow);
			Glfw.DestroyWindow(GlfwWindow);
		}

		public void Activate()
		{
			if (CurrentWindow != this)
			{
				Glfw.MakeContextCurrent(GlfwWindow);
				CurrentWindow = this;
			}
		}

		public void Minimize()
		{
			Glfw.IconifyWindow(GlfwWindow);
		}

		public void Restore()
		{
			Glfw.RestoreWindow(GlfwWindow);
		}

		public void Maximize()
		{
			Glfw.MaximizeWindow(GlfwWindow);
		}

		public void Show()
		{
			Glfw.ShowWindow(GlfwWindow);
		}

		public void Hide()
		{
			Glfw.HideWindow(GlfwWindow);
		}

		public void Close()
		{
			Glfw.SetWindowShouldClose(GlfwWindow, true);
		}

		public void Clear()
		{
			RenderWindow.Clear();
		}

		public void Draw(Drawable drawable)
		{
			RenderWindow.Draw(drawable);
		}

		public void Display()
		{
			RenderWindow.Display();
		}
		#endregion
	}
}
