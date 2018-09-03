using System;

namespace Tiler.GUI
{
	public class Animation
	{
		internal enum State
		{
			NotStarted,
			Started,
			Ended
		}
		internal State state = State.NotStarted;

		private float startTime = 0;
		private float elapsedTime { get => (float)Program.ElapsedTime.TotalSeconds - startTime; }

		public float Duration { get; set; }
		public float Delay { get; set; } = 0;
		public float Progress { get; private set; } = 0;
		public Control Control { get; private set; }

		public event EventHandler Finish;

		internal Animation(Control control)
		{
			Control = control;
		}

		public void Animate()
		{
			startTime = (float)Program.ElapsedTime.TotalSeconds + Delay;
		}

		internal void Update(TimeSpan deltaTime)
		{
			if (state == State.NotStarted && Program.ElapsedTime.TotalSeconds >= startTime)
			{
				state = State.Started;
				OnStart();
			}

			if (state != State.Started)
				return;

			Progress = Math.Min(1, elapsedTime / Duration);
			//Console.WriteLine($"{Length} | {elapsedTime} | {Math.Round(Progress * 100, 2)}");
			OnUpdate(deltaTime);

			if (elapsedTime >= Duration)
			{
				state = State.Ended;
				OnFinish();
				Finish?.Invoke(this, EventArgs.Empty);

				return;
			}
		}

		protected virtual void OnStart()
		{
			// NOP
		}

		protected virtual void OnUpdate(TimeSpan deltaTime)
		{
			// NOP
		}

		protected virtual void OnFinish()
		{
			// NOP
		}
	}
}
