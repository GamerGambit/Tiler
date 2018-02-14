﻿using System;
using SFML.Graphics;

namespace Tiler
{
	[Spawnable(true)]
	public class PlayerSpawnEntity : Entity
	{
		[MapEditable(true)]
		private string MapTest;

		private RectangleShape shape = new RectangleShape(new SFML.System.Vector2f(32, 32));

		public PlayerSpawnEntity()
		{
			shape.FillColor = new Color(255, 160, 0, 128);
		}

		protected override void Initialize()
		{
			base.Initialize();

			Console.WriteLine($"PlayerSpawnEntity Initialized. MapTest = {MapTest}.");
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;
			target.Draw(shape, states);
		}
	}
}
