namespace GUIGUI
{
	// INFO: This is a class which implements all the simple drawing primitives which the GUI uses.
	public abstract class Painter
	{
		public abstract void EnableScissor(float X, float Y, float W, float H);
		public abstract void DisableScissor();

		public abstract void SetColor(byte R, byte G, byte B, byte A);
		public abstract void DrawLabel(float X, float Y, float Size, string Txt);
		public abstract void DrawRectangle(float X, float Y, float W, float H);

		// TODO: DrawSprite which takes a position and texture TAG (including one which takes texture filename)
		// That way you can have different skins. For example close button would be SPRITE:CLOSE_BUTTON and you just load a different
		// texture whenever you feel like. As a side effect you get live skin reloading.

		// TODO: Draw9Slice, each slice gets a sprite tag
	}
}
