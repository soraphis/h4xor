using UnityEditor;
using UnityEngine;
using _Game.ScriptRework;

[CustomEditor(typeof(Attributes))]
public class AttributeEditor : Editor {

	
	// helper function:
	private static void PrintProperties(SerializedProperty property) {
		SerializedProperty it = property.Copy ();
		EditorGUILayout.LabelField(it.type + " - " + it.name);
		while (it.NextVisible(true)) {
			// Debug.Log ();
			EditorGUILayout.LabelField(it.displayName + " : " + it.type);
			
		}
	}

	private static void DrawLine(Rect line, SerializedProperty orig, SerializedProperty alt = null) {
		var b = alt == null;

		var label_rect = line.SplitRectH(b ? 2 : 3, 0);
		var value1_rect = line.SplitRectH(b ? 2 : 3, 1);

		EditorGUI.LabelField(label_rect, orig.displayName);
		EditorGUI.PropertyField(value1_rect, orig, GUIContent.none);
		
		if (!b) {
			var value2_rect = line.SplitRectH(b ? 2 : 3, 2);
			EditorGUI.PropertyField(value2_rect, alt, GUIContent.none);
		}
	}
	
	private static void DrawHeadLine(Rect line, string orig, string alt = null) {
		var b = alt == null;
		var value1_rect = line.SplitRectH(b ? 2 : 3, 1);

		EditorGUI.LabelField(value1_rect, orig);
		
		if (!b) {
			var value2_rect = line.SplitRectH(b ? 2 : 3, 2);
			EditorGUI.LabelField(value2_rect, alt);
		}
	}
	
	public override void OnInspectorGUI() {
		Attributes attribs = (Attributes) target;

		EditorGUILayout.PropertyField(serializedObject.FindProperty("attributes"));

		if (attribs.attributes != null) {
			
			var obj = serializedObject.FindProperty("attributes").objectReferenceValue;
			var attribset = new SerializedObject(obj);
			var attribset_vars = attribset.FindProperty("attributes");

			var currentstats = serializedObject.FindProperty("currentStats");

			EditorGUILayout.Space();
			var line = EditorGUILayout.GetControlRect();
			DrawHeadLine(line, "Original Stats" + (Application.isPlaying ? "¹" : ""), Application.isPlaying ? "Current Stats" : null);
			
			foreach (var stat in new string[]{"atk","hp","def","awareness","speed"}) {
				line = EditorGUILayout.GetControlRect();
				DrawLine(line, attribset_vars.FindPropertyRelative(stat), Application.isPlaying ? currentstats.FindPropertyRelative(stat) : null);						
			}
			attribset.ApplyModifiedProperties();
			
			if(Application.isPlaying) 
				EditorGUILayout.HelpBox("¹: Changes persist after exiting Playmode", MessageType.Info);
		}

		EditorGUILayout.Space();
		
		EditorGUILayout.PropertyField(serializedObject.FindProperty("viewAngle"));
		
		serializedObject.ApplyModifiedProperties();
	}
	
	
	
}
