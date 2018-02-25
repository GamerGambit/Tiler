using System;
using System.Threading.Tasks;

using Tiler;

namespace Server
{
	class Program
	{
		static ushort ServerPort = Networking.DefaultPort;
		static bool Running = true;

		static void ParseCommandLineInput(string clinput)
		{
			var split = clinput.Split(' ');

			if (split[0] == "quit")
			{
				Running = false;
				return;
			}
			else if (split[0] == "print")
			{
				Console.WriteLine(string.Join(" ", split, 1, split.Length - 1));
			}
		}

		static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				if (args[0] == "--port")
				{
					if (args.Length < 2)
						throw new Exception("'--port' requires a port number.");

					if (!ushort.TryParse(args[1], out ServerPort))
						throw new Exception("'--port' was provided with an invalid port number");
				}
			}

			Networking.Data += (s, e) =>
			{
				Console.WriteLine($"Received message from {e.SenderConnection}");
			};

			Networking.StatusChanged += (s, e) =>
			{
				var status = (Lidgren.Network.NetConnectionStatus)e.ReadByte();
				Console.WriteLine($"StatusChanged from {e.SenderConnection}: {status}");
			};

			Networking.InitServer(ServerPort);

			// Only runs once. If you press [enter], the task will exit
			var task = Task.Factory.StartNew(() => Console.ReadLine());

			while (Running)
			{
				if (task.Wait(TimeSpan.FromMilliseconds(100)))
				{
					if (!string.IsNullOrEmpty(task.Result))
					{
						ParseCommandLineInput(task.Result);
					}

					// Hack to get around the task running only once
					task = Task.Factory.StartNew(() => Console.ReadLine());
				}

				Networking.Update();
			}

			Networking.Shutdown();
		}
	}
}
