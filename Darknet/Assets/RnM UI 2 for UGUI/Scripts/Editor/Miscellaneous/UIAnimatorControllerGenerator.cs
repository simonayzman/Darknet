using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;

namespace UnityEditor.UI
{
	public class UIAnimatorControllerGenerator
	{
		/// <summary>
		/// Generate an the animator contoller.
		/// </summary>
		/// <returns>The animator contoller.</returns>
		/// <param name="triggersProperty">Triggers property.</param>
		/// <param name="preferredName">Preferred name.</param>
		public static AnimatorController GenerateAnimatorContoller(SerializedProperty triggersProperty, string preferredName)
		{
			// Prepare the triggers list
			List<string> triggersList = new List<string>();
			
			SerializedProperty serializedProperty = triggersProperty.Copy();
			SerializedProperty endProperty = serializedProperty.GetEndProperty();
			
			while (serializedProperty.NextVisible(true) && !SerializedProperty.EqualContents(serializedProperty, endProperty))
			{
				triggersList.Add(!string.IsNullOrEmpty(serializedProperty.stringValue) ? serializedProperty.stringValue : serializedProperty.name);
			}
			
			// Generate the animator controller
			return UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggersList, preferredName);
		}
		
		/// <summary>
		/// Generates an animator contoller.
		/// </summary>
		/// <returns>The animator contoller.</returns>
		/// <param name="animationTriggers">Animation triggers.</param>
		/// <param name="target">Target.</param>
		public static AnimatorController GenerateAnimatorContoller(List<string> animationTriggers, string preferredName)
		{
			if (string.IsNullOrEmpty(preferredName))
				preferredName = "New Animator Controller";
			
			string saveControllerPath = UIAnimatorControllerGenerator.GetSaveControllerPath(preferredName);
			
			if (string.IsNullOrEmpty(saveControllerPath))
				return null;
			
			AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(saveControllerPath);
			
			foreach (string trigger in animationTriggers)
			{
				UIAnimatorControllerGenerator.GenerateTriggerableTransition(trigger, animatorController);
			}
			
			return animatorController;
		}
		
		private static string GetSaveControllerPath(string name)
		{
			string message = string.Format("Create a new animator controller with name '{0}':", name);
			return EditorUtility.SaveFilePanelInProject("New Animator Contoller", name, "controller", message);
		}
		
		private static AnimationClip GenerateTriggerableTransition(string name, AnimatorController controller)
		{
			AnimationClip animationClip = AnimatorController.AllocateAnimatorClip(name);
			AssetDatabase.AddObjectToAsset(animationClip, controller);
			State dst = AnimatorController.AddAnimationClipToController(controller, animationClip);
			controller.AddParameter(name, AnimatorControllerParameterType.Trigger);
			StateMachine stateMachine = controller.GetLayer(0).stateMachine;
			Transition transition = stateMachine.AddAnyStateTransition(dst);
			AnimatorCondition condition = transition.GetCondition(0);
			condition.mode = TransitionConditionMode.If;
			condition.parameter = name;
			
			return animationClip;
		}
	}
}