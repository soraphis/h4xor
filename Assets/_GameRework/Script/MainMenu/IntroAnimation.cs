using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroAnimation : MonoBehaviour {


	public TextMeshProUGUI textfield;
	
	public GameObject[] messages;
	public int[] jack_messages = new int[]{4, 5, 7};


	private bool wantSkip = false;
	void Awake() {
		foreach (var message in messages) {
			message.SetActive(false);
		}
	}
	
	IEnumerator Start () {
		var sceneload = SceneManager.LoadSceneAsync(2);
		sceneload.allowSceneActivation = false;
		
		yield return new WaitForSeconds(0.5f);

		for (int i = 0; i < messages.Length; ++i) {
			if (wantSkip) break;
			var text = messages[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
			if (jack_messages.Contains(i)) {
				if(! jack_messages.Contains(i-1)) yield return new WaitForSeconds(2f);
				
				for (int j = 0; j < text.text.Length; ++j) {
					textfield.text = text.text.Substring(0, j);
					yield return new WaitForSeconds(0.05f);
				}
				
			} else {
				yield return new WaitForSeconds(Mathf.Sqrt(text.text.Length) / 3f);
			}
			messages[i].SetActive(true);
			if (i == 2) { // a bit hacky, but its a scripted animation anyway...
				messages[++i].SetActive(true);
			}
			textfield.text = "";
		}

		while (sceneload.progress < 0.9f) { yield return null; }
		
		sceneload.allowSceneActivation = true;

	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			// escape -> rdy to skip!
			wantSkip = true;
		}
	}
	
	
}
