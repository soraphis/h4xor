using UnityEngine;
using _Game.ScriptRework;

public class HideUIWithoutPlayer : MonoBehaviour {
    private Canvas canvas;
    void Start() { canvas = GetComponent<Canvas>(); }
    
    void Update() {
        canvas.enabled =PlayerActor.Instance.gameObject.activeSelf; 
    }
    
}
