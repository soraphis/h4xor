using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewallActor : MonoBehaviour {


	[SerializeField] private bool isActive = true;
	
	[SerializeField] private GameObject effect;

	void Start() {
		IsActive = isActive; // initializes the effect gameObject (see setter)
		
	}
	

	public bool IsActive { get { return isActive; }
		set {
			isActive = value; 
			if(effect != null) effect.SetActive(isActive);
		} 
	}
}
