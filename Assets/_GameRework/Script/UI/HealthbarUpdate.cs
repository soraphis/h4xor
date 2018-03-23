using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using _Game.ScriptRework;


public class HealthbarUpdate : MonoBehaviour {
    public Sprite[] HealthStates = new Sprite[0];
    public Image[] healthIndicators;

    private int cached_hp = 0;
    void Update() {
        var hp = PlayerActor.Instance.stats.currentStats.hp;

        if (hp == cached_hp) return;
        cached_hp = hp;

        StartCoroutine(UpdateHP(hp));
    }

    IEnumerator UpdateHP(int hp) {
        for (int i = 0; i < healthIndicators.Length; ++i){
            healthIndicators[i].sprite = HealthStates[i < hp ? 1 : 0];
            
            yield return new WaitForSeconds(0.2f);
        }
    }
    
}