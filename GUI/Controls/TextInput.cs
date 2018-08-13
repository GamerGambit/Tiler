using System;

using Glfw3;

using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI.Controls
{
	public class TextInput : Control
	{
		private RectangleShape rect = new RectangleShape();
		private RectangleShape caret = new RectangleShape();
		private Text label;
		private float backspaceAccumulator = 0.0f;
		private float moveAccumulator = 0.0f;
		private int caretPos = 0;

		private int CaretPos {
			get => caretPos;
			set
			{
				caretPos = Utils.Clamp(value, 0, Text.Length);
				InvalidateLayout();
			}
		}

		private void SetText(string newText, bool trim = true)
		{
			var str = newText.Replace("\r", "").Replace("\n", "");

			if (trim)
			{
				str = str.Trim();
			}

			label.DisplayedString = str;
			InvalidateLayout();
		}

		private void CalculateLabelPosition()
		{
			if (Text.Length == 0 || CaretPos == 0)
			{
				label.Position = new Vector2f(4, 2);
				return;
			}

			var characterPosAtCaretX = label.FindCharacterPos((uint)CaretPos).X + 4;
			var diff = (characterPosAtCaretX + label.Position.X) - Size.X;
			if (diff > 0)
			{
				label.Position -= new Vector2f(diff, 0);
			}
			else if (diff < -Size.X)
			{
				label.Position += new Vector2f(Math.Abs(diff) - Size.X + 8, 0);
			}
		}

		private void Backspace()
		{
			if (Text.Length == 0 || CaretPos == 0)
				return;

			SetText(Text.Remove(CaretPos - 1, 1), false);
			CaretPos--;
		}

		public string Text { get => label.DisplayedString; set { SetText(""); CaretPos = 0;} }
		public Font Font { get => label.Font; set { label.Font = value; InvalidateLayout(); } }
		public uint CharacterSize { get => label.CharacterSize; set { label.CharacterSize = value; InvalidateLayout(); } }
		public uint MaxCharacters { get; set; } = 0;

		public event EventHandler Submit;

		public TextInput()
		{
			RegisterEventTypes = EventType.MousePress | EventType.KeyPress | EventType.TextEntered;

			label = new Text()
			{
				Position = new Vector2f(4, 2),
				FillColor = new Color(50, 50, 50)
			};

			rect.FillColor = new Color(150, 150, 150);
			rect.OutlineColor = new Color(100, 100, 100);
			rect.OutlineThickness = -2.0f;

			caret.FillColor = Color.Black;
		}

		protected override void OnUpdate(TimeSpan deltaTime)
		{
			backspaceAccumulator += (float)deltaTime.TotalSeconds;
			moveAccumulator += (float)deltaTime.TotalSeconds;

			if ((Input.Manager.GetState(Glfw.KeyCode.Backspace).IsDown || Input.Manager.GetState(Glfw.KeyCode.Delete).IsDown)&& backspaceAccumulator >= 0.05f)
			{
				if (Input.Manager.GetState(Glfw.KeyCode.Delete).IsDown)
				{
					if (CaretPos >= Text.Length)
						return;

					CaretPos++;
				}

				Backspace();
				backspaceAccumulator = 0.0f;
			}

			if (Input.Manager.GetState(Glfw.KeyCode.Left).IsDown && moveAccumulator >= 0.05f)
			{
				CaretPos--;
				moveAccumulator = 0.0f;
			}
			
			if (Input.Manager.GetState(Glfw.KeyCode.Right).IsDown && moveAccumulator >= 0.05f)
			{
				CaretPos++;
				moveAccumulator = 0.0f;
			}
		}

		protected override void OnDraw(RenderTarget target, RenderStates states)
		{
			target.Draw(rect, states);
			target.Draw(label, states);

			if (HasFocus && Math.Sin(Program.ElapsedTime.TotalSeconds * 5) >= 0)
			{
				target.Draw(caret, states);
			}
		}

		public override void OnMousePressed(Glfw.MouseButton mouseButton, Glfw.KeyMods modifiers)
		{
			if (mouseButton != Glfw.MouseButton.ButtonLeft)
				return;

			var clickPosX = ScreenToLocal(new Vector2i((int)Input.Manager.MousePosition.X, (int)Input.Manager.MousePosition.Y)).X;
			int lastPos = -1;

			for (uint index = 0; index < Text.Length; ++index)
			{
				var charPosX = (label.FindCharacterPos(index) - label.Position).X;

				if (charPosX > clickPosX)
				{
					lastPos = (int)index - 1;
					break;
				}
			}

			if (lastPos == -1)
			{
				lastPos = Text.Length;
			}

			CaretPos = lastPos;
		}

		public override void OnKeyPressed(Glfw.KeyCode key, Glfw.KeyMods modifiers)
		{
			if (key == Glfw.KeyCode.Backspace)
			{
				backspaceAccumulator = -0.5f;
				Backspace();
			}
			else if (key == Glfw.KeyCode.Delete)
			{
				if (CaretPos >= Text.Length)
					return;

				CaretPos++;
				backspaceAccumulator = -0.5f;
				Backspace();
			}
			else if (key == Glfw.KeyCode.Left || key == Glfw.KeyCode.Right)
			{
				moveAccumulator = -0.5f;

				if (key == Glfw.KeyCode.Right)
				{
					CaretPos++;
				}
				else
				{
					CaretPos--;
				}
			}
			else if (key == Glfw.KeyCode.Home)
			{
				CaretPos = 0;
			}
			else if (key == Glfw.KeyCode.End)
			{
				CaretPos = Text.Length;
			}
			else if (key == Glfw.KeyCode.Enter || key == Glfw.KeyCode.NumpadEnter)
			{
				OnSubmit();
				Submit?.Invoke(this, EventArgs.Empty);
			}
			else if (modifiers.HasFlag(Glfw.KeyMods.Control))
			{
				switch (key)
				{
				case Glfw.KeyCode.Z:
					// TODO: undo
					break;

				case Glfw.KeyCode.Y:
					// TODO :redo
					break;

				case Glfw.KeyCode.V:
					SetText(Text.Insert(CaretPos, Glfw.GetClipboardString(Input.Manager.Window.GlfwWindow)));
					CaretPos = Text.Length;
					break;

				case Glfw.KeyCode.C:
				case Glfw.KeyCode.X:
					// TODO: add selecting
					Glfw.SetClipboardString(Input.Manager.Window.GlfwWindow, Text);

					if (key == Glfw.KeyCode.X)
					{
						Text = "";
					}

					break;
				}
			}
		}

		public override void OnTextEntered(uint codepoint, Glfw.KeyMods modifiers)
		{
			if (MaxCharacters > 0 && Text.Length >= MaxCharacters)
				return;

			var ch = char.ConvertFromUtf32(Convert.ToInt32(codepoint));

			SetText(Text.Insert(CaretPos, ch.ToString()), false);
			CaretPos++;
		}

		protected override void Layout()
		{
			CalculateLabelPosition();
			caret.Position = new Vector2f(label.Position.X + label.FindCharacterPos((uint)CaretPos).X, 4);

			if (!(label.Font is null))
			{
				var lineSpacing = (int)label.Font.GetLineSpacing(CharacterSize);
				caret.Size = new Vector2f(1, lineSpacing);
				Size = new Vector2i(Size.X, lineSpacing + 8);
			}

			rect.Size = new Vector2f(Size.X, Size.Y);
		}

		protected virtual void OnSubmit()
		{
			// NOP
		}
	}
}
