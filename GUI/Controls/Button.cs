using System;

using Glfw3;

using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI.Controls
{
	public class Button : Control
	{
		private Label label;
		private RectangleShape shape = new RectangleShape();

		public new Vector2f Size { get => base.Size; set { base.Size = value; shape.Size = value; } }
		public string Text { get => label.String; set => label.String = value; }
		public Font Font { get => label.Font; set => label.Font = value; }
		public uint CharacterSize { get => label.CharacterSize; set => label.CharacterSize = value; }
		public Color TextColor { get => label.FillColor; set => label.FillColor = value; }
		public Color FillColor { get => shape.FillColor; set => shape.FillColor = value; }
		public Color OutlineColor { get => shape.OutlineColor; set => shape.OutlineColor = value; }
		public float OutlineThickness { get => shape.OutlineThickness; set => shape.OutlineThickness = value; }

		public event EventHandler<Glfw.MouseButton> Click;

		public Button(Control parent) : base(parent)
		{
			label = new Label(this)
			{
				Position = new Vector2f(2, 2),
				String = "Button"
			};
			SizeToContents();
		}

		public void SizeToContents()
		{
			label.SizeToContents();
			Size = label.Size + new Vector2f(4, 4);
		}

		protected override void OnDraw(RenderTarget target, RenderStates states)
		{
			target.Draw(shape, states);
		}

		protected override void OnMouseReleased(Glfw.MouseButton mouseButton)
		{
			OnClick(mouseButton);
			Click?.Invoke(this, mouseButton);
		}

		protected virtual void OnClick(Glfw.MouseButton mouseButton)
		{
			// NOP
		}
	}
}
