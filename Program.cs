using SFML.Graphics;
using SFML.Window;

namespace Tiler
{
	class Program
	{
		static void Main(string[] args)
		{
			var renderWindow = new RenderWindow(new VideoMode(1024, 768), "Map renderer");
			renderWindow.Closed += (s, e) => ((RenderWindow)s).Close();

			var map = new MapChunk();
			map.Load("untitled.tmx");

			while (renderWindow.IsOpen)
			{
				renderWindow.DispatchEvents();

				renderWindow.Clear();
				renderWindow.Draw(map);
				renderWindow.Display();
			}
		}
	}
}
