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
				Size = new Vector2f(256, 128),
				Color = new SFML.Graphics.Color(64, 64, 64, 100)
			};
			var text = new Tiler.GUI.Controls.Label(panel)
			{
				Font = font,
				CharacterSize = 14,
				String = "Sample Label Text",
				Size = new Vector2f(100, 25)
			};
			text.MouseScrolled += (s, e) =>
			{
				var deltas = Tiler.Input.Manager.MouseWheelDeltas;
				if (deltas.Y < 0)
				{
					if (text.CharacterSize > 1)
					{
						text.CharacterSize--;
						text.SizeToContents();
					}
				}
				else if (deltas.Y > 0)
				{
					text.CharacterSize++;
					text.SizeToContents();
				}
			};

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

		public override void OnUpdate(TimeSpan deltaTime)
		{
			var ucmd = new UserCommand();
			var mv = new MoveData();

			Gamemode.CreateUserCommand(ucmd);
			Gamemode.SetupMove(Player, ucmd, mv);
			Gamemode.Move(Player, mv);

			Player.Velocity += mv.Velocity * (float)deltaTime.TotalSeconds;
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
