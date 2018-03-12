using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;
using _Game.ScriptRework;

public class BackgroundParallax : MonoBehaviour {

	private MeshRenderer meshRenderer;
	private Material material;

	public float parallax_effect = -0.05f;
	
	// Use this for initialization
	void Start() {
		meshRenderer = this.GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		material = meshRenderer.material;
		material.mainTextureOffset = this.transform.position.To2DXZ() * parallax_effect;
		meshRenderer.material = material;
	}
}
