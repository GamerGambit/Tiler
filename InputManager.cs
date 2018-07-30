using System;
using System.Collections.Generic;
using System.Numerics;

using Glfw3;

namespace Tiler.Input
{
	public class InputState
	{
		public Glfw.KeyCode? KeyboardKey;
		public Glfw.MouseButton? MouseButton;

		public bool IsDown;
		public bool WasPressed;
		public bool WasReleased;
		public float HoldTime;

		internal bool WasPreviouslyDown;

		internal InputState(Glfw.KeyCode key)
		{
			KeyboardKey = key;
			MouseButton = null;

			IsDown = false;
			WasPressed = false;
			WasReleased = false;
			HoldTime = 0;

			WasPreviouslyDown = false;
		}

		internal InputState(Glfw.MouseButton button)
		{
			MouseButton = button;
			KeyboardKey = null;

			IsDown = false;
			WasPressed = false;
			WasReleased = false;
			HoldTime = 0;

			WasPreviouslyDown = false;
		}
	}

	public static class Manager
	{
		internal static List<InputState> inputStates = new List<InputState>();

		private static Vector2 _mouseWheelDeltas = Vector2.Zero;
		private static bool _mouseWheelDeltasDirty = false;

		internal static Window Window = null;

		public static Vector2 MousePosition
		{
			get
			{
				Glfw.GetCursorPos(Window.GlfwWindow, out var xpos, out var ypos);
				return new Vector2((float)xpos, (float)ypos);
			}
			set
			{
				Glfw.SetCursorPos(Window.GlfwWindow, value.X, value.Y);
			}
		}
		public static Vector2 MouseWheelDeltas
		{
			get => _mouseWheelDeltas;
			set
			{
				_mouseWheelDeltas = value;
				_mouseWheelDeltasDirty = true;
			}
		}

		internal static (int, InputState) GetStateWithIndex(Glfw.KeyCode key)
		{
			for (var index = 0; index < inputStates.Count; ++index)
			{
				var state = inputStates[index];

				if (state.KeyboardKey == key)
					return (index, state);
			}

			var newState = new InputState(key);
			inputStates.Add(newState);
			return (inputStates.Count - 1, newState);
		}
		internal static (int, InputState) GetStateWithIndex(Glfw.MouseButton mouseButton)
		{
			for (var index = 0; index < inputStates.Count; ++index)
			{
				var state = inputStates[index];

				if (state.MouseButton == mouseButton)
					return (index, state);
			}

			var newState = new InputState(mouseButton);
			inputStates.Add(newState);
			return (inputStates.Count - 1, newState);
		}

		internal static void Update(TimeSpan deltaTime)
		{
			for (var index = 0; index < inputStates.Count; ++index)
			{
				var state = inputStates[index];

				if (state.IsDown)
				{
					state.HoldTime += (float)deltaTime.TotalSeconds;
				}

				if (state.IsDown != state.WasPreviouslyDown)
				{
					state.WasPreviouslyDown = state.IsDown;
					state.WasPressed = state.IsDown;
					state.WasReleased = !state.IsDown;
				}
				else
				{
					state.WasPressed = false;
					state.WasReleased = false;
				}

				if (state.WasReleased)
				{
					state.HoldTime = 0;
				}

				inputStates[index] = state;
			}

			if (_mouseWheelDeltasDirty)
			{
				_mouseWheelDeltasDirty = false;
			}
			else
			{
				_mouseWheelDeltas = Vector2.Zero;
			}
		}

		public static InputState GetState(Glfw.KeyCode key)
		{
			foreach (var state in inputStates)
			{
				if (state.KeyboardKey == key)
					return state;
			}

			return new InputState(key);
		}
		public static InputState GetState(Glfw.MouseButton mouseButton)
		{
			foreach (var state in inputStates)
			{
				if (state.MouseButton == mouseButton)
					return state;
			}

			return new InputState(mouseButton);
		}
	}
}
