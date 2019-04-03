using System;

using Tiler;

namespace Client
{
	public class Game : Tiler.Program
	{
		private readonly Tiler.GSM.GameStateStack GameStateStack = new Tiler.GSM.GameStateStack();

		public Game() : base()
		{
			Window.Title = "Habitat Game Thingo";

			GameStateContext.Window = Window;
			GameStateContext.MainMenu = new states.MainMenu(GameStateStack);
			GameStateContext.MainGame = new states.MainGame(GameStateStack);

			GameStateStack.RequestPushGameState(GameStateContext.MainGame);
		}

		public override void OnDraw()
		{
			Window.Draw(GameStateStack);
		}

		public override void OnUpdate(TimeSpan deltaTime)
		{
			if (GameStateStack.ReadyForShutdown)
			{
				Window.Close();
				return;
			}

			if (Tiler.Input.Manager.GetState(Glfw3.Glfw.KeyCode.Escape).WasPressed)
			{
				GameStateStack.RequestPopGameState();
			}
			else if (Tiler.Input.Manager.GetState(Glfw3.Glfw.KeyCode.F1).WasPressed && !GameStateContext.MainMenu.IsTopMostState)
			{
				GameStateStack.RequestSwitchGameState(GameStateContext.MainMenu);
			}
			else if (Tiler.Input.Manager.GetState(Glfw3.Glfw.KeyCode.F2).WasPressed && !GameStateContext.MainGame.IsTopMostState)
			{
				GameStateStack.RequestSwitchGameState(GameStateContext.MainGame);
			}

			GameStateStack.Update(deltaTime);
		}
	}
}
