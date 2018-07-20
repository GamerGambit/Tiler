using System;
using System.Collections.Generic;

using SFML.System;
using Tiler;

namespace Client
{
	public class Game : Tiler.Program
	{
		public Gamemode Gamemode;
		public Player Player;

		public Game() : base()
		{
			Window.Title = "Habitat Game Thingo";

			/*
			{
				var im = (InputManager)(InputManager);
				im.Subscribe(Tiler.Input.KeyboardKey.W);
				im.Subscribe(Tiler.Input.KeyboardKey.A);
				im.Subscribe(Tiler.Input.KeyboardKey.S);
				im.Subscribe(Tiler.Input.KeyboardKey.D);
			}
			*/

			/*
			// Test 1
			var dkpink = new Tiler.GUI.Controls.Panel
			{
				Position = new Vector2f(100, 100),
				Size = new System.Numerics.Vector2(300, 300),
				Color = new SFML.Graphics.Color(231, 84, 128)
			};
			var pnk = new Tiler.GUI.Controls.Panel(dkpink)
			{
				Position = new Vector2f(100, 50),
				Size = new System.Numerics.Vector2(250, 200),
				Color = new SFML.Graphics.Color(255, 192, 203)
			};
			var orng = new Tiler.GUI.Controls.Panel(pnk)
			{
				Position = new Vector2f(100, 50),
				Size = new System.Numerics.Vector2(100, 100),
				Color = new SFML.Graphics.Color(255, 211, 106)
			};

			// Test 2
			var red = new Tiler.GUI.Controls.Panel
			{
				Position = new Vector2f(100, 100),
				Size = new Vector2f(500, 300),
				Color = SFML.Graphics.Color.Red
			};
			var green = new Tiler.GUI.Controls.Panel(red)
			{
				Position = new Vector2f(100, 100),
				Size = new Vector2f(300, 400),
				Color = SFML.Graphics.Color.Green
			};
			var blue = new Tiler.GUI.Controls.Panel(green)
			{
				Position = new Vector2f(150, -50),
				Size = new Vector2f(300, 200),
				Color = SFML.Graphics.Color.Blue
			};
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
