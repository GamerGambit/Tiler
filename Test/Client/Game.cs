using System;
using System.Drawing;

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

			var font = new SFML.Graphics.Font("data/saxmono.ttf");

			var window = new Tiler.GUI.Controls.Window()
			{
				Font = font,
				Position = new Vector2i(100, 100),
				Size = new Vector2i(300, 300)
			};
			window.InvalidateLayout(true);

			var button = new Tiler.GUI.Controls.Button()
			{
				Parent = window,
				Font = font,
				CharacterSize = 14,
				Text = "Animate",
				FillColor = new SFML.Graphics.Color(100, 100, 100),
				TextColor = SFML.Graphics.Color.Black
			};
			button.SizeToParent();
			button.Click += (s, e) =>
			{
				if (window.Animating)
					return;

				var anim = window.NewAnimation<Tiler.GUI.Animations.Move>();
				anim.Duration = 1;
				anim.Finish += (s2, e2) => Console.WriteLine("Finished animating");

				if (window.Position == new Vector2i(200, 200))
				{
					anim.EndPosition = new Vector2i(100, 100);
				}
				else
				{
					anim.EndPosition = new Vector2i(200, 200);
				}

				anim.Animate();
			};

			Gamemode = new TestGamemode();
			Gamemode.RegisterTileTypes();

			using (Bitmap MapBmp = new Bitmap("data/map.png"))
			{
				World.Map = new Map(MapBmp.Width, MapBmp.Height);

				for (int Y = 0; Y < MapBmp.Height; Y++)
					for (int X = 0; X < MapBmp.Width; X++)
					{
						var MapPx = MapBmp.GetPixel(X, Y);
						var Tile = 0;

						if (MapPx.R == 255 && MapPx.G == 255 && MapPx.B == 255 && MapPx.A == 255)
							Tile = 1;
						else if (MapPx.R == 0 && MapPx.G == 0 && MapPx.B == 0 && MapPx.A == 255)
							Tile = 2;
						else if (MapPx.R == 0 && MapPx.G == 255 && MapPx.B == 0 && MapPx.A == 255)
							Tile = 3;

						World.Map.SetTile(X, Y, Tile);
					}
			}

			World.Entities.Add(new PlayerSpawn(192, 192));

			Player = new Player();

			Gamemode.CreateTeams();
			Gamemode.PlayerInitialSpawn(Player);
			PlayerSpawn spawnPoint = Gamemode.PlayerSelectSpawn(Player);

			if (spawnPoint is null)
				throw new Exception("No spawnpoint");

			// Original
			//Player.Position = spawnPoint.Position;

			var Body = Player.GetComponent<Tiler.ECS.Components.PhysicsBody>().Value;
			Body.Position = spawnPoint.SpawnPosition;
			Player.GetComponent<Tiler.ECS.Components.PhysicsBody>().Value = Body;

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

			var Pos = Player.GetComponent<Tiler.ECS.Components.PhysicsBody>().Value.Position;
			GameView.Center = new Vector2f(Pos.X, Pos.Y);
		}
	}
}
