using UnityEngine;

public class TransformStatic : MonoBehaviour {
	private Vector3 position, rotation;

	public bool keepWorldPosition, keepWorldRotation;
	
	// Use this for initialization
	void Awake() {
		position = transform.localToWorldMatrix * transform.localPosition;
		rotation = transform.localToWorldMatrix * transform.localEulerAngles;
		// scale = transform.localToWorldMatrix * transform.localScale;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (keepWorldPosition) { transform.position = position; }
		if (keepWorldRotation) { transform.rotation = Quaternion.Euler(rotation); }
		//if (keepWorldScale) { transform.scale = Quaternion.Euler(rotation); }
	}
}
