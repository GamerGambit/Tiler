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
		public Color FillColor { get => text.FillColor; set => text.FillColor = value; }
		public Color OutlineColor { get => text.OutlineColor; set => text.OutlineColor = value; }
		public float OutlineThickness { get => text.OutlineThickness; set => text.OutlineThickness = value; }
		public Text.Styles Styles { get => text.Style; set => text.Style = value; }

		public Label()
		{
			HandlesKeyboardInput = false;
			HandlesMouseInput = false;
		}

		public void SizeToContents()
		{
			var gbounds = text.GetGlobalBounds();
			Size = new Vector2i((int)(gbounds.Left + gbounds.Width), (int)(gbounds.Top + gbounds.Height));
		}

		protected override void OnDraw(RenderTarget target, RenderStates states)
		{
			target.Draw(text, states);
		}
	}
}
