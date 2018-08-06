using System;
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
		private Vector2i size = new Vector2i(0, 0);

		private bool hasFocus = false;
		private bool mouseOver = false;
		private bool mouseInBounds = false;
		private bool layoutDirty = true;

		private void DoLayout()
		{
			Layout();

			foreach (var child in children)
			{
				child.DoLayout();
			}

			layoutDirty = false;
		}

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
						parent.RemoveChild(this);
						parent = null;
						State.Roots.Add(this);
					}

					return;
				}

				if (parent is null)
				{
					State.Roots.Remove(this);
				}

				parent = value;
				parent.AddChild(this);
			}
		}
		public Vector2i Size { get => size; set { size = value; InvalidateLayout(); } }
		public Vector2i GlobalPosition {
			get
			{
				var gpos = new Vector2i(0, 0);
				var ctrl = this;

				while(!(ctrl is null))
				{
					gpos += new Vector2i((int)ctrl.Position.X, (int)ctrl.Position.Y);
					ctrl = ctrl.Parent;
				}

				return gpos;
			}
		}
		public IntRect AABB { get => new IntRect(GlobalPosition, Size); }
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

		public Control()
		{
			State.Roots.Add(this);
		}

		~Control()
		{
			OnRemove();
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
			OnRemove();

			if (parent is null)
			{
				State.Roots.Remove(this);
			}
			else
			{
				parent.RemoveChild(this);
			}
		}

		public bool HasChild(Control child)
		{
			foreach (var c in GetChildren())
			{
				if (c == child)
					return true;
			}

			return false;
		}

		public void AddChild(Control child)
		{
			if (child.parent is null && State.Roots.Contains(child))
			{
				State.Roots.Remove(child);
			}

			child.parent?.RemoveChild(child);

			children.Add(child);
			child.parent = this;

			OnChildAdded(child);
		}

		public void RemoveChild(Control child)
		{
			OnChildRemoved(child);
			child.parent = null;
			children.Remove(child);
		}

		public void RemoveAllChildren()
		{
			foreach (var child in GetChildren())
			{
				RemoveChild(child);
			}

			children.Clear();
		}

		public void InvalidateLayout(bool immediately = false)
		{
			if (immediately)
			{
				DoLayout();
			}
			else
			{
				layoutDirty = true;
			}
		}

		public void Update(TimeSpan deltaTime)
		{
			if (Visible)
			{
				HandledMouseMove();
			}

			if (layoutDirty)
			{
				DoLayout();
			}

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

			target.PushScissor(AABB);

			OnDraw(target, states);

			for (var index = 0; index < children.Count; ++index)
			{
				target.Draw(children[index], states);
			}

			target.PopScissor();
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

		protected virtual void Layout()
		{
			// NOP
		}

		public virtual IEnumerable<Control> GetChildren()
		{
			foreach (var child in children)
				yield return child;
		}

		protected virtual void OnChildAdded(Control child)
		{
			// NOP
		}

		protected virtual void OnChildRemoved(Control child)
		{
			// NOP
		}
	}
}
