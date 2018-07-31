﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI
{
	public abstract class Control : Transformable, Drawable, IUpdatable
	{
		private Control parent = null;
		private List<Control> children = new List<Control>();
		private bool visible = true;

		private bool hasFocus = false;
		private bool mouseOver = false;
		private bool mouseInBounds = false;

		internal bool HandledMouseMove()
		{
			if (!Visible || !HandlesMouseInput)
				return false;

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

			if (!HandlesMouseInput || !mouseOver)
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
			if (!Visible)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledMouseReleased(mouseButton))
					return true;
			}

			if (!HandlesMouseInput || !hasFocus || !mouseInBounds)
				return false;

			OnMouseReleased(mouseButton);
			MouseReleased?.Invoke(this, mouseButton);
			return true;
		}
		internal bool HandledMouseScroll()
		{
			if (!Visible)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledMouseScroll())
					return true;
			}

			if (!HandlesMouseInput || !mouseOver)
				return false;

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

			if (!HandlesKeyboardInput || !hasFocus)
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

			if (!HandlesKeyboardInput || !hasFocus)
				return false;

			OnKeyReleased(key);
			KeyReleased?.Invoke(this, key);
			return true;
		}

		protected bool HandlesKeyboardInput { get; set; } = true;
		protected bool HandlesMouseInput { get; set; } = true;

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

		public void Update(TimeSpan deltaTime)
		{
			if (!Visible)
				return;

			HandledMouseMove();
			OnUpdate(deltaTime);

			foreach (var child in children)
			{
				child.Update(deltaTime);
			}
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			if (!Visible)
				return;

			states.Transform *= Transform;

			var offset = State.GetRelativeOffset(target);
			ScissorStack.Push(new ScissorRect()
			{
				Position = new Vector2i((int)(offset.X + GlobalPosition.X), (int)(offset.Y + GlobalPosition.Y)),
				Size = new Vector2i((int)Size.X, (int)Size.Y)
			});

			OnDraw(target, states);

			for (var index = 0; index < children.Count; ++index)
			{
				target.Draw(children[index], states);
			}

			ScissorStack.Pop();
		}

		protected virtual void OnUpdate(TimeSpan deltaTime)
		{
			// NOP
		}

		protected virtual void OnDraw(RenderTarget target, RenderStates states)
		{
			// NOP
		}

		protected virtual void OnRemove()
		{
			// NOP
		}

		protected virtual void OnMouseEnter()
		{
			// NOP
		}

		protected virtual void OnMouseExit()
		{
			// NOP
		}

		protected virtual void OnMousePressed(Glfw3.Glfw.MouseButton mouseButton)
		{
			// NOP
		}

		protected virtual void OnMouseReleased(Glfw3.Glfw.MouseButton mouseButton)
		{
			// NOP
		}

		protected virtual void OnMouseScroll()
		{
			// NOP
		}

		protected virtual void OnKeyPressed(Glfw3.Glfw.KeyCode key)
		{
			// NOP
		}

		protected virtual void OnKeyReleased(Glfw3.Glfw.KeyCode key)
		{
			// NOP
		}
	}
}
