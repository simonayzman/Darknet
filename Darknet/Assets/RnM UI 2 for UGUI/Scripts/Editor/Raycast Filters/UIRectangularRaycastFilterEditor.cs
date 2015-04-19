using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

namespace UnityEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(UIRectangularRaycastFilter))]
	public class UIRectangularRaycastFilterEditor : Editor {
		
		public const string PREFS_KEY = "UIRectRaycastFilter_DG";
		private bool m_DisplayGeometry = true;
		
		protected void OnEnable()
		{
			this.m_DisplayGeometry = EditorPrefs.GetBool(PREFS_KEY, true);
		}
		
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			GUI.changed = false;
			
			this.m_DisplayGeometry = EditorGUILayout.Toggle("Display Geometry", this.m_DisplayGeometry);
			
			if (GUI.changed)
			{
				EditorPrefs.SetBool(PREFS_KEY, this.m_DisplayGeometry);
				EditorUtility.SetDirty(target);
			}
		}
		
		protected void OnSceneGUI()
		{
			if (!this.m_DisplayGeometry)
				return;
			
			UIRectangularRaycastFilter filter = this.target as UIRectangularRaycastFilter;
			Vector3[] worldCorners = filter.scaledWorldCorners;
			
			Handles.color = Color.green;
			Handles.DrawLine(worldCorners[0], worldCorners[1]); // Left line
			Handles.DrawLine(worldCorners[1], worldCorners[2]); // Top line
			Handles.DrawLine(worldCorners[2], worldCorners[3]); // Right line
			Handles.DrawLine(worldCorners[3], worldCorners[0]); // Bottom line
		}
	}
}