using System;
using System.Collections.Generic;

using SFML.System;

using Tiler;

namespace Client
{
	public class Game : Tiler.Program
	{
		public GUIGUI.State GUIState;
		public Gamemode Gamemode;
		public Player Player;

		public Game() : base()
		{
			Window.Title = "Habitat Game Thingo";

			GUIState = new GUIGUI.State(new Painter(Window.RenderWindow));
			GUIState.ParseYAML("data\\gui.yaml");

			// TODO: This is only temp
			GUIGUI.Controls.Panel Panel = new GUIGUI.Controls.Panel(200, 200, 300, 60);
			Panel.AddChild(new GUIGUI.Controls.Label(-50, 5, 40, "Hello World!")
			{
				G = 100,
				B = 100
			});
			GUIState.AddControl(Panel);

			/*
			{
				var im = (InputManager)(InputManager);
				im.Subscribe(Tiler.Input.KeyboardKey.W);
				im.Subscribe(Tiler.Input.KeyboardKey.A);
				im.Subscribe(Tiler.Input.KeyboardKey.S);
				im.Subscribe(Tiler.Input.KeyboardKey.D);
			}
			*/

			World.Map = new Map
			{
				Size = new Vector2i(10, 10),
				TileIDs = new List<int>()
				{
					0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
					0, 2, 2, 2, 2, 2, 2, 2, 2, 0,
					0, 2, 1, 1, 1, 1, 1, 1, 2, 0,
					0, 2, 1, 1, 1, 1, 1, 1, 2, 0,
					0, 2, 1, 1, 1, 1, 1, 1, 2, 0,
					0, 2, 1, 1, 1, 1, 1, 1, 2, 0,
					0, 2, 1, 1, 1, 1, 1, 1, 2, 0,
					0, 2, 1, 1, 1, 1, 1, 1, 2, 0,
					0, 2, 2, 2, 2, 2, 2, 2, 2, 0,
					0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				}
			};
			World.Entities.Add(new PlayerSpawn(192, 192));
			World.Map.Rebuild();

			Gamemode = new TestGamemode();
			Player = new Player();

			Gamemode.CreateTeams();
			Gamemode.PlayerInitialSpawn(Player);
			var spawnPoint = Gamemode.PlayerSelectSpawn(Player);

			if (spawnPoint is null)
				throw new Exception("No spawnpoint");

			Player.Position = spawnPoint.Position;
			Gamemode.PlayerSpawn(Player);

			{
				var view = Window.RenderWindow.GetView();
				view.Center = Player.Position;
				Window.RenderWindow.SetView(view);
			}
		}

		public override void OnDraw()
		{
			World.Draw(Window);
			Window.Draw(Player);
			GUIState.Draw();
		}

		public override void OnUpdate(float deltaTime)
		{
			var ucmd = new UserCommand();
			var mv = new MoveData();

			Gamemode.CreateUserCommand(ucmd);
			Gamemode.SetupMove(Player, ucmd, mv);
			Gamemode.Move(Player, mv);

			Player.Velocity += mv.Velocity * deltaTime;
			Player.Position += Player.Velocity;

			{
				var view = Window.RenderWindow.GetView();
				view.Move(Player.Velocity);
				Window.RenderWindow.SetView(view);
			}

			Player.Velocity -= Player.Velocity * 0.1f;
		}
	}
}
