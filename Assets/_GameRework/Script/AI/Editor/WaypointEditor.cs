using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace _Game.ScriptRework.AI.Editor {
    
    [CustomEditor(typeof(PatrouilleBehaviour))]
    public class WaypointEditor : UnityEditor.Editor {
        
        private bool visualmode = false;
        private ReorderableList list;

        private void OnEnable() {
            list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("waypoints").FindPropertyRelative("points"),
                false, true, true, true);

            list.drawElementCallback = (rect, index, active, focused) => {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                EditorGUIUtility.labelWidth = 50;
                
                var line = new Rect(rect);
                line.height = EditorGUIUtility.singleLineHeight;
                var left = line.SplitRectH(2, 0);
                var cut = left.SnipRectH(-60-25, out left);
                cut = cut.SnipRectH(60);
                if (GUI.Button(cut.SplitRectH(2, 0), "+")) element.FindPropertyRelative("x").intValue++;
                if (GUI.Button(cut.SplitRectH(2, 1), "-")) element.FindPropertyRelative("x").intValue--;
                
                
                var right = line.SplitRectH(2, 1);
                right.SnipRectH(-25, out right);
                
                cut = right.SnipRectH(-30-25, out right);
                cut = cut.SnipRectH(60);
                if (GUI.Button(cut.SplitRectH(2, 0), "+")) element.FindPropertyRelative("y").intValue++;
                if (GUI.Button(cut.SplitRectH(2, 1), "-")) element.FindPropertyRelative("y").intValue--;

                EditorGUI.PropertyField(left, element.FindPropertyRelative("x"));
                EditorGUI.PropertyField(right, element.FindPropertyRelative("y"));
            };
            list.drawHeaderCallback = rect => {
                var right = rect.SnipRectH(-100, out rect);
                EditorGUI.LabelField(rect, "Waypoints");

                if (GUI.Button(right, "Reset", "toolbarbutton")) { list.serializedProperty.arraySize = 1; }
            };
        }
        
        public override void OnInspectorGUI() {
            serializedObject.Update();
            
            EditorGUILayout.Space();
            var line = EditorGUILayout.GetControlRect().SnipRectH(200);
            var _visualmode = EditorGUI.Toggle(line, "visual mode", visualmode, "MuteToggle");
            if (visualmode != _visualmode) {
                visualmode = _visualmode;
                SceneView.RepaintAll();
            }
            
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("type"));
            
            
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI() {
            var up_quat = Quaternion.Euler(90, 0, 0);
            
            var points = serializedObject.FindProperty("waypoints").FindPropertyRelative("points");
            Vector3 p1 = Vector3.zero, p2 = Vector3.zero;
            
            Handles.color = Color.red;
            for (int i = 0; i < points.arraySize; ++i) {
                var point = points.GetArrayElementAtIndex(i);
                var x = point.FindPropertyRelative("x").intValue;
                var y = point.FindPropertyRelative("y").intValue;

                p2 = GridUtil.GridToWorld(new Vector2(x, y));
                if (i != 0) {
                    Handles.DrawLine(p1, p2);
                }
                p1 = p2;
                Handles.RectangleHandleCap(i, p1, up_quat, 0.2f, EventType.Repaint);
            }
            
            // --------------
            if(!visualmode) return;
            
            var plane = new Plane(Vector3.up, Vector3.zero);

            float enter;
            var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            plane.Raycast(ray, out enter);
            var hitpoint = ray.GetPoint(enter);
            var tile = GridUtil.WorldToGrid(hitpoint);

            Handles.color = new Color(1f, 0.45f, 0f);
            Handles.DrawDottedLine(p1, GridUtil.GridToWorld(tile), 5);
            
            if (Handles.Button(GridUtil.GridToWorld(tile) + Vector3.up*0.1f, up_quat, 0.5f, 0.5f, Handles.RectangleHandleCap)) {
                //active.Tiles[tile_y*active.TileMapWidth + tile_x] = selectedTile;
                //active.OnValidate();
                var selected_point = new NVector2(tile);
                var pat = (target as PatrouilleBehaviour);
                if (pat.waypoints.points.Contains(selected_point)) {
                    pat.waypoints.points.Remove(selected_point);
                } else {
                    pat.waypoints.points.Add(selected_point);                    
                }
                SceneView.RepaintAll();
                EditorUtility.SetDirty(target);
                
            }
            
            
        }
    }
}