using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Tiler;
using GUIGUI;

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

				Input.MouseWheelDeltas = newMouseWheelDeltas;
			};

			var GUI = new GUIState(new Painter(renderWindow));
			GUI.ParseYAML("data\\gui.yaml");

			// TODO: This is only temp
			GUIPanel Panel = new GUIPanel(200, 200, 300, 60);
			Panel.Children.Add(new GUILabel(205, 205, 40, "Hello World!"));
			GUI.AddControl(Panel);

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
				Input.Update(delta);

				var sin = Utils.Clamp(((float)Math.Sin(runTime * 5) / 2) + 0.5f, 0.2f, 1.0f);
				shape.FillColor = new Color(0, 128, 255, (byte)(sin * 255));
				//shape.FillColor = Color.Blue;

				var movespeed = 100.0f * delta;
				var newpos = shape.Position;
				if (Input.GetInputState(Keyboard.Key.W).IsDown)
				{
					newpos.Y = shape.Position.Y - movespeed;
				}
				if (Input.GetInputState(Keyboard.Key.S).IsDown)
				{
					newpos.Y = shape.Position.Y + movespeed;
				}
				if (Input.GetInputState(Keyboard.Key.A).IsDown)
				{
					newpos.X = shape.Position.X - movespeed;
				}
				if (Input.GetInputState(Keyboard.Key.D).IsDown)
				{
					newpos.X = shape.Position.X + movespeed;
				}

				shape.Position = newpos;
				
				renderWindow.Clear();
				{
					World.Draw(renderWindow);
					renderWindow.Draw(shape);

					GUI.Draw();
				}
				renderWindow.Display();
			}
		}
	}

	// TODO: Move this to a nother file or somethin'
	class Painter : GUIPainter
	{
		RenderWindow RWind;
		Color Clr;
		
		public Painter(RenderWindow RWind)
		{
			this.RWind = RWind;
		}

		public override void EnableScissor(float X, float Y, float W, float H) {
			UtilsDrawing.EnableScissor(true);
			UtilsDrawing.SetScissor(RWind, (int)X, (int)Y, (int)W, (int)H);
		}

		public override void DisableScissor() {
			UtilsDrawing.EnableScissor(false);
		}

		public override void SetColor(byte R, byte G, byte B, byte A) {
			Clr = new Color(R, G, B, A);
		}

		Text SFMLTxt;
		public override void DrawLabel(float X, float Y, float Size, string Txt)
		{
			if (SFMLTxt == null) 
				SFMLTxt = new Text("", new Font("data\\saxmono.ttf"));

			SFMLTxt.DisplayedString = Txt;
			SFMLTxt.CharacterSize = (uint)Size;
			SFMLTxt.Position = new Vector2f(X, Y);
			SFMLTxt.FillColor = Clr;
			RWind.Draw(SFMLTxt);
		}

		RectangleShape SFMLRect;
		public override void DrawRectangle(float X, float Y, float W, float H)
		{
			if (SFMLRect == null) 
				SFMLRect = new RectangleShape();

			SFMLRect.Position = new Vector2f(X, Y);
			SFMLRect.Size = new Vector2f(W, H);
			SFMLRect.FillColor = Clr;
			RWind.Draw(SFMLRect);
		}
	}
}
