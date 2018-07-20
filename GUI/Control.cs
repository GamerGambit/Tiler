using System.Collections.Generic;
using System.Collections.ObjectModel;
using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI
{
	public abstract class Control : Transformable, Drawable
	{
		private Control _Parent = null;
		private List<Control> _Children = new List<Control>();

		public Control Parent
		{
			get => _Parent;
			set
			{
				if (value is null)
				{
					if (!(_Parent is null))
					{
						_Parent._Children.Remove(this);
						_Parent = null;
						State.Roots.Add(this);
					}

					return;
				}

				if (_Parent is null)
				{
					State.Roots.Remove(this);
				}
				else
				{
					_Parent._Children.Remove(this);
				}

				_Parent = value;
				_Parent._Children.Add(this);
			}
		}
		public ReadOnlyCollection<Control> Children { get; private set; }
		public Vector2f Size { get; set; } = new Vector2f(0, 0);
		public Vector2f GlobalPosition {
			get
			{
				var gpos = new Vector2f(0, 0);
				var ctrl = this;

				while(!(ctrl is null))
				{
					gpos += ctrl.Position;
					ctrl = ctrl.Parent;
				}

				return gpos;
			}
		}

		public Control(Control parent)
		{
			Children = new ReadOnlyCollection<Control>(_Children);

			if (parent is null)
			{
				State.Roots.Add(this);
			}
			else
			{
				Parent = parent;
			}
		}

		public void Remove()
		{
			if (_Parent is null)
			{
				State.Roots.Remove(this);
			}
			else
			{
				_Parent._Children.Remove(this);
			}
		}

		public void RemoveAllChildren()
		{
			_Children.Clear();
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			states.Transform *= Transform;

			OnDraw(target, states);

			var offset = State.GetRelativeOffset(target);

			foreach (var child in _Children)
			{
				UtilsDrawing.SetScissor(target, (int)(offset.X + GlobalPosition.X), (int)(offset.Y + GlobalPosition.Y), (int)Size.X, (int)Size.Y);
				target.Draw(child, states);
			}
		}

		public virtual void OnDraw(RenderTarget target, RenderStates states)
		{
			// NOP
		}
	}
}
