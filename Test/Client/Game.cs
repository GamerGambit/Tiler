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

		public SFML.Graphics.View GameView;

		public Game() : base()
		{
			Window.Title = "Habitat Game Thingo";
			GameView = new SFML.Graphics.View(Window.RenderWindow.GetView());
			/*
			// Test 1
			var dkpink = new Tiler.GUI.Controls.Panel
			{
				Position = new Vector2f(100, 100),
				Size = new Vector2f(300, 300),
				Color = new SFML.Graphics.Color(231, 84, 128)
			};
			dkpink.MouseEnter += (s, e) => Console.WriteLine("Mouse Enter dark-pink");
			dkpink.MouseExit += (s, e) => Console.WriteLine("Mouse Exit dark-pink");
			dkpink.MousePressed += (s, e) => Console.WriteLine($"Mouse {e} pressed in dark-pink");
			dkpink.MouseScrolled += (s, e) => Console.WriteLine($"Mouse scrolled in dark-pink");
			var pnk = new Tiler.GUI.Controls.Panel(dkpink)
			{
				Position = new Vector2f(100, 50),
				Size = new Vector2f(250, 200),
				Color = new SFML.Graphics.Color(255, 192, 203)
			};
			pnk.MouseEnter += (s, e) => Console.WriteLine("Mouse Enter pink");
			pnk.MouseExit += (s, e) => Console.WriteLine("Mouse Exit pink");
			pnk.MousePressed += (s, e) => Console.WriteLine($"Mouse {e} pressed in pink");
			pnk.MouseScrolled += (s, e) => Console.WriteLine($"Mouse scrolled in pink");
			var orng = new Tiler.GUI.Controls.Panel(dkpink)
			{
				Position = new Vector2f(85, 50),
				Size = new Vector2f(100, 100),
				Color = new SFML.Graphics.Color(255, 211, 106)
			};
			orng.MouseEnter += (s, e) => Console.WriteLine("Mouse Enter orange");
			orng.MouseExit += (s, e) => Console.WriteLine("Mouse Exit orange");
			orng.MousePressed += (s, e) => Console.WriteLine($"Mouse {e} pressed in orange");
			orng.MouseScrolled += (s, e) => Console.WriteLine($"Mouse scrolled in orange");
			*/

			var font = new SFML.Graphics.Font("data/saxmono.ttf");

			var panel = new Tiler.GUI.Controls.Panel()
			{
				Position = new Vector2f(100, 100),
				Size = new Vector2i(256, 128),
				Color = new SFML.Graphics.Color(64, 64, 64, 100)
			};
			var button = new Tiler.GUI.Controls.Button()
			{
				Font = font,
				CharacterSize = 12,
				Text = "Fancy Button",
				FillColor = new SFML.Graphics.Color(255, 0, 0, 100),
				OutlineColor = new SFML.Graphics.Color(0, 255, 0, 50),
				OutlineThickness = -0.5f
			};
			button.Click += (s, e) => Console.WriteLine($"Fancy button clicked with {e}");
			button.SizeToContents();
			panel.AddChild(button);
			var text = new Tiler.GUI.Controls.Label()
			{
				Position = new Vector2f(0, button.Position.Y + button.Size.Y + 2),
				Font = font,
				CharacterSize = 140,
				String = "Sample Label Text",
				Size = new Vector2i(100, 25),
				OutlineColor = SFML.Graphics.Color.Blue,
				OutlineThickness = 1,
				FillColor = SFML.Graphics.Color.Cyan
			};
			text.SizeToContents();
			panel.AddChild(text);

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

			GameView.Center = Player.Position;
		}

		public override void OnDraw()
		{
			Window.RenderWindow.SetView(GameView);
			World.Draw(Window);
			Window.Draw(Player);
		}

		public override void OnUpdate(TimeSpan deltaTime)
		{
			var ucmd = new UserCommand();
			var mv = new MoveData();

			Gamemode.CreateUserCommand(ucmd);
			Gamemode.SetupMove(Player, ucmd, mv);
			Gamemode.Move(Player, mv);

			Player.Velocity += mv.Velocity * (float)deltaTime.TotalSeconds;
			Player.Position += Player.Velocity;

			GameView.Move(Player.Velocity);

			Player.Velocity -= Player.Velocity * 0.1f;
		}
	}
}
