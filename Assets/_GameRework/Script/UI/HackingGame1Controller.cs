using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using ProBuilder2.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Game.ScriptRework;

public class HackingGame1Controller : MonoBehaviour {


	public string code;
	private int[] currentCode;

	public int turns;
	
	[SerializeField] private Slider progressbar;
	
	[SerializeField] private RectTransform mainPanel;
	[SerializeField] private TextMeshProUGUI turnsLeftText;
	[SerializeField] private Image[] images;
	[SerializeField] private Button[] buttons;
	[SerializeField] private bool[] forwardButton;
	
	[SerializeField] private Color[] barColors;

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
			turns = TerminalActor.currentActiveTerminal.turns;
			turnsLeftText.text = $"{turns} Turns Left";
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
		
		var tr = this.transform as RectTransform;
		tr.localScale = Vector3.one;
		progressbar.colors = new ColorBlock(){colorMultiplier = 1, disabledColor = barColors[0]};
		
		UpdateImageRotations();

		PlayerActor.Instance.actionSelector.enabled = false;

		tr.DOScale(Vector3.zero, 0.1f).From();
		tr.DOAnchorPos(Vector3.zero, 0.1f).From();
	}

	public void OnDisable() {
		Debug.Log("disabled!");
		PlayerActor.Instance.actionSelector.enabled = true;
		PlayerActor.Instance.actionSelector.UIActionDone(new IdleAction());
	}

	private void UpdateImageRotations() {
		for (int i = 0; i < 9; ++i) {
			images[i].rectTransform.eulerAngles = new Vector3(0, 0, 90 * currentCode[i]);
		}
	}

	public IEnumerator Sucess() {
		var tr = this.transform as RectTransform;
		yield return null;
		
		progressbar.colors = new ColorBlock(){colorMultiplier = 1, disabledColor = barColors[2]};
		yield return new WaitForSeconds(0.8f);

		TerminalActor.currentActiveTerminal.TerminalHacked();

		var pos = tr.anchoredPosition3D;
		yield return tr.DOScale(Vector3.zero, 0.1f);
		yield return tr.DOAnchorPos(Vector3.zero, 0.1f);
		
		this.gameObject.SetActive(false);
		tr.anchoredPosition3D = pos;
	}

	public IEnumerator Failed() {

		turnsLeftText.text = $"{turns} Turns Left";
		yield return null;
		progressbar.colors = new ColorBlock(){colorMultiplier = 1, disabledColor = barColors[1]};
		
		yield return mainPanel.DOShakeAnchorPos(0.4f, new Vector2(30, 0));
		yield return new WaitForSeconds(0.8f);
		this.gameObject.SetActive(false);
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
		
		if (!inverse) {
			if (--turns <= 0) { StartCoroutine(Failed()); }
			else {
				UpdateImageRotations();
				turnsLeftText.text = $"{turns} Turns Left";
			}
			
		}

		if (currentCode.All(c => c == 0)) {
			StartCoroutine(Sucess());
		}
	}
	

}
