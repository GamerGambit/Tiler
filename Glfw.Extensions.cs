using System;
using System.Runtime.InteropServices;

namespace Glfw3
{
	public static partial class Glfw
	{
		[DllImport("glfw3", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr glfwGetWin32Window(IntPtr Handle);
	}
}
