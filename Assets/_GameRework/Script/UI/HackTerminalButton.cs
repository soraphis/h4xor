using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _Game.ScriptRework;


[RequireComponent(typeof(Button))]
public class HackTerminalButton : MonoBehaviour{
	
	private Button button;
	//todo: reference hacking window
	
	
	// Use this for initialization
	void Start() {
		button = GetComponent<Button>();
		
	}
	
	// Update is called once per frame
	void Update() {
		button.interactable &= TerminalActor.currentActiveTerminal != null;
		
	}

	
	
	public void StartHacking() {
		Debug.Log("DO HACKING STUFF!");
		
		// PlayerActor.Instance.actionSelector.
	}


}
