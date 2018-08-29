using System;
using System.Collections.Generic;

using SFML.System;

namespace Tiler.GUI.Controls
{
	public class ControlList : Control
	{
		private VerticalScrollBar vscrollbar;
		private Panel canvas;

		private int GetCanvasChildrenSizeY()
		{
			int newY = 0;
			int yoffset = 0;

			foreach (var child in canvas.GetChildren())
			{
				child.Position = new Vector2i(0, yoffset);
				child.Size = new Vector2i(Math.Min(child.Size.X, Size.X - (vscrollbar.Enabled ? 13 : 0)), child.Size.Y);

				newY = Math.Max(newY, child.Position.Y + child.Size.Y);
				yoffset += child.Size.Y + YPadding;
			}

			return newY;
		}

		public int YPadding { get; set; } = 0;

		public ControlList()
		{
			RegisterEventTypes = EventType.MouseScroll;

			vscrollbar = new VerticalScrollBar()
			{
				Parent = this
			};

			canvas = new Panel()
			{
				Parent = this,
				Color = SFML.Graphics.Color.Transparent
			};
		}

		protected override void Layout()
		{
			var canvasHeight = GetCanvasChildrenSizeY();

			vscrollbar.Size = new Vector2i(13, Size.Y);
			vscrollbar.Position = new Vector2i(Size.X - vscrollbar.Size.X, 0);
			vscrollbar.Setup(Size.Y, canvasHeight);

			canvas.Position = new Vector2i(0, -vscrollbar.Scroll);
			canvas.Size = new Vector2i(Size.X - (vscrollbar.Enabled ? 13 : 0), canvasHeight);
		}

		public override IEnumerable<Control> GetChildren()
		{
			foreach (var child in canvas.GetChildren())
				yield return child;
		}

		protected override bool OnChildAdded(Control child)
		{
			if (canvas is null || child == vscrollbar || child == canvas)
				return true;

			canvas.AddChild(child);
			InvalidateLayout();

			return false;
		}

		protected override bool OnChildRemoved(Control child)
		{
			canvas.RemoveChild(child);
			InvalidateLayout();
			return false;
		}

		protected override Vector2i GetInternalSize()
		{
			return canvas.Size;
		}

		public override void OnMouseScroll()
		{
			vscrollbar.OnMouseScroll();
		}
	}
}
