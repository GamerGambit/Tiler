using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

namespace GUIGUI
{
	public class Control
	{
		protected List<Control> _children = new List<Control>();
		public ReadOnlyCollection<Control> Children { get; protected set; }

		public Control Parent = null;

		public Vector2 Position;
		public Vector2 Size;
		public byte R, G, B, A;

		public Control()
		{
			Children = new ReadOnlyCollection<Control>(_children);
		}

		public Vector2 GetGlobalPosition()
		{
			Vector2 ret = new Vector2(0, 0);
			var ctrl = this;

			while (!(ctrl is null))
			{
				ret += ctrl.Position;
				ctrl = ctrl.Parent;
			}

			return ret;
		}

		public Control AddChild(Control child)
		{
			child.Parent?.RemoveChild(child);
			child.Parent = this;
			_children.Add(child);
			return child;
		}

		public void RemoveChild(Control child)
		{
			if (Children.Contains(child))
			{
				child.Parent = null;
				_children.Remove(child);
			}
		}

		public virtual void Draw(Painter P)
		{
			throw new InvalidOperationException();
		}
	}
}
