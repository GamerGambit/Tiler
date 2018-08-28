using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace Tiler.CrossPlatform {
	public static class Platform {
		const string WinLib = "kernel32";
		const CallingConvention WinCConv = CallingConvention.Winapi;

		static PlatformID CurrentPlatform = Environment.OSVersion.Platform;

		[DllImport(WinLib, CallingConvention = WinCConv)]
		static extern bool SetDllDirectory(string Dir);

		public static bool SetLibraryDir(string Dir) {
			string NewPath = Environment.GetEnvironmentVariable("path") + ";" + Dir;
			Environment.SetEnvironmentVariable("path", NewPath);

			if (CurrentPlatform == PlatformID.Win32NT)
				return SetDllDirectory(Dir);

			throw new Exception("Not implemented for platform " + CurrentPlatform);
		}

		public static string GetExeDir() {
			return Path.GetFullPath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
		}

		public static string GetX86() {
			return Path.Combine(GetExeDir(), "native", "x86");
		}

		public static string GetX64() {
			return Path.Combine(GetExeDir(), "native", "x64");
		}
	}
}
