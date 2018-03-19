using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestDrawMesh : MonoBehaviour {
	
	public Mesh mesh;
	public Material material;
	private MaterialPropertyBlock activeCircle;
	
	void Start () {
		activeCircle = new MaterialPropertyBlock();
		activeCircle.SetColor("_TintColor", Color.red);

	}        
	
	// Update is called once per frame
	void Update () {
		Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0, Camera.current, 0);
	}
}
