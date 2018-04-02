using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIGUI {
	public delegate void GUIDrawingFunc();

	public class GUIState {
		public GUIDrawingFunc OnDraw;

		public void Update() {

		}

		public void Draw() {

		}
	}
}
