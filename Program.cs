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

			World.LoadChunk("untitled.tmx", new Vector2i(0, 0));

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
