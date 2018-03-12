using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITooltip : MonoBehaviour {
	
	[SerializeField] private TextMeshProUGUI text;



	void Awake() {
		Hide();
	}
	
	public void Show(string content) {
		this.text.text = content;
		gameObject.SetActive(true);
	}

	public void Hide() { gameObject.SetActive(false); }
	
}
