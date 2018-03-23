using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeToMenu : MonoBehaviour {

	[SerializeField] private float endYpos = 1800;
	[SerializeField] private RectTransform pane;
	
	// Update is called once per frame
	void Update () {
		pane.position = Vector3.MoveTowards(pane.position, new Vector3(pane.position.x, endYpos, pane.position.z), 30 * Time.deltaTime);
		
		if(Input.GetKey(KeyCode.Escape) || pane.localPosition.y > endYpos)
			SceneManager.LoadScene(0);
	}
}
