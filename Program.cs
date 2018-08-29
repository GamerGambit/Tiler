using System;
using System.Diagnostics;

using Glfw3;

using SFML.System;

namespace Tiler
{
	public abstract class Program : IUpdatable
	{
		private static Stopwatch runtimeSW = new Stopwatch();

		public Window Window;

		public static TimeSpan ElapsedTime { get => runtimeSW.Elapsed; }

		static Program()
		{
			//Glfw.ConfigureNativesDirectory(Path.GetFullPath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)) + "\\thirdparty\\Glfw3");

			string LibDir = IntPtr.Size == sizeof(long) ? CrossPlatform.Platform.GetX64() : CrossPlatform.Platform.GetX86();

			if (CrossPlatform.Platform.SetLibraryDir(LibDir))
				Console.WriteLine("Running as {0}", IntPtr.Size == sizeof(long) ? "64 bit" : "32 bit");
			else
				Environment.Exit(2);

			if (!Glfw.Init())
				Environment.Exit(1);

			runtimeSW.Start();
		}

		public Program()
		{
			Window = new Window(1024, 768, "Tiler Program");
			Window.Activate();
			Window.Closed += (s, e) => ((Window)s).Close();
			Window.KeyPressed += (s, e) => GUI.State.HandleKeyPressed(e);
			Window.KeyReleased += (s, e) => GUI.State.HandleKeyReleased(e);
			Window.UnicodeInput += (s, e) => GUI.State.HandleTextInput(e);
			Window.MousePressed += (s, e) => GUI.State.HandleMousePressed(e);
			Window.MouseReleased += (s, e) => GUI.State.HandleMouseReleased(e);
			Window.MouseScrolled += (s, e) =>
			{
				Input.Manager.MouseWheelDeltas = new System.Numerics.Vector2((float)e.X, (float)e.Y);
				GUI.State.HandleMouseScroll();
			};
			Window.Resized += (s, e) =>
			{
				var rw = ((Window)s).RenderWindow;
				var view = rw.GetView();
				view.Size = new Vector2f(e.Width, e.Height);
				rw.SetView(view);
			};
			Input.Manager.Window = Window;
		}

		public void Run()
		{
			var frameTime = Stopwatch.StartNew();
			float deltaTime;
			while (Window.IsOpen)
			{
				while (frameTime.ElapsedMilliseconds / 1000.0f < (1.0f / 120.0f))
					;
				deltaTime = frameTime.ElapsedMilliseconds / 1000.0f;

				frameTime.Restart();

				Glfw.PollEvents();

				Update(TimeSpan.FromSeconds(deltaTime));

				Window.Clear();
				OnDraw();
				Window.RenderWindow.SetView(Window.RenderWindow.DefaultView);
				GUI.State.Draw(Window);
				Window.Display();
			}

			Glfw.Terminate();
		}

		public void Update(TimeSpan deltaTime)
		{
			Input.Manager.Update(deltaTime);
			GUI.State.Update(deltaTime);

			OnUpdate(deltaTime);
		}

		public abstract void OnDraw();
		public abstract void OnUpdate(TimeSpan deltaTime);
	}
}
