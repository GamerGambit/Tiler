using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	public static class Utils
	{
		private static Random rng = new Random();

		public static float Clamp(float Value, float Min, float Max)
		{
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

		public static byte RandomByte()
		{
			return (byte)rng.Next(0, 256);
		}

		public static T Pin<T>(T input)
		{
			GCHandle.Alloc(input, GCHandleType.Normal);
			return input;
		}

		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			if (value.CompareTo(min) < 0)
				return min;

			if (value.CompareTo(max) > 0)
				return max;

			return value;
		}

		public static float Lerp(float start, float end, float fraction)
		{
			return start * (1.0f - fraction) + end * fraction;
		}

		public static Vector2 LerpVector(Vector2 start, Vector2 end, float fraction)
		{
			return new Vector2(
				Lerp(start.X, end.X, fraction),
				Lerp(start.Y, end.Y, fraction)
			);
		}

		public static Vector2i LerpVector(Vector2i start, Vector2i end, float fraction)
		{
			return new Vector2i(
				(int)Lerp(start.X, end.X, fraction),
				(int)Lerp(start.Y, end.Y, fraction)
			);
		}
	}

	public static class UtilsDrawing
	{
		const string LibName = "opengl32";
		public const int GL_SCISSOR_TEST = 0xC11;

		[DllImport(LibName)]
		static extern void glEnable(int Cap);

		[DllImport(LibName)]
		static extern void glDisable(int Cap);

		[DllImport(LibName)]
		static extern void glScissor(int X, int Y, int W, int H);

		static void glScissor2(int WindH, int X, int Y, int W, int H)
		{
			glScissor(X, WindH - Y - H, W, H);
		}

		public static void EnableScissor(bool Enable)
		{
			if (Enable)
				glEnable(GL_SCISSOR_TEST);
			else
				glDisable(GL_SCISSOR_TEST);
		}

		public static void SetScissor(RenderTarget RT, int X, int Y, int W, int H)
		{
			View V = RT.GetView();
			var offset = V.Center - V.Size / 2;
			glScissor2((int)V.Size.Y, (int)(X - offset.X), (int)(Y - offset.Y), W, H);
		}
	}
}