using System;
using System.Collections.Generic;

using Glfw3;

using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI.Controls
{
	public class Button : Control
	{
		private Label label;
		private RectangleShape shape = new RectangleShape();

		public string Text { get => label.Text; set => label.Text = value; }
		public Font Font { get => label.Font; set => label.Font = value; }
		public uint CharacterSize { get => label.CharacterSize; set => label.CharacterSize = value; }
		public Color TextColor { get => label.FillColor; set => label.FillColor = value; }
		public Color FillColor { get => shape.FillColor; set => shape.FillColor = value; }
		public Color OutlineColor { get => shape.OutlineColor; set => shape.OutlineColor = value; }
		public float OutlineThickness { get => shape.OutlineThickness; set => shape.OutlineThickness = value; }

		public event EventHandler<MouseButtonEventArgs> Click;

		public Button()
		{
			RegisterEventTypes = EventType.MousePress | EventType.MouseRelease;

			label = new Label()
			{
				Position = new Vector2i(2, 2),
				Text = "Button"
			};
			AddChild(label);
			SizeToContents();
		}

		public void SizeToContents()
		{
			label.SizeToContents();
			Size = label.Size + new Vector2i(4, 4);
		}

		protected override void Layout()
		{
			shape.Size = new Vector2f(Size.X, Size.Y);
			label.Size = Size - new Vector2i(4, 4);
		}

		protected override void OnDraw(RenderTarget target, RenderStates states)
		{
			target.Draw(shape, states);
		}

		public override void OnMouseReleased(Glfw.MouseButton mouseButton, Glfw.KeyMods modifiers)
		{
			OnClick(mouseButton);
			Click?.Invoke(this, new MouseButtonEventArgs()
			{
				Button = mouseButton,
				Modifiers = modifiers
			});
		}

		public virtual void OnClick(Glfw.MouseButton mouseButton)
		{
			// NOP
		}

		public override IEnumerable<Control> GetChildren()
		{
			yield break;
		}
	}
}
