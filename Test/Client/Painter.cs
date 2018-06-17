using SFML.Graphics;
using SFML.System;

using Tiler;

namespace Client
{
	class Painter : GUIGUI.Painter
	{
		RenderWindow RWind;
		Color Clr;

		public Painter(RenderWindow RWind)
		{
			this.RWind = RWind;
		}

		public override void EnableScissor(float X, float Y, float W, float H)
		{
			UtilsDrawing.EnableScissor(true);
			UtilsDrawing.SetScissor(RWind, (int)X, (int)Y, (int)W, (int)H);
		}

		public override void DisableScissor()
		{
			UtilsDrawing.EnableScissor(false);
		}

		public override void SetColor(byte R, byte G, byte B, byte A)
		{
			Clr = new Color(R, G, B, A);
		}

		Text SFMLTxt;
		public override void DrawLabel(float X, float Y, float Size, string Txt)
		{
			if (SFMLTxt == null)
				SFMLTxt = new Text("", new Font("data\\saxmono.ttf"));

			SFMLTxt.DisplayedString = Txt;
			SFMLTxt.CharacterSize = (uint)Size;
			SFMLTxt.Position = new Vector2f(X, Y);
			SFMLTxt.FillColor = Clr;
			RWind.Draw(SFMLTxt);
		}

		RectangleShape SFMLRect;
		public override void DrawRectangle(float X, float Y, float W, float H)
		{
			if (SFMLRect == null)
				SFMLRect = new RectangleShape();

			SFMLRect.Position = new Vector2f(X, Y);
			SFMLRect.Size = new Vector2f(W, H);
			SFMLRect.FillColor = Clr;
			RWind.Draw(SFMLRect);
		}
	}
}
