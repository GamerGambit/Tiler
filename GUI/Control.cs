using System;
using System.Collections.Generic;

using Glfw3;

using SFML.Graphics;
using SFML.System;

namespace Tiler.GUI
{
	public abstract class Control : Transformable, Drawable, IUpdatable
	{
		[Flags]
		protected enum EventType
		{
			None = 0,
			MouseEnterExit = 1,
			MousePress = 2,
			MouseRelease = 4,
			MouseScroll = 8,

			KeyPress = 16,
			KeyRelease = 32,
			TextEntered = 64
		}
		protected static EventType Mouse = EventType.MouseEnterExit | EventType.MousePress | EventType.MouseRelease | EventType.MouseScroll;
		protected static EventType Keyboard = EventType.KeyPress | EventType.KeyRelease;
		protected bool HasFocus { get; private set; } = false;

		private Control parent = null;
		private List<Control> children = new List<Control>();
		private bool visible = true;
		private Vector2i size = new Vector2i(0, 0);

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
			if (!Visible)
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

					if (RegisterEventTypes.HasFlag(EventType.MouseEnterExit))
					{
						OnMouseExit();
						MouseExit?.Invoke(this, EventArgs.Empty);
					}
				}

				return false;
			}

			var ret = false;
			mouseOver = true;

			if (!mouseInBounds)
			{
				mouseInBounds = true;

				if (RegisterEventTypes.HasFlag(EventType.MouseEnterExit))
				{
					OnMouseEnter();
					MouseEnter?.Invoke(this, EventArgs.Empty);
					ret = true;
				}
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

			return ret;
		}
		internal bool HandledMousePressed(Glfw.MouseButton mouseButton)
		{
			if (!Visible)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledMousePressed(mouseButton))
				{
					HasFocus = false;
					return true;
				}
			}

			if (!mouseOver)
			{
				HasFocus = false;
				return false;
			}

			// TODO: Add smarter bring-to-front functionality. Right now, clicking on ANY child will bring it to the front which will cause lists of Controls to be reordered.
			//BringToFront();

			HasFocus = true;

			if (!RegisterEventTypes.HasFlag(EventType.MousePress))
				return false;

			OnMousePressed(mouseButton);
			MousePressed?.Invoke(this, mouseButton);

			return true;
		}
		internal bool HandledMouseReleased(Glfw.MouseButton mouseButton)
		{
			if (!Visible)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledMouseReleased(mouseButton))
					return true;
			}

			if (!HasFocus || !mouseInBounds)
				return false;

			if (!RegisterEventTypes.HasFlag(EventType.MouseRelease))
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

			if (!RegisterEventTypes.HasFlag(EventType.MouseScroll) || !mouseOver)
				return false;

			OnMouseScroll();
			MouseScrolled?.Invoke(this, EventArgs.Empty);
			return true;
		}
		internal bool HandledKeyPress(Glfw.KeyCode key)
		{
			if (!Visible)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledKeyPress(key))
					return true;
			}

			if (!RegisterEventTypes.HasFlag(EventType.KeyPress) || !HasFocus)
				return false;

			OnKeyPressed(key);
			KeyPressed?.Invoke(this, key);
			return true;
		}
		internal bool HandledKeyReleased(Glfw.KeyCode key)
		{
			if (!Visible)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledKeyReleased(key))
					return true;
			}

			if (!RegisterEventTypes.HasFlag(EventType.KeyRelease) || !HasFocus)
				return false;

			OnKeyReleased(key);
			KeyReleased?.Invoke(this, key);
			return true;
		}
		internal bool HandledTextInput(uint codepoint)
		{
			if (!visible)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledTextInput(codepoint))
					return true;
			}

			if (!RegisterEventTypes.HasFlag(EventType.TextEntered) || !HasFocus)
				return false;

			OnTextEntered(codepoint);
			TextEntered?.Invoke(this, codepoint);

			return true;
		}

		protected EventType RegisterEventTypes = EventType.None;

		public event EventHandler MouseEnter;
		public event EventHandler MouseExit;
		public event EventHandler<Glfw.MouseButton> MousePressed;
		public event EventHandler<Glfw.MouseButton> MouseReleased;
		public event EventHandler MouseScrolled;
		public event EventHandler<Glfw.KeyCode> KeyPressed;
		public event EventHandler<Glfw.KeyCode> KeyReleased;
		public event EventHandler<uint> TextEntered;

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
					HasFocus = false;
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

		public Vector2i ScreenToLocal(Vector2i vec)
		{
			return vec - GlobalPosition;
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

			if (!(child.parent is null) && child.parent != this)
			{
				child.parent?.RemoveChild(child);
			}

			if (!OnChildAdded(child))
				return;

			children.Add(child);
			child.parent = this;
		}

		public void RemoveChild(Control child)
		{
			if (!HasChild(child))
				return;

			if (!OnChildRemoved(child))
				return;

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

		public void SizeToParent()
		{
			if (parent is null)
				return;

			Size = parent.GetInternalSize();
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

		public virtual void OnMouseEnter()
		{
			// NOP
		}

		public virtual void OnMouseExit()
		{
			// NOP
		}

		public virtual void OnMousePressed(Glfw.MouseButton mouseButton)
		{
			// NOP
		}

		public virtual void OnMouseReleased(Glfw.MouseButton mouseButton)
		{
			// NOP
		}

		public virtual void OnMouseScroll()
		{
			// NOP
		}

		public virtual void OnKeyPressed(Glfw.KeyCode key)
		{
			// NOP
		}

		public virtual void OnKeyReleased(Glfw.KeyCode key)
		{
			// NOP
		}

		public virtual void OnTextEntered(uint codepoint)
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="child"></param>
		/// <returns>Whether or not to add the control</returns>
		protected virtual bool OnChildAdded(Control child)
		{
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="child"></param>
		/// <returns>Whether or not to remove the control</returns>
		protected virtual bool OnChildRemoved(Control child)
		{
			return true;
		}

		protected virtual Vector2i GetInternalSize()
		{
			return Size;
		}
	}
}
