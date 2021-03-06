﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Tiler.CrossPlatform
{
	public static class Platform
	{
		const string WinLib = "kernel32";
		const CallingConvention WinCConv = CallingConvention.Winapi;

		static PlatformID CurrentPlatform = Environment.OSVersion.Platform;

		[DllImport(WinLib, CallingConvention = WinCConv)]
		static extern bool SetDllDirectory(string Dir);

		public static bool SetLibraryDir(string Dir)
		{
			string Path = Environment.GetEnvironmentVariable("path");

			if (!Path.Contains(Dir))
				Path = Path + ";" + Dir;

			Environment.SetEnvironmentVariable("path", Path);

			if (CurrentPlatform == PlatformID.Win32NT)
				return SetDllDirectory(Dir);

			throw new Exception("Not implemented for platform " + CurrentPlatform);
		}

		public static string GetExeDir()
		{
			return Path.GetFullPath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
		}

		public static string GetX86()
		{
			return Path.Combine(GetExeDir(), "native", "x86");
		}

		public static string GetX64()
		{
			return Path.Combine(GetExeDir(), "native", "x64");
		}
	}
}
