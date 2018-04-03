using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace Tiler {
	public static class Utils {
		public static float Clamp(float Value, float Min, float Max) {
			return Math.Max(Math.Min(Value, Max), Min);
		}
	}

	public static class UtilsDrawing {
		const string LibName = "opengl32";
		public const int GL_SCISSOR_TEST = 0xC11;

		[DllImport(LibName)]
		static extern void glEnable(int Cap);

		[DllImport(LibName)]
		static extern void glDisable(int Cap);

		[DllImport(LibName)]
		static extern void glScissor(int X, int Y, int W, int H);

		static void glScissor2(int WindH, int X, int Y, int W, int H) {
			glScissor(X, WindH - Y - H, W, H);
		}

		public static void EnableScissor(bool Enable) {
			if (Enable)
				glEnable(GL_SCISSOR_TEST);
			else
				glDisable(GL_SCISSOR_TEST);
		}

		public static void SetScissor(RenderTarget RT, int X, int Y, int W, int H) {
			View V = RT.GetView();
			glScissor2((int)V.Size.Y, X, Y, W, H);
		}
	}
}