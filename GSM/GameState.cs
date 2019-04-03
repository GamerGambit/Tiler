using System;

using SFML.Graphics;

namespace Tiler.GSM
{
	public abstract class GameState : Drawable, IUpdatable
	{
		public GameStateStack GameStateStack { get; private set; }
		public bool UpdateBelow { get; protected set; }
		public bool RenderBelow { get; protected set; }
		public bool IsTopMostState { get => GameStateStack.TopMostGameState == this; }

		public GameState(GameStateStack gameStateStack)
		{
			GameStateStack = gameStateStack;
		}

		public virtual void OnPush()
		{
			// NOP
		}

		public virtual void OnPop()
		{

		}

		public virtual void OnSuspend()
		{

		}

		public virtual void OnContinue()
		{

		}

		public virtual void OnConceal()
		{

		}

		public virtual void OnReveal()
		{

		}

		public abstract void Update(TimeSpan deltaTime);
		public abstract void Draw(RenderTarget target, RenderStates states);
	}
}
