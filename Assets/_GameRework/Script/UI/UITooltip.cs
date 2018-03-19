using TMPro;
using UnityEngine;

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
