using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
	}

	class Program
	{
		static void Main(string[] args)
		{
			var renderWindow = new RenderWindow(new VideoMode(1024, 768), "Map renderer");
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

				Input.MouseWheelDeltas = newMouseWheelDeltas;
			};

			Input.Window = renderWindow;
			Input.SubscribeInput(Keyboard.Key.W);
			Input.SubscribeInput(Keyboard.Key.A);
			Input.SubscribeInput(Keyboard.Key.S);
			Input.SubscribeInput(Keyboard.Key.D);

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

			var shape = new RectangleShape(new Vector2f(32, 32))
			{
				Position = (World.Entities.FindAll(e => (e as PlayerSpawnEntity) != null).PickRandom(false) ?? new PlayerSpawnEntity(100, 100)).Position
			};

			var swatch = Stopwatch.StartNew();
			float runTime = 0;
			float delta = 0;

			while (renderWindow.IsOpen)
			{
				delta = swatch.ElapsedMilliseconds / 1000.0f;
				runTime += delta;
				swatch.Restart();

				renderWindow.DispatchEvents();
				Input.Update(delta);

				var sin = Math.Abs(Math.Sin(runTime));
				//shape.FillColor = new Color(0, 128, 255, (byte)(128 * sin));
				shape.FillColor = Color.Blue;

				var newpos = shape.Position;
				if (Input.GetInputState(Keyboard.Key.W).IsDown)
				{
					newpos.Y = shape.Position.Y - 0.1f;
				}
				if (Input.GetInputState(Keyboard.Key.S).IsDown)
				{
					newpos.Y = shape.Position.Y + 0.1f;
				}
				if (Input.GetInputState(Keyboard.Key.A).IsDown)
				{
					newpos.X = shape.Position.X - 0.1f;
				}
				if (Input.GetInputState(Keyboard.Key.D).IsDown)
				{
					newpos.X = shape.Position.X + 0.1f;
				}

				shape.Position = newpos;

				renderWindow.Clear();
				World.Draw(renderWindow);
				renderWindow.Draw(shape);
				renderWindow.Display();
			}
		}
	}
}
