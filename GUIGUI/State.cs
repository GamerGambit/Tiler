using System.Collections.Generic;

namespace GUIGUI
{
	public class State {
		Painter Painter;
		List<Control> Controls = new List<Control>(); // First item should be topmost

		public State(Painter Painter) {
			this.Painter = Painter;
		}

		public void AddControl(Control C) {
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
			Painter.EnableScissor(X, Y, W, H);
		}

		void PopScissor() {
			Painter.DisableScissor();
		}

		// TODO: Proper implementation. Each child should draw at an offset from the parent position.
		// So for a panel at 100, 100 and a label at 5, 5 the label should draw at 105, 105 global
		// This should draw children from back to front.
		void DrawSelfThenChildren(Control C, Painter P) {
			C.Draw(P);
			PushScissor(C.Position.X, C.Position.Y, C.Size.X, C.Size.Y);

			// TODO: Bounds checking
			for (int index = 0; index < C.Children.Count; ++index)
			{
				DrawSelfThenChildren(C.Children[index], P);
			}

			PopScissor();
		}

		public void Draw()
		{
			for (int index = 0; index < Controls.Count; ++index)
			{
				DrawSelfThenChildren(Controls[index], Painter);
			}
		}
	}
}
