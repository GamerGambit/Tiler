using System;

using SFML.Graphics;

namespace Tiler.GUI.Controls
{
	public class VerticalScrollBar : Control
	{
		private RectangleShape rect;
		private RectangleShape grip;

		private int barHeight = 0;
		private int canvasHeight = 0;
		private float scroll = 0;
		private bool enabled = true;

		public float Scroll { get => scroll; private set { scroll = Utils.Clamp(value, 0, canvasHeight); Parent?.InvalidateLayout(); } }
		public bool Enabled { get => enabled; private set { enabled = value; Visible = value; } }
		public float BarScale {
			get
			{
				if (barHeight == 0)
					return 1;

				return (float)barHeight / (canvasHeight + barHeight);
			}
		}

		public VerticalScrollBar()
		{
			rect = new RectangleShape()
			{
				FillColor = new Color(50, 50, 50)
			};

			grip = new RectangleShape()
			{
				FillColor = new Color(100, 100, 100)
			};
		}

		public void Setup(int barHeight, int canvasHeight)
		{
			this.barHeight = barHeight;
			this.canvasHeight = Math.Max(canvasHeight - barHeight, 1);
			Enabled = canvasHeight > barHeight;
			InvalidateLayout();
		}

		protected override void OnDraw(RenderTarget target, RenderStates states)
		{
			target.Draw(rect, states);
			target.Draw(grip, states);
		}

		protected override void OnMouseScroll()
		{
			if (!Enabled)
				return;

			Scroll += ((int)Input.Manager.MouseWheelDeltas.Y * -2);
		}

		protected override void Layout()
		{
			rect.Size = new SFML.System.Vector2f(Size.X, Size.Y);

			var gripHeight = Math.Max(BarScale * Size.Y, 10);
			var track = (Size.Y - gripHeight) + 1;
			var scroll = (Scroll / canvasHeight) * track;

			grip.Position = new SFML.System.Vector2f(0, scroll);
			grip.Size = new SFML.System.Vector2f(Size.X, gripHeight);
		}
	}
}
