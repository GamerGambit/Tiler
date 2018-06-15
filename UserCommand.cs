using System;

namespace Tiler
{
	[Flags]
	public enum InKeys
	{
		None = 0,
		MoveForward = 1,
		MoveBackward = 1 << 1,
		MoveLeft = 1 << 2,
		MoveRight = 1 << 3,
		Jump = 1 << 4
	}

	public class UserCommand
	{
		public InKeys Keys { get; set; }
	}
}
