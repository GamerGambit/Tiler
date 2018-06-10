using System.Collections.Generic;

using SFML.System;
using SFML.Window;

namespace Tiler.Input
{
	public enum Type
	{
		Invalid,
		Keyboard,
		Mouse,
		Joystick
	}

	public struct State
	{
		public Type Type;
		public Keyboard.Key Key;
		public Mouse.Button Button;

		public bool IsDown;
		public bool WasPressed;
		public bool WasReleased;
		public float HoldTime;

		internal bool WasPreviouslyDown;

		internal State(Keyboard.Key key)
		{
			Type = Type.Keyboard;
			Key = key;
			Button = (Mouse.Button)(-1);

			IsDown = false;
			WasPressed = false;
			WasReleased = false;
			HoldTime = 0;

			WasPreviouslyDown = false;
		}

		internal State(Mouse.Button button)
		{
			Type = Type.Mouse;
			Button = button;
			Key = Keyboard.Key.Unknown;

			IsDown = false;
			WasPressed = false;
			WasReleased = false;
			HoldTime = 0;

			WasPreviouslyDown = false;
		}
	}

	public static class Manager
	{
		private static List<State> InputStates = new List<State>();
		private static bool MouseWheelDeltasDirty = false;
		private static Vector2f _MouseWheelDeltas = new Vector2f(0, 0);
		private static Vector2i _MousePosition = new Vector2i(0, 0);
		private static bool InputSubscribed(Mouse.Button button)
		{
			foreach (var state in InputStates)
			{
				if (state.Type == Type.Mouse && state.Button == button)
					return true;
			}

			return false;
		}
		private static bool InputSubscribed(Keyboard.Key key)
		{
			foreach (var state in InputStates)
			{
				if (state.Type == Type.Keyboard && state.Key == key)
					return true;
			}

			return false;
		}

		public static Window Window = null;
		public static Vector2i MouseOffset { get; private set; } = new Vector2i(0, 0);
		public static Vector2f MouseWheelDeltas
		{
			get
			{
				return _MouseWheelDeltas;
			}
			set
			{
				_MouseWheelDeltas = value;
				MouseWheelDeltasDirty = true;
			}
		}
		public static bool MouseMoved {
			get
			{
				return MouseOffset != new Vector2i(0, 0);
			}
		}
		public static Vector2i MousePosition
		{
			get
			{
				return _MousePosition;
			}
			set
			{
				if (Window is null)
				{
					Mouse.SetPosition(value);
				}
				else
				{
					Mouse.SetPosition(value, Window);
				}
			}
		}

		public static void SubscribeInput(Mouse.Button button)
		{
			if (InputSubscribed(button))
				return;

			InputStates.Add(new State(button));
		}
		public static void SubscribeInput(Keyboard.Key key)
		{
			if (InputSubscribed(key))
				return;

			InputStates.Add(new State(key));
		}
		public static void UnsubscribeInput(Mouse.Button button)
		{
			foreach (var state in InputStates)
			{
				if (state.Type == Type.Mouse && state.Button == button)
				{
					InputStates.Remove(state);
					return;
				}
			}
		}
		public static void UnsubscribeInput(Keyboard.Key key)
		{
			foreach (var state in InputStates)
			{
				if (state.Type == Type.Keyboard && state.Key == key)
				{
					InputStates.Remove(state);
					return;
				}
			}
		}

		public static void GrabMouseCursor(bool grab = true)
		{
			Window?.SetMouseCursorGrabbed(grab);
		}
		public static void HideMouseCursor(bool hide = true)
		{
			Window?.SetMouseCursorVisible(!hide);
		}

		public static State GetInputState(Mouse.Button button)
		{
			foreach (var state in InputStates)
			{
				if (state.Type == Type.Mouse && state.Button == button)
					return state;
			}

			return new State();
		}
		public static State GetInputState(Keyboard.Key key)
		{
			foreach (var state in InputStates)
			{
				if (state.Type == Type.Keyboard && state.Key == key)
					return state;
			}

			return new State();
		}

		public static bool WasHeldFor(Mouse.Button button, float seconds, bool resetOnTrue = true)
		{
			for (var index = 0; index < InputStates.Count; ++index)
			{
				var state = InputStates[index];

				if (state.Type == Type.Mouse && state.Button == button)
				{
					var result = state.HoldTime >= seconds;

					if (result && resetOnTrue)
					{
						state.HoldTime = 0;
						InputStates[index] = state;
					}

					return result;
				}
			}

			return false;
		}
		public static bool WasHeldFor(Keyboard.Key key, float seconds, bool resetOnTrue = true)
		{
			for (var index = 0; index < InputStates.Count; ++index)
			{
				var state = InputStates[index];

				if (state.Type == Type.Keyboard && state.Key == key)
				{
					var result = state.HoldTime >= seconds;

					if (result && resetOnTrue)
					{
						state.HoldTime = 0;
						InputStates[index] = state;
					}

					return result;
				}
			}

			return false;
		}

		public static void UpdateInputState(int index, float deltaTime)
		{
			var newState = InputStates[index];

			switch (newState.Type)
			{
			case Type.Keyboard:
				newState.IsDown = Keyboard.IsKeyPressed(newState.Key);
				break;

			case Type.Mouse:
				newState.IsDown = Mouse.IsButtonPressed(newState.Button);
				break;
			}

			if (newState.IsDown)
			{
				newState.HoldTime += deltaTime;
			}

			if (newState.IsDown != newState.WasPreviouslyDown)
			{
				newState.WasPreviouslyDown = newState.IsDown;

				newState.WasPressed = newState.IsDown;
				newState.WasReleased = !newState.IsDown;
			}
			else
			{
				newState.WasPressed = false;
				newState.WasReleased = false;
			}

			if (newState.WasReleased)
			{
				newState.HoldTime = 0;
			}

			InputStates[index] = newState;
		}
		public static void Update(float deltaTime)
		{
			Vector2i mousePos;

			if (Window is null)
			{
				mousePos = Mouse.GetPosition();
			}
			else
			{
				mousePos = Mouse.GetPosition(Window);
			}

			MouseOffset = mousePos - MousePosition;
			_MousePosition = mousePos;

			for (var index = 0; index < InputStates.Count; ++index)
			{
				UpdateInputState(index, deltaTime);
			}

			if (MouseWheelDeltasDirty)
			{
				MouseWheelDeltasDirty = false;
			}
			else
			{
				_MouseWheelDeltas.X = 0;
				_MouseWheelDeltas.Y = 0;
			}
		}
	}
}
