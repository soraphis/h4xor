using UnityEngine;

[ExecuteInEditMode]
public class FolderElement : MonoBehaviour {

    void OnValidate() {
        Awake();
    }

    void Awake() {
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;

        this.hideFlags = HideFlags.DontSaveInBuild;
        this.transform.hideFlags = HideFlags.NotEditable;
    }

    #if UNITY_EDITOR
    void Update() {
        if(this.transform.hasChanged) Awake();
    }
    #endif
}
