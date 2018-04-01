using System.Collections.Generic;

using SFML.System;
using SFML.Window;

namespace Tiler
{
	public class InputState
	{
		public enum InputType
		{
			Invalid,
			Keyboard,
			Mouse,
			Joystick
		}

		public InputType Type;
		public Keyboard.Key Key;
		public Mouse.Button Button;

		public bool IsDown;
		public bool WasPressed;
		public bool WasReleased;
		public float HoldTime;

		private bool WasPreviouslyDown;

		internal InputState(int i)
		{
			Type = InputType.Invalid;
			Key = (Keyboard.Key)(-1);
			Button = (Mouse.Button)(-1);

			IsDown = false;
			WasPressed = false;
			WasReleased = false;
			HoldTime = 0;

			WasPreviouslyDown = false;
		}

		internal InputState(Keyboard.Key key)
		{
			Type = InputType.Keyboard;
			Key = key;
			Button = (Mouse.Button)(-1);

			IsDown = false;
			WasPressed = false;
			WasReleased = false;
			HoldTime = 0;

			WasPreviouslyDown = false;
		}

		internal InputState(Mouse.Button button)
		{
			Type = InputType.Mouse;
			Button = button;
			Key = (Keyboard.Key)(-1);

			IsDown = false;
			WasPressed = false;
			WasReleased = false;
			HoldTime = 0;

			WasPreviouslyDown = false;
		}

		public bool WasHeldFor(float seconds, bool resetOnTrue = true)
		{
			var result = HoldTime >= seconds;

			if (result && resetOnTrue)
			{
				HoldTime = 0;
			}

			return result;
		}

		internal void Update(float deltaTime)
		{
			switch (Type)
			{
			case InputType.Keyboard:
				IsDown = Keyboard.IsKeyPressed(Key);
				break;

			case InputType.Mouse:
				IsDown = Mouse.IsButtonPressed(Button);
				break;
			}

			if (IsDown)
			{
				HoldTime += deltaTime;
			}

			if (IsDown != WasPreviouslyDown)
			{
				WasPreviouslyDown = IsDown;

				WasPressed = IsDown;
				WasReleased = !IsDown;
			}
			else
			{
				WasPressed = false;
				WasReleased = false;
			}

			if (WasReleased)
			{
				HoldTime = 0;
			}
		}
	}

	public static class Input
	{
		private static List<InputState> InputStates = new List<InputState>();
		private static Vector2i _MousePosition = new Vector2i(0, 0);
		private static bool InputSubscribed(Mouse.Button button)
		{
			foreach (var state in InputStates)
			{
				if (state.Type == InputState.InputType.Mouse && state.Button == button)
					return true;
			}

			return false;
		}
		private static bool InputSubscribed(Keyboard.Key key)
		{
			foreach (var state in InputStates)
			{
				if (state.Type == InputState.InputType.Keyboard && state.Key == key)
					return true;
			}

			return false;
		}

		public static Window Window = null;
		public static Vector2i MouseOffset { get; private set; } = new Vector2i(0, 0);
		public static Vector2f MouseWheelDeltas = new Vector2f(0, 0);
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

			InputStates.Add(new InputState(button));
		}
		public static void SubscribeInput(Keyboard.Key key)
		{
			if (InputSubscribed(key))
				return;

			InputStates.Add(new InputState(key));
		}
		public static void UnsubscribeInput(Mouse.Button button)
		{
			foreach (var state in InputStates)
			{
				if (state.Type == InputState.InputType.Mouse && state.Button == button)
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
				if (state.Type == InputState.InputType.Keyboard && state.Key == key)
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

		public static InputState GetInputState(Mouse.Button button)
		{
			foreach (var state in InputStates)
			{
				if (state.Type == InputState.InputType.Mouse && state.Button == button)
					return state;
			}

			return new InputState(0);
		}
		public static InputState GetInputState(Keyboard.Key key)
		{
			foreach (var state in InputStates)
			{
				if (state.Type == InputState.InputType.Keyboard && state.Key == key)
					return state;
			}

			return new InputState(0);
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

			foreach (var state in InputStates)
			{
				state.Update(deltaTime);
			}

			MouseWheelDeltas.X = 0;
			MouseWheelDeltas.Y = 0;
		}
	}
}
