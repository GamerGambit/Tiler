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
		public enum EventType
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
		protected bool MouseInBounds { get; private set; } = false;
		protected bool MouseDirectlyOver { get; private set; } = false;

		private Control parent = null;
		private List<Control> children = new List<Control>();
		private bool visible = true;
		private Vector2i size = new Vector2i(0, 0);

		private bool layoutDirty = true;
		private bool enabled = true;

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
			if (!Visible || !enabled)
				return false;

			var mousePos = Input.Manager.MousePosition;
			var globalPosition = GlobalPosition;

			if (!(
				mousePos.X >= globalPosition.X && mousePos.X <= globalPosition.X + Size.X &&
				mousePos.Y >= globalPosition.Y && mousePos.Y <= globalPosition.Y + Size.Y
				))
			{
				if (MouseInBounds)
				{
					MouseInBounds = false;
					MouseDirectlyOver = false;

					if (RegisterEventTypes.HasFlag(EventType.MouseEnterExit))
					{
						OnMouseExit();
						MouseExit?.Invoke(this, EventArgs.Empty);
					}
				}

				return false;
			}

			var ret = false;
			MouseDirectlyOver = true;

			if (!MouseInBounds)
			{
				MouseInBounds = true;

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
					if (MouseDirectlyOver)
					{
						MouseDirectlyOver = false;
					}

					return true;
				}
			}

			return ret;
		}
		internal bool HandledMousePressed(MouseButtonEventArgs e)
		{
			if (!Visible || !enabled)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledMousePressed(e))
				{
					HasFocus = false;
					return true;
				}
			}

			if (!MouseDirectlyOver)
			{
				HasFocus = false;
				return false;
			}

			// TODO: Add smarter bring-to-front functionality. Right now, clicking on ANY child will bring it to the front which will cause lists of Controls to be reordered.
			//BringToFront();

			HasFocus = true;
			State.FocusedControl = this;

			if (!RegisterEventTypes.HasFlag(EventType.MousePress))
				return false;

			OnMousePressed(e.Button, e.Modifiers);
			MousePressed?.Invoke(this, e);

			return true;
		}
		internal bool HandledMouseReleased(MouseButtonEventArgs e)
		{
			if (!Visible || !enabled)
				return false;

			bool handled = false;
			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledMouseReleased(e))
				{
					handled = true;
				}
			}

			if (!HasFocus)
				return handled;

			if (!RegisterEventTypes.HasFlag(EventType.MouseRelease))
				return handled;

			OnMouseReleased(e.Button, e.Modifiers);
			MouseReleased?.Invoke(this, e);

			return true;
		}
		internal bool HandledMouseScroll()
		{
			if (!Visible || !enabled)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledMouseScroll())
					return true;
			}

			if (!RegisterEventTypes.HasFlag(EventType.MouseScroll) || !MouseDirectlyOver)
				return false;

			OnMouseScroll();
			MouseScrolled?.Invoke(this, EventArgs.Empty);
			return true;
		}
		internal bool HandledKeyPress(KeyEventArgs e)
		{
			if (!Visible || !enabled)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledKeyPress(e))
					return true;
			}

			if (!RegisterEventTypes.HasFlag(EventType.KeyPress) || !HasFocus)
				return false;

			OnKeyPressed(e.Key, e.Modifiers);
			KeyPressed?.Invoke(this, e);
			return true;
		}
		internal bool HandledKeyReleased(KeyEventArgs e)
		{
			if (!Visible || !enabled)
				return false;

			bool handled = false;
			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledKeyReleased(e))
				{
					handled = true;
				}
			}

			if (!RegisterEventTypes.HasFlag(EventType.KeyRelease) || !HasFocus)
				return handled;

			OnKeyReleased(e.Key, e.Modifiers);
			KeyReleased?.Invoke(this, e);
			return true;
		}
		internal bool HandledTextInput(UnicodeInputEventArgs e)
		{
			if (!visible || !enabled)
				return false;

			for (var index = children.Count - 1; index >= 0; --index)
			{
				if (children[index].HandledTextInput(e))
					return true;
			}

			if (!RegisterEventTypes.HasFlag(EventType.TextEntered) || !HasFocus)
				return false;

			OnTextEntered(e.CodePoint, e.Modifiers);
			TextEntered?.Invoke(this, e);

			return true;
		}

		public event EventHandler MouseEnter;
		public event EventHandler MouseExit;
		public event EventHandler<MouseButtonEventArgs> MousePressed;
		public event EventHandler<MouseButtonEventArgs> MouseReleased;
		public event EventHandler MouseScrolled;
		public event EventHandler<KeyEventArgs> KeyPressed;
		public event EventHandler<KeyEventArgs> KeyReleased;
		public event EventHandler<UnicodeInputEventArgs> TextEntered;

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
					MouseDirectlyOver = false;
				}
			}
		}
		public bool Enabled
		{
			get => enabled;
			set
			{
				enabled = value;
				if (!enabled)
				{
					HasFocus = false;
					MouseDirectlyOver = false;
				}
			}
		}
		public EventType RegisterEventTypes { get; set; } = EventType.None;
		public bool HasFocus { get; private set; } = false;

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

		public virtual void OnMousePressed(Glfw.MouseButton mouseButton, Glfw.KeyMods modifiers)
		{
			// NOP
		}

		public virtual void OnMouseReleased(Glfw.MouseButton mouseButton, Glfw.KeyMods modifiers)
		{
			// NOP
		}

		public virtual void OnMouseScroll()
		{
			// NOP
		}

		public virtual void OnKeyPressed(Glfw.KeyCode key, Glfw.KeyMods modifiers)
		{
			// NOP
		}

		public virtual void OnKeyReleased(Glfw.KeyCode key, Glfw.KeyMods modifiers)
		{
			// NOP
		}

		public virtual void OnTextEntered(uint codepoint, Glfw.KeyMods modifiers)
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
