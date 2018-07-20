using System;
using System.Diagnostics;
using System.IO;

using Glfw3;
using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	public abstract class Program
	{
		public Window Window;

		static Program()
		{
			Glfw.ConfigureNativesDirectory(Path.GetFullPath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)) + "\\thirdparty\\Glfw3");
			string foo = Path.GetFullPath(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
			if (!Glfw.Init())
			{
				Environment.Exit(1);
			}
		}

		public Program()
		{
			Window = new Window(1024, 768, "Tiler Program");
			Window.Activate();
			Window.Closed += (s, e) => ((Window)s).Close();
			Window.KeyPressed += (s, e) =>
			{
				var (index, state) = Input.Manager.GetStateWithIndex(e.Key);
				state.IsDown = true;
				Input.Manager.inputStates[index] = state;
			};
			Window.KeyReleased += (s, e) =>
			{
				var (index, state) = Input.Manager.GetStateWithIndex(e.Key);
				state.IsDown = false;
				Input.Manager.inputStates[index] = state;
			};
			Window.MousePressed += (s, e) =>
			{
				var (index, state) = Input.Manager.GetStateWithIndex(e.Button);
				state.IsDown = true;
				Input.Manager.inputStates[index] = state;
			};
			Window.MouseReleased += (s, e) =>
			{
				var (index, state) = Input.Manager.GetStateWithIndex(e.Button);
				state.IsDown = true;
				Input.Manager.inputStates[index] = state;
			};
			Window.MouseScrolled += (s, e) =>
			{
				Input.Manager.MouseWheelDeltas = new System.Numerics.Vector2((float)e.X, (float)e.Y);
			};
			Window.Resized += (s, e) =>
			{
				var rw = ((Window)s).RenderWindow;
				var view = rw.GetView();
				view.Size = new Vector2f(e.Width, e.Height);
				rw.SetView(view);
			};
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

				Update(deltaTime);

				Window.Clear();
				OnDraw();
				GUI.State.Draw(Window);
				Window.Display();
			}
		}

		public void Update(float deltaTime)
		{
			Input.Manager.Update(deltaTime);

			OnUpdate(deltaTime);
		}

		public abstract void OnDraw();
		public abstract void OnUpdate(float deltaTime);
	}
}
