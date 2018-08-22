using Glfw3;

using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI.Controls
{
	public class KeyBinder : Control
	{
		private Text text = new Text();
		private RectangleShape shape = new RectangleShape();

		private string Text {
			get => text.DisplayedString;
			set
			{
				text.DisplayedString = value;
				InvalidateLayout();
			}
		}

		public uint CharacterSize
		{
			get => text.CharacterSize;
			set
			{
				text.CharacterSize = value;
				InvalidateLayout();
			}
		}
		public Font Font
		{
			get => text.Font;
			set
			{
				text.Font = value;
				InvalidateLayout();
			}
		}
		public Glfw.KeyCode KeyCode { get; private set; } = Glfw.KeyCode.Unknown;

		public KeyBinder()
		{
			RegisterEventTypes = EventType.KeyPress;

			Text = "Press a Key";
			text.FillColor = Color.Black;

			shape.FillColor = Color.Transparent;
			shape.OutlineColor = new Color(100, 100, 100);
			shape.OutlineThickness = -2.0f;
		}

		public void SizeToContents(bool resizeX = true, bool resizeY = true)
		{
			if (Font is null)
				return;

			var gb = text.GetGlobalBounds();

			var newSize = Size;

			if (resizeX)
			{
				newSize.X = (int)(gb.Left + gb.Width) + 8;
			}

			if (resizeY)
			{
				newSize.Y = (int)(gb.Top + gb.Height) + 8;
			}

			Size = newSize;
			InvalidateLayout(true);
		}

		protected override void OnDraw(RenderTarget target, RenderStates states)
		{
			target.Draw(shape, states);
			target.Draw(text, states);
		}

		public override void OnKeyPressed(Glfw.KeyCode key, int scancode, Glfw.KeyMods modifiers)
		{
			var name = key.ToString();

			if (string.IsNullOrEmpty(name) || key == Glfw.KeyCode.Unknown)
			{
				KeyCode = Glfw.KeyCode.Unknown;
				Text = "Press a Key";
				return;
			}

			KeyCode = key;
			Text = name;
		}

		protected override void Layout()
		{
			if (Font is null)
				return;


			var gb = text.GetGlobalBounds();
			var width = (int)(gb.Left + gb.Width);

			shape.Size = new Vector2f(Size.X, Size.Y);
			text.Position = new Vector2f((Size.X - width) / 2 + 2, 2);
		}
	}
}
