using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {


	public void ClickGameStart() {
		SceneManager.LoadScene(1);
	}
	
	public void ClickCredits() {
		SceneManager.LoadScene(4);
	}
	
}
