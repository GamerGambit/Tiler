﻿using System;

using Glfw3;

using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI.Controls
{
	public class VerticalScrollBar : Control
	{
		private RectangleShape rect;
		private RectangleShape grip;

		private int barHeight = 0;
		private int canvasHeight = 0;
		private int scroll = 0;

		private bool dragging = false;
		private Vector2i mouseClickPos = new Vector2i(0, 0);

		public int Scroll { get => scroll; private set { scroll = Utils.Clamp(value, 0, canvasHeight); Parent?.InvalidateLayout(); } }
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
			RegisterEventTypes = EventType.MouseScroll | EventType.MousePress | EventType.MouseRelease;

			rect = new RectangleShape()
			{
				FillColor = new Color(100, 100, 100)
			};

			grip = new RectangleShape()
			{
				FillColor = new Color(150, 150, 150),
				OutlineColor = new Color(50, 50, 50),
				OutlineThickness = -2.0f
			};
		}

		public void Setup(int barHeight, int canvasHeight)
		{
			this.barHeight = barHeight;
			this.canvasHeight = Math.Max(canvasHeight - barHeight, 1);
			Enabled = canvasHeight > barHeight;
			InvalidateLayout();
		}

		protected override void OnUpdate(TimeSpan deltaTime)
		{
			if (dragging)
			{
				var mousePos = ScreenToLocal(new Vector2i((int)Input.Manager.MousePosition.X, (int)Input.Manager.MousePosition.Y));
				var diff = mousePos - mouseClickPos;
				Scroll += (int)(diff.Y / BarScale);
				mouseClickPos = mousePos;
			}
		}

		protected override void OnDraw(RenderTarget target, RenderStates states)
		{
			target.Draw(rect, states);
			target.Draw(grip, states);
		}

		public override void OnMousePressed(Glfw.MouseButton mouseButton, Glfw.KeyMods modifiers)
		{
			if (mouseButton != Glfw.MouseButton.ButtonLeft)
				return;

			var mousePos = ScreenToLocal(new Vector2i((int)Input.Manager.MousePosition.X, (int)Input.Manager.MousePosition.Y));

			if (grip.GetGlobalBounds().Contains(mousePos.X, mousePos.Y))
			{
				dragging = true;
				mouseClickPos = mousePos;
			}
		}

		public override void OnMouseReleased(Glfw.MouseButton mouseButton, Glfw.KeyMods modifiers)
		{
			if (mouseButton != Glfw.MouseButton.ButtonLeft)
				return;

			dragging = false;
		}

		public override void OnMouseScroll()
		{
			if (!Enabled)
				return;

			Scroll += (int)Input.Manager.MouseWheelDeltas.Y * -50;
		}

		protected override void Layout()
		{
			rect.Size = new Vector2f(Size.X, Size.Y);

			var gripHeight = Math.Max(BarScale * (Size.Y - 1), 10);
			var track = (Size.Y - 3 - gripHeight) + 1;
			var scroll = (Scroll / canvasHeight) * track;

			grip.Position = new Vector2f(1, 1 + scroll);
			grip.Size = new Vector2f(Size.X - 2, gripHeight);
		}

		protected override void EnabledChanged()
		{
			Visible = Enabled;
		}
	}
}
