using System;
using System.Collections.Generic;

using SFML.Graphics;
using SFML.System;

using Tiler;

namespace Client
{
	public class Game : Tiler.Program
	{
		public Gamemode Gamemode;
		public Player Player;

		public View GameView;

		public Game() : base()
		{
			Window.Title = "Habitat Game Thingo";
			GameView = new View(Window.RenderWindow.GetView());

			/*
			var font = new Font("data/saxmono.ttf");

			var window = new Tiler.GUI.Controls.Window()
			{
				Font = font,
				Position = new Vector2i(100, 100),
				Size = new Vector2i(300, 300)
			};

			var keybinder = new Tiler.GUI.Controls.KeyBinder()
			{
				Parent = window,
				Size = new Vector2i(250, 0),
				Font = font,
				CharacterSize = 14
			};
			keybinder.KeyPressed += (s, e) => keybinder.SizeToContents();
			keybinder.SizeToContents();

			var textinput = new Tiler.GUI.Controls.TextInput()
			{
				Parent = window,
				Position = new Vector2i(0, 30),
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
			controlList.Position = new Vector2i(0, textinput.Size.Y + 2);
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
					FillColor = new Color(Utils.RandomByte(), Utils.RandomByte(), Utils.RandomByte()),
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
				TileIDs = new Map.TileType[]
				{
					Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space,
					Map.TileType.Space, Map.TileType.Wall , Map.TileType.Wall , Map.TileType.Wall , Map.TileType.Wall , Map.TileType.Wall , Map.TileType.Wall , Map.TileType.Wall , Map.TileType.Wall , Map.TileType.Space,
					Map.TileType.Space, Map.TileType.Wall , Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Wall , Map.TileType.Space,
					Map.TileType.Space, Map.TileType.Wall , Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Wall , Map.TileType.Space,
					Map.TileType.Space, Map.TileType.Wall , Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Wall , Map.TileType.Space,
					Map.TileType.Space, Map.TileType.Wall , Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Wall , Map.TileType.Space,
					Map.TileType.Space, Map.TileType.Wall , Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Wall , Map.TileType.Space,
					Map.TileType.Space, Map.TileType.Wall , Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Floor, Map.TileType.Wall , Map.TileType.Space,
					Map.TileType.Space, Map.TileType.Wall , Map.TileType.Wall , Map.TileType.Floor , Map.TileType.Floor , Map.TileType.Wall , Map.TileType.Wall , Map.TileType.Wall , Map.TileType.Wall , Map.TileType.Space,
					Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space, Map.TileType.Space
				}
			};
			World.Entities.Add(new PlayerSpawn(192, 192));
			World.Map.Rebuild();

			Gamemode = new TestGamemode();
			Player = new Player();

			Gamemode.CreateTeams();
			Gamemode.PlayerInitialSpawn(Player);
			PlayerSpawn spawnPoint = Gamemode.PlayerSelectSpawn(Player);

			if (spawnPoint is null)
				throw new Exception("No spawnpoint");
			
			// Original
			//Player.Position = spawnPoint.Position;

			var Body = Player.GetComponent<Tiler.Physics.Body>(EntityComponents.PhysicsBody);
			Body.Position = spawnPoint.SpawnPosition;
			Player.SetComponent(EntityComponents.PhysicsBody, Body);

			Gamemode.PlayerSpawn(Player);
		}

		public override void OnDraw()
		{
			Window.RenderWindow.SetView(GameView);
			World.Draw(Window);

			Player.Draw(Window.RenderWindow, RenderStates.Default);
		}

		public override void OnUpdate(TimeSpan deltaTime)
		{
			MoveData mv = new MoveData();
			Gamemode.SetupMove(Player, ref mv);
			Gamemode.Move(Player, mv, deltaTime);

			var Pos = Player.GetComponent<Tiler.Physics.Body>(EntityComponents.PhysicsBody).Position;
			GameView.Center = new Vector2f(Pos.X, Pos.Y);
		}
	}
}
