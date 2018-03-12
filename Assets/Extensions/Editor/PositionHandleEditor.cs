using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
public class PositionHandleEditor : UnityEditor.Editor{

    void OnSceneGUI()
    {
        var t = target as MonoBehaviour;
        if (t == null) return;

        foreach (var fieldInfo in t.GetType().GetFields())
        {
            try
            {
                var attribs = fieldInfo.GetCustomAttributes(typeof(PositionHandleAttribute), false);
                if (attribs.Length > 0 && fieldInfo.FieldType == typeof(Vector3))
                {
                    Vector3 v = (Vector3)fieldInfo.GetValue(t);
                    v = Handles.PositionHandle((Vector3)v, Quaternion.identity);
                    Handles.Label(v, fieldInfo.Name);
                    fieldInfo.SetValue(t, v);
                }
            }
            catch (System.Exception)
            {
                // ignored
            }
        }
    }
}
