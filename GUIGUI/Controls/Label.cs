using System.Numerics;

namespace GUIGUI.Controls
{
	public class Label : Control
	{
		public string Text;
		public float TextSize;

		public Label(float X, float Y, float Size, string Txt)
		{
			Position = new Vector2(X, Y);
			TextSize = Size;
			Text = Txt;
			R = G = B = A = 255;
		}

		public override void Draw(Painter P)
		{
			P.SetColor(R, G, B, A);
			P.DrawLabel(Position.X, Position.Y, TextSize, Text);
		}
	}
}
