using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI.Controls
{
	public class Label : Control
	{
		private Text text = new Text();

		public Font Font { get => text.Font; set => text.Font = value; }
		public uint CharacterSize { get => text.CharacterSize; set => text.CharacterSize = value; }
		public string String { get => text.DisplayedString; set => text.DisplayedString = value; }

		public Label(Control parent = null) : base(parent)
		{
			// NOP
		}

		public void SizeToContents()
		{
			var gbounds = text.GetGlobalBounds();
			Size = new Vector2f(gbounds.Left + gbounds.Width, gbounds.Top + gbounds.Height);
		}

		public override void OnDraw(RenderTarget target, RenderStates states)
		{
			target.Draw(text, states);
		}
	}
}
