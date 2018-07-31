using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI.Controls
{
	public class Panel : Control
	{
		private RectangleShape rectShape;
		public new Vector2f Size
		{
			get => base.Size;
			set
			{
				if (!(rectShape is null))
				{
					rectShape.Size = new Vector2f(value.X, value.Y);
				}

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
			Size = new Vector2f(250, 100);
			rectShape = new RectangleShape(Size)
			{
				FillColor = new Color(0, 128, 255, 100),
				OutlineColor = Color.Black,
				OutlineThickness = 0.1f
			};
		}

		protected override void OnDraw(RenderTarget target, RenderStates states)
		{
			target.Draw(rectShape, states);
		}
	}
}
