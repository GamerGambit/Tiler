using System;
using SFML.System;

namespace Tiler.GUI.Animations
{
	public class Move : Animation
	{
		private Vector2i startPosition;

		public Vector2i EndPosition;

		public Move(Control control) : base(control)
		{
			// NOP
		}

		protected override void OnStart()
		{
			startPosition = Control.Position;
		}

		protected override void OnUpdate(TimeSpan deltaTime)
		{
			Control.Position = Utils.LerpVector(startPosition, EndPosition, Progress);
		}

		protected override void OnFinish()
		{
			Control.Position = EndPosition;
		}
	}
}
