using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI.Controls
{
	public class Window : Control
	{
		private Panel TitlePanel;
		private Label TitleLabel;
		private Button CloseButton;
		private Panel Body;
		private Panel ClientArea;

		private bool dragging = false;
		private Vector2i mouseClickPos = new Vector2i(0, 0);

		public Font Font { get => TitleLabel.Font; set { TitleLabel.Font = value; CloseButton.Font = value; } }

		public event EventHandler Close;

		public Window()
		{
			TitlePanel = new Panel()
			{
				Parent = this,
				Color = new Color(100, 100, 100)
			};
			TitlePanel.RegisterEventTypes |= EventType.MousePress | EventType.MouseRelease;
			TitlePanel.MousePressed += (s, e) =>
			{
				if (e != Glfw3.Glfw.MouseButton.ButtonLeft)
					return;

				dragging = true;
				mouseClickPos = ScreenToLocal(new Vector2i((int)Input.Manager.MousePosition.X, (int)Input.Manager.MousePosition.Y));
			};
			TitlePanel.MouseReleased += (s, e) =>
			{
				if (e != Glfw3.Glfw.MouseButton.ButtonLeft)
					return;

				dragging = false;
			};

			TitleLabel = new Label()
			{
				Parent = this,
				Text = "Window",
				CharacterSize = 12,
				Position = new Vector2f(2, 2)
			};

			CloseButton = new Button()
			{
				Parent = this,
				Text = "X",
				CharacterSize = 10,
				FillColor = Color.Red
			};
			CloseButton.Click += (s, e) =>
			{
				if (e == Glfw3.Glfw.MouseButton.ButtonLeft)
				{
					CloseWindow();
				}
			};

			Body = new Panel()
			{
				Parent = this,
				Color = new Color(200, 200, 200),
				Position = new Vector2f(0, 20)
			};

			ClientArea = new Panel()
			{
				Parent = this,
				Position = new Vector2f(2, 22),
				Color = Color.Transparent
			};
		}

		public void CloseWindow()
		{
			OnClose();
			Close?.Invoke(this, EventArgs.Empty);
			Remove();
		}

		protected virtual void OnClose()
		{
			// NOP
		}

		protected override void OnUpdate(TimeSpan deltaTime)
		{
			if (dragging)
			{
				var mousePos = ScreenToLocal(new Vector2i((int)Input.Manager.MousePosition.X, (int)Input.Manager.MousePosition.Y));
				var diff = mousePos - mouseClickPos;
				Position += new Vector2f(diff.X, diff.Y);
			}
		}

		protected override void Layout()
		{
			TitlePanel.Size = new Vector2i(Size.X, 20);
			TitleLabel.SizeToContents();
			TitleLabel.Position = new Vector2f(2, 2);
			CloseButton.SizeToContents();
			CloseButton.Position = new Vector2f(Size.X - CloseButton.Size.X - 2, 2);
			Body.Size = new Vector2i(Size.X, Size.Y - 20);
			ClientArea.Size = new Vector2i(Size.X - 4, Size.Y - 20 - 4);
		}

		public override IEnumerable<Control> GetChildren()
		{
			foreach (var child in ClientArea.GetChildren())
				yield return child;
		}

		protected override bool OnChildAdded(Control child)
		{
			if (ClientArea is null || child == TitlePanel || child == TitleLabel || child == CloseButton || child == Body || child == ClientArea)
				return true;

			ClientArea.AddChild(child);
			return false;
		}

		protected override bool OnChildRemoved(Control child)
		{
			if (child == ClientArea || child == TitlePanel || child == TitleLabel || child == CloseButton || child == Body)
				return true;

			ClientArea.RemoveChild(child);
			return false;
		}

		protected override Vector2i GetInternalSize()
		{
			return ClientArea.Size;
		}
	}
}
