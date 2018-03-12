using UnityEditor;
using UnityEngine;

public class LineRendererEditor {

    [InitializeOnLoadMethod]
    private static void InitializeAnalyzer() {

        active = false;
        Selection.selectionChanged += SelectionChanged;
        SelectionChanged();
    }

    private static bool active;
    private static LineRenderer lineRenderer;

    private static void SelectionChanged() {
        var ob = Selection.activeGameObject;
        if (ob == null) {
            goto INACTIVE_ENDING;
        }
        
        lineRenderer = ob.GetComponent<LineRenderer>();

        if (lineRenderer != null) {
            if (!active) {
                active = true;
                SceneView.onSceneGUIDelegate += OnUpdate;
            }
            return;
        }
        
    INACTIVE_ENDING:
        if(active){
            active = false;
            SceneView.onSceneGUIDelegate -= OnUpdate;
        }
        return;
        
    }

    private static void staticRotationHandleCap(int controlID, Vector3 position, Quaternion rotation, float size, EventType eventType) {
        Handles.RectangleHandleCap(controlID, position, Quaternion.LookRotation(Vector3.up), size, eventType);
    }
    
    private static void OnUpdate(SceneView view) {
        var m = Handles.matrix;
        
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        int n = lineRenderer.GetPositions(positions);

        var pos = lineRenderer.transform.position;
        //Matrix4x4 t = lineRenderer.useWorldSpace ? Matrix4x4.identity : lineRenderer.transform.localToWorldMatrix; 
        //Matrix4x4 t_ = lineRenderer.useWorldSpace ? Matrix4x4.identity : lineRenderer.transform.worldToLocalMatrix;

        if (! lineRenderer.useWorldSpace) Handles.matrix = lineRenderer.transform.localToWorldMatrix;
        
        for (int i = 0; i < n; ++i) {
            
            //var v = Handles.FreeMoveHandle(positions[i], Quaternion.identity, 0.25f, Vector3.one, staticRotationHandleCap);
            var v = Handles.PositionHandle(positions[i], Quaternion.LookRotation(Vector3.up));

            positions[i] = v;
        }
        
        lineRenderer.SetPositions(positions);

        Handles.matrix = m;

    }
    

}
