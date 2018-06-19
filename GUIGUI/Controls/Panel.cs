using System.Numerics;

namespace GUIGUI.Controls
{
	public class Panel : Control
	{
		public Panel(float X, float Y, float W, float H)
		{
			Position = new Vector2(X, Y);
			Size = new Vector2(W, H);
			R = G = B = 50;
			A = 255;
		}

		public override void Draw(Painter P)
		{
			P.SetColor(R, G, B, A);
			var gpos = GlobalPosition;
			P.DrawRectangle(gpos.X, gpos.Y, Size.X, Size.Y);
		}
	}
}
