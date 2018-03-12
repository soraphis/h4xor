using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeToMenu : MonoBehaviour {

	private bool animEnded = false;
	
	public void OnAnimationEnd() { animEnded = true; }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Escape) || animEnded)
			SceneManager.LoadScene(0);
	}
}
