using System;
using System.Collections.Generic;
using System.Text;

using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI.Controls
{
	public class Label : Control
	{
		public enum WrapTypes
		{
			None,
			Normal,
			BreakWords
		}

		private List<Text> texts = new  List<Text>(){ new Text() };
		private string str = "";
		private WrapTypes wrapType = WrapTypes.None;

		private void CalculateWrap()
		{
			if (str.Length == 0 || wrapType == WrapTypes.None || Size.X == 0 || Font is null)
			{
				texts[0].DisplayedString = str;
				return;
			}

			float spaceWidth = Font.GetGlyph(' ', CharacterSize, Styles.HasFlag(SFML.Graphics.Text.Styles.Bold), texts[0].OutlineThickness).Advance;

			var sb = new StringBuilder(str);

			int lastWSPos = 0;
			char lastWS = '\0';
			float curWidth = 0;
			float widthSinceLastWS = 0;
			char lastChar = '\0';
			for (var index = 0; index < str.Length; ++index)
			{
				var ch = str[index];

				float glyphWidth = 0;

				if (ch == ' ')
				{
					glyphWidth = spaceWidth;
					lastWSPos = index;
					lastWS = ch;
					widthSinceLastWS = 0;
				}
				else if (ch == '\t')
				{
					glyphWidth = spaceWidth * TabWrapWidth;
				}
				else if (ch == '\n')
				{
					curWidth = 0;
				}
				else
				{
					var glyph = Font.GetGlyph(ch, CharacterSize, Styles.HasFlag(SFML.Graphics.Text.Styles.Bold), texts[0].OutlineThickness);
					glyphWidth += glyph.Advance + Font.GetKerning(lastChar, ch, CharacterSize);
				}

				if (curWidth + glyphWidth >= Size.X)
				{
					curWidth = widthSinceLastWS - glyphWidth;

					if (wrapType == WrapTypes.BreakWords)
					{
						sb.Insert(index - 1, '\n');
					}
					else
					{
						sb.Replace(lastWS, '\n', lastWSPos, 1);
					}
				}
				else
				{
					curWidth += glyphWidth;
					widthSinceLastWS += glyphWidth;
				}
			}

			var split = sb.ToString().Split('\n');
			var newTexts = new List<Text>(split.Length);
			float yOffset = 0;

			foreach (var line in split)
			{
				var text = new Text(line, Font, CharacterSize)
				{
					FillColor = FillColor,
					OutlineColor = OutlineColor,
					OutlineThickness = OutlineThickness,
					Position = new Vector2f(0, yOffset)
				};

				yOffset += Font.GetLineSpacing(CharacterSize);

				newTexts.Add(text);
			}

			texts = newTexts;
		}

		public Font Font { get => texts[0].Font; set => texts.ForEach(t => t.Font = value); }
		public uint CharacterSize { get => texts[0].CharacterSize; set => texts.ForEach(t => t.CharacterSize = value); }
		public string Text { get => str; set { str = value; CalculateWrap(); } }
		public Color FillColor { get => texts[0].FillColor; set => texts.ForEach(t => t.FillColor = value); }
		public Color OutlineColor { get => texts[0].OutlineColor; set => texts.ForEach(t => t.OutlineColor = value); }
		public float OutlineThickness { get => texts[0].OutlineThickness; set => texts.ForEach(t => t.OutlineThickness = value); }
		public Text.Styles Styles { get => texts[0].Style; set => texts.ForEach(t => t.Style = value); }
		public WrapTypes WrapType { get => wrapType; set { wrapType = value; CalculateWrap(); } }
		public uint TabWrapWidth { get; set; } = 4;

		public void SizeToContents(bool resizeX = true, bool resizeY = true)
		{
			if (Font is null)
				return;

			CalculateWrap();

			float x = 0;
			float y = 0;

			foreach (var text in texts)
			{
				var gb = text.GetLocalBounds();
				x = Math.Max(x, Position.X + gb.Left + gb.Width);
				y = text.Position.Y + gb.Top + gb.Height;
			}

			Size = new Vector2i(resizeX ? (int)Math.Ceiling(x) : Size.X, resizeY ? (int)Math.Ceiling(y) : Size.Y);
		}

		protected override void OnDraw(RenderTarget target, RenderStates states)
		{
			foreach (var text in texts)
			{
				target.Draw(text, states);
			}
		}
	}
}
