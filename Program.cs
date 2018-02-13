using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Tiler
{
	class Program
	{
		static void Main(string[] args)
		{
			var renderWindow = new RenderWindow(new VideoMode(1024, 768), "Map renderer");
			renderWindow.Closed += (s, e) => ((RenderWindow)s).Close();

			World.LoadChunk("testmap1.tmx", new Vector2i(0, 0));
			World.LoadChunk("testmap1.tmx", new Vector2i(320, 0));
			World.LoadChunk("testmap1.tmx", new Vector2i(640, 0));
			World.LoadChunk("testmap2.tmx", new Vector2i(0, 320));
			World.LoadChunk("testmap2.tmx", new Vector2i(320, 320));
			World.LoadChunk("testmap2.tmx", new Vector2i(640, 320));

			while (renderWindow.IsOpen)
			{
				renderWindow.DispatchEvents();

				renderWindow.Clear();
					World.Draw(renderWindow);
				renderWindow.Display();
			}
		}
	}
}
