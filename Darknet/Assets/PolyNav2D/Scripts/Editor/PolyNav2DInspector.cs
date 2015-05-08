using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PolyNav2D))]
public class PolyNav2DInspector : Editor {

	private PolyNav2D polyNav{
		get {return target as PolyNav2D;}
	}

	public override void OnInspectorGUI(){

		polyNav.generateOnUpdate = EditorGUILayout.Toggle("Regenerate On Change", polyNav.generateOnUpdate);
		polyNav.inflateRadius = EditorGUILayout.FloatField("Radius", polyNav.inflateRadius);

		GUI.backgroundColor = new Color(0.7f, 0.7f, 1);
		if (GUILayout.Button("Add New Polygon Obstacle")){

			PolyNavObstacle newPoly= new GameObject("NavObstacle").AddComponent(typeof(PolyNavObstacle)) as PolyNavObstacle;
			newPoly.transform.parent = polyNav.transform;
			newPoly.transform.localPosition = new Vector3(0, 0, -1);
		}

		GUI.backgroundColor = Color.white;

		foreach (PolyNavObstacle c in polyNav.navObstacles.ToArray()){

			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Select"))
				Selection.activeObject = c.gameObject;

			GUI.backgroundColor = new Color(1, 0.7f, 0.7f);
			if (GUILayout.Button("Remove"))
				DestroyImmediate(c.gameObject);
			GUI.backgroundColor = Color.white;

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndVertical();
		}

		EditorGUILayout.LabelField("Nodes Count", polyNav.nodesCount.ToString());

		if (GUI.changed)
			EditorUtility.SetDirty(polyNav);
	}
}
