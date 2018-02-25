using System;
using System.Net;

using SFML.Graphics;
using SFML.Window;
using Tiler;

namespace Client
{
	class Program
	{
		static IPEndPoint ClientEP;
		static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				if (args[0] == "--connect")
				{
					if (args.Length < 2)
						throw new Exception("'--connect' requires a remote address.");

					var epString = args[1];
					var epSplit = epString.Split(':');

					if (epSplit.Length > 2)
						throw new Exception("'--connect' only supports IPv4 with an optional port.");

					if (!IPAddress.TryParse(epSplit[0], out var address))
						throw new Exception("'--connect' was provided with an invalid IP address.");

					var clientPort = Networking.DefaultPort;
					if (epSplit.Length == 2)
					{
						if (!ushort.TryParse(epSplit[1], out clientPort))
							throw new Exception("'--connect' was provided with an invalid port number.");
					}

					ClientEP = new IPEndPoint(address, clientPort);
				}
			}

			Networking.InitClient();
			Networking.Connect(ClientEP);

			var renderWindow = new RenderWindow(new VideoMode(1024, 768), "Map renderer");
			renderWindow.Closed += (s, e) => ((RenderWindow)s).Close();

			/*
			World.LoadChunk("testmap1.tmx", new Vector2i(0, 0));
			World.LoadChunk("testmap1.tmx", new Vector2i(320, 0));
			World.LoadChunk("testmap1.tmx", new Vector2i(640, 0));
			World.LoadChunk("testmap2.tmx", new Vector2i(0, 320));
			World.LoadChunk("testmap2.tmx", new Vector2i(320, 320));
			World.LoadChunk("testmap2.tmx", new Vector2i(640, 320));
			*/

			while (renderWindow.IsOpen)
			{
				Networking.Update();

				renderWindow.DispatchEvents();

				renderWindow.Clear();
				World.Draw(renderWindow);
				renderWindow.Display();
			}

			Networking.Shutdown();
		}
	}
}
