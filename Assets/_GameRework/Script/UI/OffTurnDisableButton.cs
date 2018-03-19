using UnityEngine;
using UnityEngine.UI;
using _Game.ScriptRework;


/// <summary>
///  script that disables a button if player is not on turn
/// </summary>
[RequireComponent(typeof(Button))]
public class OffTurnDisableButton : MonoBehaviour {

	private Button button;
	
	// Use this for initialization
	void OnEnable() {
		button = GetComponent<Button>();

		PlayerActor.Instance.actionSelector.OnEnableSelector += OnTurn;
		PlayerActor.Instance.actionSelector.OnDisableSelector += OffTurn;
	}

	private void OnTurn() { if (isApplicationQuitting) return; button.interactable = true; }
	private void OffTurn() { if (isApplicationQuitting) return; button.interactable = false; }
	
	
	private bool isApplicationQuitting = false;
	void OnDisable() {
		if (isApplicationQuitting) return;
		PlayerActor.Instance.actionSelector.OnEnableSelector -= OnTurn;
		PlayerActor.Instance.actionSelector.OnDisableSelector -= OffTurn;
	}
	void OnApplicationQuit () {
		isApplicationQuitting = true;
	}
	
}
