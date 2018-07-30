using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI
{
	public abstract class Control : Transformable, Drawable
	{
		private Control parent = null;
		private List<Control> children = new List<Control>();
		private bool visible = true;

		private bool hasFocus = false;
		private bool mouseOver = false;
		private bool mouseInBounds = false;

		internal bool HandledMouseMove()
		{
			var mousePos = Input.Manager.MousePosition;
			var globalPosition = GlobalPosition;

			if (!(
				mousePos.X >= globalPosition.X && mousePos.X <= globalPosition.X + Size.X &&
				mousePos.Y >= globalPosition.Y && mousePos.Y <= globalPosition.Y + Size.Y
				))
			{
				if (mouseInBounds)
				{
					mouseInBounds = false;
					mouseOver = false;
					OnMouseExit();
					MouseExit?.Invoke(this, EventArgs.Empty);
				}

				return false;
			}

			mouseOver = true;

			if (!mouseInBounds)
			{
				mouseInBounds = true;
				OnMouseEnter();
				MouseEnter?.Invoke(this, EventArgs.Empty);
			}

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledMouseMove())
				{
					if (mouseOver)
					{
						mouseOver = false;
					}

					return true;
				}
			}

			return true;
		}
		internal bool HandledMousePressed(Glfw3.Glfw.MouseButton mouseButton)
		{
			if (!Visible)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledMousePressed(mouseButton))
				{
					hasFocus = false;
					return true;
				}
			}

			if (!mouseOver)
			{
				hasFocus = false;
				return false;
			}

			BringToFront();
			OnMousePressed(mouseButton);
			MousePressed?.Invoke(this, mouseButton);
			hasFocus = true;
			return true;
		}
		internal bool HandledMouseReleased(Glfw3.Glfw.MouseButton mouseButton)
		{
			if (!Visible || !hasFocus)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledMouseReleased(mouseButton))
					return true;
			}

			OnMouseReleased(mouseButton);
			MouseReleased?.Invoke(this, mouseButton);
			return true;
		}
		internal bool HandledMouseScroll()
		{
			if (!Visible || !mouseOver)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledMouseScroll())
					return true;
			}

			OnMouseScroll();
			MouseScrolled?.Invoke(this, EventArgs.Empty);
			return true;
		}
		internal bool HandledKeyPress(Glfw3.Glfw.KeyCode key)
		{
			if (!Visible)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledKeyPress(key))
					return true;
			}

			if (!hasFocus)
				return false;

			OnKeyPressed(key);
			KeyPressed?.Invoke(this, key);
			return true;
		}
		internal bool HandledKeyReleased(Glfw3.Glfw.KeyCode key)
		{
			if (!Visible)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledKeyReleased(key))
					return true;
			}

			if (!hasFocus)
				return false;

			OnKeyReleased(key);
			KeyReleased?.Invoke(this, key);
			return true;
		}

		public event EventHandler MouseEnter;
		public event EventHandler MouseExit;
		public event EventHandler<Glfw3.Glfw.MouseButton> MousePressed;
		public event EventHandler<Glfw3.Glfw.MouseButton> MouseReleased;
		public event EventHandler MouseScrolled;
		public event EventHandler<Glfw3.Glfw.KeyCode> KeyPressed;
		public event EventHandler<Glfw3.Glfw.KeyCode> KeyReleased;

		public Control Parent
		{
			get => parent;
			set
			{
				if (value is null)
				{
					if (!(parent is null))
					{
						parent.children.Remove(this);
						parent = null;
						State.Roots.Add(this);
					}

					return;
				}

				if (parent is null)
				{
					State.Roots.Remove(this);
				}
				else
				{
					parent.children.Remove(this);
				}

				parent = value;
				parent.children.Add(this);
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
		public bool Visible {
			get => visible;
			set
			{
				visible = value;
				if (!visible)
				{
					hasFocus = false;
					mouseOver = false;
				}
			}
		}

		public Control(Control parent)
		{
			Children = new ReadOnlyCollection<Control>(children);

			if (parent is null)
			{
				State.Roots.Add(this);
			}
			else
			{
				Parent = parent;
			}
		}

		public void BringToFront()
		{
			if (parent is null)
			{
				State.Roots.Remove(this);
				State.Roots.Add(this);
			}
			else
			{
				parent.children.Remove(this);
				parent.children.Add(this);
			}
		}

		public void Remove()
		{
			if (parent is null)
			{
				State.Roots.Remove(this);
			}
			else
			{
				parent.children.Remove(this);
			}
		}

		public void RemoveAllChildren()
		{
			children.Clear();
		}

		public void Update(float deltaTime)
		{
			if (!Visible)
				return;

			HandledMouseMove();
			OnUpdate(deltaTime);
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			if (!Visible)
				return;

			states.Transform *= Transform;

			OnDraw(target, states);

			var offset = State.GetRelativeOffset(target);

			for (var index = 0; index < children.Count; ++index)
			{
				UtilsDrawing.SetScissor(target, (int)(offset.X + GlobalPosition.X), (int)(offset.Y + GlobalPosition.Y), (int)Size.X, (int)Size.Y);
				target.Draw(children[index], states);
			}
		}

		public virtual void OnUpdate(float deltaTime)
		{
			// NOP
		}

		public virtual void OnDraw(RenderTarget target, RenderStates states)
		{
			// NOP
		}

		public virtual void OnMouseEnter()
		{
			// NOP
		}

		public virtual void OnMouseExit()
		{
			// NOP
		}

		public virtual void OnMousePressed(Glfw3.Glfw.MouseButton mouseButton)
		{
			// NOP
		}

		public virtual void OnMouseReleased(Glfw3.Glfw.MouseButton mouseButton)
		{
			// NOP
		}

		public virtual void OnMouseScroll()
		{
			// NOP
		}

		public virtual void OnKeyPressed(Glfw3.Glfw.KeyCode key)
		{
			// NOP
		}

		public virtual void OnKeyReleased(Glfw3.Glfw.KeyCode key)
		{
			// NOP
		}
	}
}
