using System;
using System.Collections.Generic;

using SFML.Graphics;

namespace Tiler.GSM
{
	public class GameStateStack : Drawable, IUpdatable
	{
		protected struct PendingAction
		{
			public enum ActionType
			{
				Push,
				Pop,
				Switch
			}

			public ActionType Type;
			public GameState GameState;
		}

		protected Queue<PendingAction> PendingActions = new Queue<PendingAction>();
		protected List<GameState> GameStates = new List<GameState>();

		protected void ApplyPendingActions()
		{
			foreach (var action in PendingActions)
			{
				switch (action.Type)
				{
				case PendingAction.ActionType.Push:
					PushGameState(action.GameState);
					break;

				case PendingAction.ActionType.Pop:
					PopGameState();
					break;

				case PendingAction.ActionType.Switch:
					SwitchGameState(action.GameState);
					break;
				}
			}
		}

		protected void PushGameState(GameState gameState)
		{
			var currentState = TopMostGameState;

			if (!(currentState is null))
			{
				if (!gameState.UpdateBelow)
				{
					currentState.OnSuspend();
				}

				if (!gameState.RenderBelow)
				{
					currentState.OnConceal();
				}
			}

			GameStates.Add(gameState);
			gameState.OnPush();
			gameState.OnContinue();
		}

		protected void PopGameState()
		{
			if (ReadyForShutdown)
				throw new InvalidOperationException("GameStateStack is empty");

			var currentState = TopMostGameState;

			currentState.OnSuspend();
			GameStates.RemoveAt(GameStates.Count - 1);
			currentState.OnPop();

			if (!ReadyForShutdown)
			{
				var newTopState = TopMostGameState;

				if (!currentState.UpdateBelow)
				{
					newTopState.OnContinue();
				}

				if (!currentState.RenderBelow)
				{
					newTopState.OnReveal();
				}
			}
		}

		protected void SwitchGameState(GameState gameState)
		{
			PopGameState();

			GameStates.Add(gameState);
			gameState.OnPush();
			gameState.OnContinue();
		}

		protected void UpdateGameState(int index, TimeSpan deltaTime)
		{
			if (index >= GameStates.Count)
				return;

			var state = GameStates[index];

			if (state.UpdateBelow)
			{
				UpdateGameState(index - 1, deltaTime);
			}

			state.Update(deltaTime);
		}

		protected void DrawGameState(int index, RenderTarget target, RenderStates states)
		{
			if (index >= GameStates.Count)
				return;

			var state = GameStates[index];

			if (state.RenderBelow)
			{
				DrawGameState(index - 1, target, states);
			}

			target.Draw(state, states);
		}

		public GameState TopMostGameState { get => GameStates.Count == 0 ? null : GameStates[GameStates.Count - 1]; }
		public bool ReadyForShutdown { get; private set; } = false;

		public void RequestPushGameState(GameState gameState)
		{
			PendingActions.Enqueue(new PendingAction()
			{
				Type = PendingAction.ActionType.Push,
				GameState = gameState
			});
		}

		public void RequestPopGameState()
		{
			PendingActions.Enqueue(new PendingAction()
			{
				Type = PendingAction.ActionType.Pop
			});
		}

		public void RequestSwitchGameState(GameState gameState)
		{
			PendingActions.Enqueue(new PendingAction()
			{
				Type = PendingAction.ActionType.Switch,
				GameState = gameState
			});
		}

		public void Update(TimeSpan deltaTime)
		{
			ApplyPendingActions();

			if (GameStates.Count == 0)
			{
				ReadyForShutdown = true;
				return;
			}

			UpdateGameState(GameStates.Count - 1, deltaTime);
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			if (ReadyForShutdown)
				return;

			DrawGameState(GameStates.Count - 1, target, states);
		}
	}
}
