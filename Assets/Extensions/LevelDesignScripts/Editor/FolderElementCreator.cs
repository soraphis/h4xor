using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FolderElementCreator {

    [MenuItem("GameObject/FolderObject", false, 0)] 
    static void Create() {
        var go = new GameObject("Folder:");
        go.AddComponent<FolderElement>();
        go.isStatic = true;
    }

    static FolderElementCreator() {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect) {
        GameObject go = (GameObject)EditorUtility.InstanceIDToObject(instanceID);

        Rect rect = new Rect (selectionRect);
        rect.x += rect.width - 5;

        if (go != null && go.GetComponent<FolderElement>() != null) {
            var topline = new Rect(selectionRect);
            topline.height = 1;
            topline.width -= 5;
            EditorGUI.DrawRect(topline, Color.gray);

            rect.x -= 15;
            rect.width = 15;
            go.SetActive(GUI.Toggle(rect, go.activeInHierarchy, ""));
        }

    }

}