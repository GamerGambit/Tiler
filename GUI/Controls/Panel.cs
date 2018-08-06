using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI.Controls
{
	public class Panel : Control
	{
		private RectangleShape rectShape = new RectangleShape();
		public new Vector2i Size
		{
			get => base.Size;
			set
			{
				base.Size = value;
				rectShape.Size = new Vector2f(base.Size.X, base.Size.Y);
			}
		}

		public Color Color
		{
			get => rectShape.FillColor;
			set => rectShape.FillColor = value;
		}

		public Panel()
		{
			Size = new Vector2i(250, 100);
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
