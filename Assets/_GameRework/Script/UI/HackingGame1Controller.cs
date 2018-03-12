using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProBuilder2.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Game.ScriptRework;

public class HackingGame1Controller : MonoBehaviour {


	public string code;
	private int[] currentCode;

	public float TimeLimit;
	
	[SerializeField] private Slider progressbar;
	
	[SerializeField] private Image[] images;
	[SerializeField] private Button[] buttons;
	[SerializeField] private bool[] forwardButton;

	private Coroutine activeTimer = null;

	public void OnValidate() {
		images = images ?? new Image[9];
		forwardButton = forwardButton ?? new bool[6];
		buttons = buttons ?? new Button[6];

		/*
		code = code ?? new int[9];
		if (code.Length != 9) {
			code = code.AddRange(new int[9 - code.Length]);
		}
		
		for (int i = 0; i < code.Length; ++i) { code[i] = (4 + code[i]) % 4; }
		*/

		code = code ?? "";
		if(code.Length > 0)
			code = code.ToCharArray().Where(c => c > '0' && c <= '6').Select(c => c.ToString()).Aggregate((a, b) => a + b);
	}

	public void OnEnable() {
		if (TerminalActor.currentActiveTerminal != null) {
			code = TerminalActor.currentActiveTerminal.code;
			TimeLimit = TerminalActor.currentActiveTerminal.time;			
		}
		
		OnValidate();
		// "" ""
		
		currentCode = new int[9];
		
		for (int i = 0; i < 6; ++i) {
			buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = forwardButton[i] ? "" : "";
			buttons[i].onClick.RemoveAllListeners();

			int j = i;
			buttons[i].onClick.AddListener(() => PressButton(j));
		}

		// currentCode = code;
		var char_code = code.ToCharArray();
		for(int i = char_code.Length - 1; i >= 0; --i){
			PressButton(char_code[i]-'1', true);
		}
		
		UpdateImageRotations();
		if(activeTimer != null) StopCoroutine(activeTimer);
		activeTimer = StartCoroutine(CountDown(TimeLimit));
		
		PlayerActor.Instance.actionSelector.gameObject.SetActive(false);
	}

	public void OnDisable() {
		Debug.Log("disabled!");
		if(activeTimer != null) StopCoroutine(activeTimer);
		activeTimer = null;
		
		PlayerActor.Instance.actionSelector.gameObject.SetActive(true);
		PlayerActor.Instance.actionSelector.UIActionDone(new IdleAction());
	}

	private IEnumerator CountDown(float time) {
		var _t = time;
		yield return null;
		while (_t > 0) {
			_t -= Time.deltaTime;
			progressbar.value = _t / time;
			
			yield return null;
		}
		
		Debug.Log("Time is up!");
		this.gameObject.SetActive(false);
	}

	private void UpdateImageRotations() {
		for (int i = 0; i < 9; ++i) {
			images[i].rectTransform.eulerAngles = new Vector3(0, 0, 90 * currentCode[i]);
		}
	}
	
	public void PressButton(int index, bool inverse = false) {
		if(index < 0 || index > 5) return;

		var d = forwardButton[index] ? -1 : 1;
		if (inverse) d *= -1;
			
		if (index < 3) {
			// columns:
			currentCode[index] = (4 + currentCode[index] + d) % 4;
			currentCode[index+3] = (4 + currentCode[index+3] + d) % 4;
			currentCode[index+6] = (4 + currentCode[index+6] + d) % 4;
		} else {
			// rows:
			currentCode[(index-3)*3] = (4 + currentCode[(index-3)*3] + d) % 4;
			currentCode[(index-3)*3+1] = (4 + currentCode[(index-3)*3+1] + d) % 4;
			currentCode[(index-3)*3+2] = (4 + currentCode[(index-3)*3+2] + d) % 4;
		}

		if(!inverse) UpdateImageRotations();

		if (currentCode.All(c => c == 0)) {
			Debug.Log("OPEN!");
			TerminalActor.currentActiveTerminal.TerminalHacked();
			this.gameObject.SetActive(false);
		}
	}
	

}
