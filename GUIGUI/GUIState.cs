using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GUIGUI {
	// INFO: This is a class which implements all the simple drawing primitives which the GUI uses.
	public abstract class GUIPainter {
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

	public class GUIState {
		GUIPainter Painter;
		List<GUIControl> Controls = new List<GUIControl>(); // First item should be topmost

		public GUIState(GUIPainter Painter) {
			this.Painter = Painter;
		}

		public void AddControl(GUIControl C) {
			Controls.Add(C);
		}

		// TODO: Functions like ClickMouse(X, Y) which calculate the child under the click position
		// and call an apropriate event on said child
		// SendInput(Key) and SendUnicode() and similar which operate on currently active control
		// MoveMouse, Scroll...
		// Clicks should scan recursively and send input to deepest child.

		// TODO: Z-ordering which just moves clicked item to the start of the Controls list
		// Proper drawing will be handled automatically, input too (as you iterate the list from start to end)

		public void ParseYAML(string YAMLSrc) {
			// TODO: A nice YAML thing where you can define complex windows like Settings and Main Menu
			// and inventory and shit. Each control would have a tag.
			// For example a button could have a NewGame tag on it, when you press that button
			// a tag event will be called which will invoke a custom function.

			// You would effectively define operations like New Game and Quit as tag actions and YAML would handle the GUI layout.
			// That shit is least priority, you should be able to create custom layouts by hand for now.
		}

		void PushScissor(float X, float Y, float W, float H) {
			// TODO: Push and set scissor, can only decrease size of parent scissor, not increase.
		}

		void PopScissor() {

		}

		// TODO: Proper implementation. Each child should draw at an offset from the parent position.
		// So for a panel at 100, 100 and a label at 5, 5 the label should draw at 105, 105 global
		// This should draw children from back to front.
		void DrawSelfThenChildren(GUIControl C, GUIPainter P) {
			PushScissor(C.Position.X, C.Position.Y, C.Size.X, C.Size.Y);
			C.Draw(P);

			// TODO: Bounds checking
			foreach (var Child in C.Children)
				DrawSelfThenChildren(Child, P);

			PopScissor();
		}

		public void Draw() {
			foreach (var C in Controls) // TODO: Back to front
				DrawSelfThenChildren(C, Painter);
		}
	}

	public class GUIControl {
		public List<GUIControl> Children = new List<GUIControl>();

		public Vector2 Position;
		public Vector2 Size;
		public byte R, G, B, A;

		public virtual void Draw(GUIPainter P) {
			throw new InvalidOperationException();
		}
	}

	public class GUIPanel : GUIControl {
		public GUIPanel(float X, float Y, float W, float H) {
			Position = new Vector2(X, Y);
			Size = new Vector2(W, H);
			R = G = B = 50;
			A = 255;
		}

		public override void Draw(GUIPainter P) {
			P.SetColor(R, G, B, A);
			P.DrawRectangle(Position.X, Position.Y, Size.X, Size.Y);
		}
	}

	public class GUILabel : GUIControl {
		public string Text;
		public float TextSize;

		public GUILabel(float X, float Y, float Size, string Txt) {
			Position = new Vector2(X, Y);
			TextSize = Size;
			Text = Txt;
			R = G = B = A = 255;
		}

		public override void Draw(GUIPainter P) {
			P.SetColor(R, G, B, A);
			P.DrawLabel(Position.X, Position.Y, TextSize, Text);
		}
	}
}
