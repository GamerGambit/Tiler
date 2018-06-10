﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using SFML.Graphics;

namespace Tiler
{
	public static class Utils {
		private static Random rng = new Random();

		public static float Clamp(float Value, float Min, float Max) {
			return Math.Max(Math.Min(Value, Max), Min);
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
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