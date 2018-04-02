﻿using System;
using SFML.Graphics;
using SFML.System;

namespace Tiler
{
	[Spawnable]
	public class PlayerSpawnEntity : Entity
	{
		[MapEditable]
		private string MapTest;

		private RectangleShape shape = new RectangleShape(new SFML.System.Vector2f(32, 32));

		public PlayerSpawnEntity()
		{
			shape.FillColor = new Color(255, 160, 0, 128);
		}

		public PlayerSpawnEntity(float X, float Y) : this()
		{
			Position = new Vector2f(X, Y);
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;
			target.Draw(shape, states);
		}
	}
}
