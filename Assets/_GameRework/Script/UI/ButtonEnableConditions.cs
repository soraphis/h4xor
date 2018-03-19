using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEditor.Experimental.Build.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using _Game.ScriptRework;
using _Game.ScriptRework.ActionSelectors;

[System.Serializable]
public struct Condition {
	public string methodName;

	public bool Result(object obj) {
		if (string.IsNullOrEmpty(methodName)) return true;
		
		var name = methodName;
		bool inverted = false;
		if (name[0] == '!') {
			name = methodName.Substring(1);
			inverted = true;
		}
		
		var method = obj.GetType().GetMethod(name);
		if (method == null) {
			Debug.Log($"method {name} not found on {obj}", (GameObject)obj);
			return false;
		}

		return inverted ^ (bool) method.Invoke(obj, new object[0]);
	}
}

public class ButtonEnableConditions : MonoBehaviour {
	public Condition[] conditions = new Condition[0];
	private Button button;
	
	bool Evaluate() { return conditions.All(c => c.Result(this)); }
	
	public bool isPlayersTurn() { return PlayerActor.Instance.actionSelector.enabled; }
	public bool isPlayersOnTerminal() { return TerminalActor.currentActiveTerminal != null; }
	public bool isPlayerInAttackMode() { return PlayerActor.Instance.actionSelector.currentState.GetType() == typeof(SelectEnemyActionSelector); }
	public bool isPlayerInMoveMode() { return PlayerActor.Instance.actionSelector.currentState.GetType() == typeof(MoveActionSelectorState); }
	
	public bool isPermanentDisabled() { return false;  }
	
	
	void Start() { button = GetComponent<Button>(); }

	void Update() {
		button.interactable = Evaluate();
	}
	
	public void DoSwitchToMoveMode() {		PlayerActor.Instance.actionSelector.SwitchToMovementSelection();	}
	public void DoSwitchToAttackMode() {	PlayerActor.Instance.actionSelector.SwitchToAttackSelection();	}
	
}
