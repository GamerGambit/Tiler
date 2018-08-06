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

		public Font Font { get => TitleLabel.Font; set { TitleLabel.Font = value; CloseButton.Font = value; } }

		public event EventHandler Close;

		public Window()
		{
			TitlePanel = new Panel()
			{
				Parent = this,
				Color = new Color(100, 100, 100)
			};

			TitleLabel = new Label()
			{
				Parent = this,
				String = "Window",
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
			TitlePanel.Update(deltaTime);
			TitleLabel.Update(deltaTime);
			CloseButton.Update(deltaTime);
			Body.Update(deltaTime);
			ClientArea.Update(deltaTime);
		}

		protected override void OnDraw(RenderTarget target, RenderStates states)
		{
			TitlePanel.Draw(target, states);
			TitleLabel.Draw(target, states);
			CloseButton.Draw(target, states);
			Body.Draw(target, states);
			ClientArea.Draw(target, states);
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

		protected override void OnChildAdded(Control child)
		{
			if (ClientArea is null || child == TitleLabel || child == TitleLabel || child == CloseButton || child == Body || child == ClientArea)
				return;

			ClientArea.AddChild(child);
		}

		protected override void OnChildRemoved(Control child)
		{
			if (child == ClientArea || child == TitlePanel || child == TitleLabel || child == CloseButton || child == Body)
				return;

			ClientArea.RemoveChild(child);
		}

		protected override Vector2i GetInternalSize()
		{
			return ClientArea.Size;
		}
	}
}
