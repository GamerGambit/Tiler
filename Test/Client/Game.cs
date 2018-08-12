using System;
using System.Collections.Generic;

using SFML.System;

using Tiler;

namespace Client
{
	public class Game : Tiler.Program
	{
		public Gamemode Gamemode;
		public Player Player;

		public SFML.Graphics.View GameView;

		public Game() : base()
		{
			Window.Title = "Habitat Game Thingo";
			GameView = new SFML.Graphics.View(Window.RenderWindow.GetView());

			var font = new SFML.Graphics.Font("data/saxmono.ttf");

			var window = new Tiler.GUI.Controls.Window()
			{
				Font = font,
				Position = new Vector2f(100, 100),
				Size = new Vector2i(300, 300)
			};

			var textinput = new Tiler.GUI.Controls.TextInput()
			{
				Parent = window,
				Font = font,
				CharacterSize = 21,
				//MaxCharacters = 10
			};
			window.InvalidateLayout(true);
			textinput.SizeToParent();

			var controlList = new Tiler.GUI.Controls.ControlList()
			{
				Parent = window,
				YPadding = 2
			};
			window.InvalidateLayout(true);
			controlList.SizeToParent();
			controlList.Position = new Vector2f(0, textinput.Size.Y + 2);
			controlList.Size = new Vector2i(controlList.Size.X, controlList.Size.Y - (int)controlList.Position.Y);
			textinput.Submit += (s, e) =>
			{
				if (textinput.Text == "/close")
				{
					window.CloseWindow();
					return;
				}
				else if (textinput.Text.StartsWith("/chsize"))
				{
					textinput.CharacterSize = uint.Parse(textinput.Text.Substring(8));
					textinput.InvalidateLayout();
					textinput.Text = "";
					return;
				}

				var label = new Tiler.GUI.Controls.Label()
				{
					Font = font,
					CharacterSize = 12,
					Text = textinput.Text,
					FillColor = new SFML.Graphics.Color(Utils.RandomByte(), Utils.RandomByte(), Utils.RandomByte()),
					WrapType = Tiler.GUI.Controls.Label.WrapTypes.Normal,
					Size = new Vector2i(controlList.Size.X, 0)
				};
				label.SizeToContents();

				controlList.AddChild(label);
				textinput.Text = "";
			};

			/*
			for (var count = 0; count < 50; ++count)
			{
				var i = count;
				var label = new Tiler.GUI.Controls.Label()
				{
					Font = font,
					CharacterSize = 12,
					Text = "Label " + i.ToString()
				};
				label.SizeToContents();
				controlList.AddChild(label);
			}
			*/

			World.Map = new Map
			{
				Size = new Vector2i(10, 10),
				TileIDs = new List<int>()
				{
					0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
					0, 2, 2, 2, 2, 2, 2, 2, 2, 0,
					0, 2, 1, 1, 1, 1, 1, 1, 2, 0,
					0, 2, 1, 1, 1, 1, 1, 1, 2, 0,
					0, 2, 1, 1, 1, 1, 1, 1, 2, 0,
					0, 2, 1, 1, 1, 1, 1, 1, 2, 0,
					0, 2, 1, 1, 1, 1, 1, 1, 2, 0,
					0, 2, 1, 1, 1, 1, 1, 1, 2, 0,
					0, 2, 2, 2, 2, 2, 2, 2, 2, 0,
					0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				}
			};
			World.Entities.Add(new PlayerSpawn(192, 192));
			World.Map.Rebuild();

			Gamemode = new TestGamemode();
			Player = new Player();

			Gamemode.CreateTeams();
			Gamemode.PlayerInitialSpawn(Player);
			var spawnPoint = Gamemode.PlayerSelectSpawn(Player);

			if (spawnPoint is null)
				throw new Exception("No spawnpoint");

			Player.Position = spawnPoint.Position;
			Gamemode.PlayerSpawn(Player);

			GameView.Center = Player.Position;
		}

		public override void OnDraw()
		{
			Window.RenderWindow.SetView(GameView);
			World.Draw(Window);
			Window.Draw(Player);
		}

		public override void OnUpdate(TimeSpan deltaTime)
		{
			var ucmd = new UserCommand();
			var mv = new MoveData();

			Gamemode.CreateUserCommand(ucmd);
			Gamemode.SetupMove(Player, ucmd, mv);
			Gamemode.Move(Player, mv);

			Player.Velocity += mv.Velocity * (float)deltaTime.TotalSeconds;
			Player.Position += Player.Velocity;

			GameView.Move(Player.Velocity);

			Player.Velocity -= Player.Velocity * 0.1f;
		}
	}
}
