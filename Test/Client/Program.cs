using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using GUIGUI;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

using Tiler;

namespace Client
{
	static class Random
	{
		public static System.Random Rnd = new System.Random();

		public static T PickRandom<T>(this IList<T> list, bool ThrowIfEmpty = true)
		{
			if (list.Count == 0) {
				if (ThrowIfEmpty)
				throw new Exception("Can not pick random item from empty list");

				return default(T);
			}

			var index = Rnd.Next(list.Count);
			return list[index];
		}

		public static byte RandomByte()
		{
			return (byte)Rnd.Next(256);
		}

		public static Color RandomColor(byte Alpha = 255)
		{
			return new Color(RandomByte(), RandomByte(), RandomByte(), Alpha);
		}
	}

	class Program
	{
		static RenderWindow renderWindow;

		static void Main(string[] args)
		{
			renderWindow = new RenderWindow(new VideoMode(1024, 768), "Map renderer");
			renderWindow.Closed += (s, e) => (s as RenderWindow).Close();
			renderWindow.MouseWheelScrolled += (s, e) =>
			{
				var newMouseWheelDeltas = new Vector2f(0, 0);

				if (e.Wheel == Mouse.Wheel.HorizontalWheel)
				{
					newMouseWheelDeltas.X = e.Delta;
				}
				else
				{
					newMouseWheelDeltas.Y = e.Delta;
				}

				Tiler.Input.Manager.MouseWheelDeltas = newMouseWheelDeltas;
			};

			var GUI = new State(new Painter(renderWindow));
			GUI.ParseYAML("data\\gui.yaml");

			// TODO: This is only temp
			GUIGUI.Controls.Panel Panel = new GUIGUI.Controls.Panel(200, 200, 300, 60);
			Panel.Children.Add(new GUIGUI.Controls.Label(205, 205, 40, "Hello World!"));
			GUI.AddControl(Panel);

			Tiler.Input.Manager.Window = renderWindow;
			Tiler.Input.Manager.SubscribeInput(Keyboard.Key.W);
			Tiler.Input.Manager.SubscribeInput(Keyboard.Key.A);
			Tiler.Input.Manager.SubscribeInput(Keyboard.Key.S);
			Tiler.Input.Manager.SubscribeInput(Keyboard.Key.D);

			if (File.Exists("testmap1.tmx")) {
				World.LoadChunk("testmap1.tmx", new Vector2i(0, 0));
				World.LoadChunk("testmap1.tmx", new Vector2i(320, 0));
				World.LoadChunk("testmap1.tmx", new Vector2i(640, 0));
			}

			if (File.Exists("testmap2.tmx")) {
				World.LoadChunk("testmap2.tmx", new Vector2i(0, 320));
				World.LoadChunk("testmap2.tmx", new Vector2i(320, 320));
				World.LoadChunk("testmap2.tmx", new Vector2i(640, 320));
			}

			var gamemode = new TestGamemode();
			var player = new Player();

			gamemode.CreateTeams();
			gamemode.PlayerInitialSpawn(player);
			var spawnPoint = gamemode.PlayerSelectSpawn(player);
			if (spawnPoint is null)
				throw new Exception("No spawnpoint");
			player.Position = spawnPoint.Position;
			gamemode.PlayerSpawn(player);

			var swatch = Stopwatch.StartNew();
			var runtimeWatch = Stopwatch.StartNew();

			float runTime = 0;
			float delta = 0;
			
			while (renderWindow.IsOpen)
			{
				// INFO: 120 FPS cap because shit bugs out at huge framerates (delta approaches 0)
				while (swatch.ElapsedMilliseconds / 1000.0f < (1.0f / 120.0f))
					;
				delta = swatch.ElapsedMilliseconds / 1000.0f;
				swatch.Restart();

				runTime = runtimeWatch.ElapsedMilliseconds / 1000.0f;

				renderWindow.DispatchEvents();
				Tiler.Input.Manager.Update(delta);

				var ucmd = new UserCommand();
				var mv = new MoveData();

				gamemode.CreateUserCommand(ucmd);
				gamemode.SetupMove(player, ucmd, mv);
				gamemode.Move(player, mv);

				player.Velocity += mv.Velocity * delta;
				player.Position += player.Velocity;
				player.Velocity -= player.Velocity * 0.1f;
				
				renderWindow.Clear();
				{
					World.Draw(renderWindow);
					renderWindow.Draw(player);

					GUI.Draw();
				}
				renderWindow.Display();
			}
		}
	}
}
