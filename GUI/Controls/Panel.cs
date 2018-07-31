using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI.Controls
{
	public class Panel : Control
	{
		private RectangleShape rectShape = new RectangleShape();
		public new Vector2f Size
		{
			get => base.Size;
			set
			{
				rectShape.Size = new Vector2f(value.X, value.Y);
				base.Size = value;
			}
		}

		public Color Color
		{
			get => rectShape.FillColor;
			set => rectShape.FillColor = value;
		}

		public Panel(Control parent = null) : base(parent)
		{
			HandlesKeyboardInput = false;
			HandlesMouseInput = false;

			Size = new Vector2f(250, 100);
			rectShape.FillColor = new Color(0, 128, 255, 100);
			rectShape.OutlineColor = Color.Black;
			rectShape.OutlineThickness = 0.1f;
		}

		protected override void OnDraw(RenderTarget target, RenderStates states)
		{
			target.Draw(rectShape, states);
		}
	}
}
