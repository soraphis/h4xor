using UnityEngine;

public class FollowCamera : MonoBehaviour {

    
    public Transform followObject;
    public float distance = 5;

    void OnValidate() {
        if(followObject)LateUpdate();
    }
    
    void LateUpdate () {
        this.transform.position = followObject.position - this.transform.forward * distance;
    }

}