using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PPEScanlines : MonoBehaviour {

	public Material scanlineMaterial;

	private void OnValidate() {
		// find material?
	}

	private void Start() {
		if (scanlineMaterial != null) {
			var px = this.GetComponent<Camera>().pixelHeight;
			scanlineMaterial.SetFloat("_Scale", px);
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest) {
		Graphics.Blit(src, dest, scanlineMaterial);
	}
}
