using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Drag Object", 82)]
	public class UIDragObject : UIBehaviour, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[Serializable] public class BeginDragEvent : UnityEvent<BaseEventData> { }
		[Serializable] public class EndDragEvent : UnityEvent<BaseEventData> { }
		[Serializable] public class DragEvent : UnityEvent<BaseEventData> { }
		
		[SerializeField] private RectTransform m_Target;
		[SerializeField] private bool m_Horizontal = true;
		[SerializeField] private bool m_Vertical = true;
		[SerializeField] private bool m_Inertia = true;
		[SerializeField] private float m_DecelerationRate = 0.135f;
		
		private Vector2 m_PointerStartPosition = Vector2.zero;
		private Vector2 m_TargetStartPosition = Vector2.zero;
		private Vector2 m_Velocity;
		private bool m_Dragging;
		private Vector2 m_LastPosition = Vector2.zero;
		
		/// <summary>
		/// The on begin drag event.
		/// </summary>
		public BeginDragEvent onBeginDrag = new BeginDragEvent();
		
		/// <summary>
		/// The on end drag event.
		/// </summary>
		public EndDragEvent onEndDrag = new EndDragEvent();
		
		/// <summary>
		/// The on drag event.
		/// </summary>
		public DragEvent onDrag = new DragEvent();
		
		/// <summary>
		/// Gets or sets the target.
		/// </summary>
		/// <value>The target.</value>
		public RectTransform target
		{
			get
			{
				return this.m_Target;
			}
			set
			{
				this.m_Target = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UnityEngine.UI.UIDragObject"/> is allowed horizontal movement.
		/// </summary>
		/// <value><c>true</c> if horizontal; otherwise, <c>false</c>.</value>
		public bool horizontal
		{
			get
			{
				return this.m_Horizontal;
			}
			set
			{
				this.m_Horizontal = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UnityEngine.UI.UIDragObject"/> is allowed vertical movement.
		/// </summary>
		/// <value><c>true</c> if vertical; otherwise, <c>false</c>.</value>
		public bool vertical
		{
			get
			{
				return this.m_Vertical;
			}
			set
			{
				this.m_Vertical = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UnityEngine.UI.UIDragObject"/> should use inertia.
		/// </summary>
		/// <value><c>true</c> if intertia; otherwise, <c>false</c>.</value>
		public bool inertia
		{
			get
			{
				return this.m_Inertia;
			}
			set
			{
				this.m_Inertia = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the deceleration rate for the inertia.
		/// </summary>
		/// <value>The deceleration rate.</value>
		public float decelerationRate
		{
			get
			{
				return this.m_DecelerationRate;
			}
			set
			{
				this.m_DecelerationRate = value;
			}
		}
	
		protected override void OnEnable()
		{
			base.OnEnable();
		}
		
		public override bool IsActive()
		{
			return base.IsActive() && this.m_Target != null;
		}
		
		/// <summary>
		/// Stops the inertia movement.
		/// </summary>
		public void StopMovement()
		{
			this.m_Velocity = Vector2.zero;
		}
		
		/// <summary>
		/// Raises the begin drag event.
		/// </summary>
		/// <param name="data">Data.</param>
		public void OnBeginDrag(PointerEventData data)
		{
			if (!this.IsActive())
				return;
			
			this.m_PointerStartPosition = data.position;
			this.m_TargetStartPosition = this.m_Target.anchoredPosition;
			this.m_Velocity = Vector2.zero;
			this.m_Dragging = true;
			
			// Invoke the event
			if (this.onBeginDrag != null)
				this.onBeginDrag.Invoke(data as BaseEventData);
		}
		
		public void OnEndDrag(PointerEventData data)
		{
			this.m_Dragging = false;
			
			if (!this.IsActive())
				return;
			
			// Invoke the event
			if (this.onEndDrag != null)
				this.onEndDrag.Invoke(data as BaseEventData);
		}
		
		public void OnDrag(PointerEventData data)
		{
			if (!this.IsActive())
				return;
			
			Vector2 a = data.position;
			Vector2 b = a - this.m_PointerStartPosition;
			Vector2 vector = this.m_TargetStartPosition + b;
			
			// Restrict movement on the axis
			if (!this.m_Horizontal)
			{
				vector.x = this.m_Target.anchoredPosition.x;
			}
			if (!this.m_Vertical)
			{
				vector.y = this.m_Target.anchoredPosition.y;
			}
			
			// Apply the position change
			this.m_Target.anchoredPosition = vector;
			
			// Invoke the event
			if (this.onDrag != null)
				this.onDrag.Invoke(data as BaseEventData);
		}
		
		protected virtual void LateUpdate()
		{
			if (!this.m_Target)
				return;
			
			// Capture the velocity of our drag to be used for the inertia
			if (this.m_Dragging && this.m_Inertia)
			{
				Vector3 to = (this.m_Target.anchoredPosition - this.m_LastPosition) / Time.unscaledDeltaTime;
				this.m_Velocity = Vector3.Lerp(this.m_Velocity, to, Time.unscaledDeltaTime * 10f);
			}
			
			this.m_LastPosition = this.m_Target.anchoredPosition;
			
			// Handle inertia only when not dragging
			if (!this.m_Dragging && this.m_Velocity != Vector2.zero)
			{
				Vector2 anchoredPosition = this.m_Target.anchoredPosition;
				
				for (int i = 0; i < 2; i++)
				{
					// Calculate the inerta amount to be applied on this update
					if (this.m_Inertia)
					{
						this.m_Velocity[i] *= Mathf.Pow(this.m_DecelerationRate, Time.unscaledDeltaTime);
						anchoredPosition[i] += this.m_Velocity[i] * Time.unscaledDeltaTime;
					}
					else
					{
						this.m_Velocity[i] = 0f;
					}
				}
				
				if (this.m_Velocity != Vector2.zero)
				{
					// Restrict movement on the axis
					if (!this.m_Horizontal)
					{
						anchoredPosition.x = this.m_Target.anchoredPosition.x;
					}
					if (!this.m_Vertical)
					{
						anchoredPosition.y = this.m_Target.anchoredPosition.y;
					}
					
					// Apply the inertia
					if (anchoredPosition != this.m_Target.anchoredPosition)
					{
						this.m_Target.anchoredPosition = anchoredPosition;
					}
				}
			}
		}
	}
}
