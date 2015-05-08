using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace UnityEngine.UI
{
	[ExecuteInEditMode, AddComponentMenu("UI/Button Extended - Target", 58), RequireComponent(typeof(UIButtonExtended))]
	public class UIButtonExtended_Target : MonoBehaviour {
		
		[SerializeField] private Selectable.Transition m_Transition = Selectable.Transition.ColorTint;
		[SerializeField] private ColorBlock m_Colors = ColorBlock.defaultColorBlock;
		[SerializeField] private SpriteState m_SpriteState;
		[SerializeField] private AnimationTriggers m_AnimationTriggers = new AnimationTriggers();
		
		[SerializeField, Tooltip("Graphic that will have the selected transtion applied.")]
		private Graphic m_TargetGraphic;
		
		[SerializeField, Tooltip("GameObject that will have the selected transtion applied.")]
		private GameObject m_TargetGameObject;
		
		private bool m_HasEnableRun;
		
		/// <summary>
		/// Gets or sets the transition type.
		/// </summary>
		/// <value>The transition.</value>
		public Selectable.Transition transition
		{
			get
			{
				return this.m_Transition;
			}
			set
			{
				this.m_Transition = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the colors.
		/// </summary>
		/// <value>The colors.</value>
		public ColorBlock colors
		{
			get
			{
				return this.m_Colors;
			}
			set
			{
				this.m_Colors = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the sprite state.
		/// </summary>
		/// <value>The state of the sprite.</value>
		public SpriteState spriteState
		{
			get
			{
				return this.m_SpriteState;
			}
			set
			{
				this.m_SpriteState = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the animation triggers.
		/// </summary>
		/// <value>The animation triggers.</value>
		public AnimationTriggers animationTriggers
		{
			get
			{
				return this.m_AnimationTriggers;
			}
			set
			{
				this.m_AnimationTriggers = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the target graphic.
		/// </summary>
		/// <value>The target graphic.</value>
		public Graphic targetGraphic
		{
			get
			{
				return this.m_TargetGraphic;
			}
			set
			{
				this.m_TargetGraphic = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the target game object.
		/// </summary>
		/// <value>The target game object.</value>
		public GameObject targetGameObject
		{
			get
			{
				return this.m_TargetGameObject;
			}
			set
			{
				this.m_TargetGameObject = value;
			}
		}
		
		/// <summary>
		/// Gets the animator.
		/// </summary>
		/// <value>The animator.</value>
		public Animator animator
		{
			get
			{
				if (this.m_TargetGameObject != null)
					return this.m_TargetGameObject.GetComponent<Animator>();
					
				// Default
				return null;
			}
		}
		
		protected void Awake()
		{
			// Get the button
			UIButtonExtended button = this.GetComponent<UIButtonExtended>();
			
			// Add event listener
			if (button != null)
				button.onStateChange.AddListener(OnStateChange);
		}
		
		protected void OnEnable()
		{
			this.m_HasEnableRun = true;
			this.InternalEvaluateAndTransitionToNormalState(true);
		}

		protected void OnDisable()
		{
			this.InstantClearState();
		}
		
		protected void OnValidate()
		{
			this.m_Colors.fadeDuration = Mathf.Max(this.m_Colors.fadeDuration, 0f);
			
			if (this.m_HasEnableRun)
			{
				this.DoSpriteSwap(null);
				this.InternalEvaluateAndTransitionToNormalState(true);
			}
		}
		
		/// <summary>
		/// Instantly clears the visual state.
		/// </summary>
		protected void InstantClearState()
		{
			switch (this.m_Transition)
			{
				case Selectable.Transition.ColorTint:
					this.StartColorTween(Color.white, true);
					break;
				case Selectable.Transition.SpriteSwap:
					this.DoSpriteSwap(null);
					break;
				case Selectable.Transition.Animation:
					this.TriggerAnimation(this.m_AnimationTriggers.normalTrigger);
					break;
			}
		}
		
		/// <summary>
		/// Internally evaluates and transitions to normal state.
		/// </summary>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void InternalEvaluateAndTransitionToNormalState(bool instant)
		{
			this.OnStateChange(UIButtonExtended.VisualState.Normal, instant);
		}
		
		/// <summary>
		/// Raises the state change event.
		/// </summary>
		/// <param name="state">State.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		protected virtual void OnStateChange(UIButtonExtended.VisualState state, bool instant)
		{
			// Check if the script is enabled
			if (!this.enabled || !this.gameObject.activeInHierarchy)
				return;
				
			Color color = this.m_Colors.normalColor;
			Sprite newSprite = null;
			string triggername = this.m_AnimationTriggers.normalTrigger;
			
			// Prepare the transition values
			switch (state)
			{
				case UIButtonExtended.VisualState.Normal:
					color = this.m_Colors.normalColor;
					newSprite = null;
					triggername = this.m_AnimationTriggers.normalTrigger;
					break;
				case UIButtonExtended.VisualState.Highlighted:
					color = this.m_Colors.highlightedColor;
					newSprite = this.m_SpriteState.highlightedSprite;
					triggername = this.m_AnimationTriggers.highlightedTrigger;
					break;
				case UIButtonExtended.VisualState.Pressed:
					color = this.m_Colors.pressedColor;
					newSprite = this.m_SpriteState.pressedSprite;
					triggername = this.m_AnimationTriggers.pressedTrigger;
					break;
				case UIButtonExtended.VisualState.Disabled:
					color = this.m_Colors.disabledColor;
					newSprite = this.m_SpriteState.disabledSprite;
					triggername = this.m_AnimationTriggers.disabledTrigger;
					break;
			}
			
			// Do the transition
			switch (this.m_Transition)
			{
				case Selectable.Transition.ColorTint:
					this.StartColorTween(color * this.m_Colors.colorMultiplier, instant);
					break;
				case Selectable.Transition.SpriteSwap:
					this.DoSpriteSwap(newSprite);
					break;
				case Selectable.Transition.Animation:
					this.TriggerAnimation(triggername);
					break;
			}
		}
		
		/// <summary>
		/// Starts the color tween.
		/// </summary>
		/// <param name="targetColor">Target color.</param>
		/// <param name="instant">If set to <c>true</c> instant.</param>
		private void StartColorTween(Color targetColor, bool instant)
		{
			if (this.m_TargetGraphic == null)
				return;
			
			this.m_TargetGraphic.CrossFadeColor(targetColor, (!instant) ? this.m_Colors.fadeDuration : 0f, true, true);
		}
		
		private void DoSpriteSwap(Sprite newSprite)
		{
			Image image = this.m_TargetGraphic as Image;
			
			if (image == null)
				return;
			
			image.overrideSprite = newSprite;
		}
		
		private void TriggerAnimation(string triggername)
		{
			if (this.targetGameObject == null)
				return;
			
			// Get the animator on the target game object
			Animator animator = this.targetGameObject.GetComponent<Animator>();
			
			if (animator == null || animator.runtimeAnimatorController == null || string.IsNullOrEmpty(triggername))
				return;
		
			animator.ResetTrigger(this.m_AnimationTriggers.normalTrigger);
			animator.ResetTrigger(this.m_AnimationTriggers.pressedTrigger);
			animator.ResetTrigger(this.m_AnimationTriggers.highlightedTrigger);
			animator.ResetTrigger(this.m_AnimationTriggers.disabledTrigger);
			animator.SetTrigger(triggername);
		}
	}
}	
