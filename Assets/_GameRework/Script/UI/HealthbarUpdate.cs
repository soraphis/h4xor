using System;
using UnityEngine;
using UnityEngine.UI;
using _Game.ScriptRework;


public class HealthbarUpdate : MonoBehaviour {
    public Sprite[] HealthStates = new Sprite[0];
    public Image[] healthIndicators;

    void Update() {
        var hp = PlayerActor.Instance.stats.currentStats.hp;

        for (int i = 0; i < healthIndicators.Length; ++i){
            healthIndicators[i].sprite = HealthStates[i < hp ? 1 : 0];
        }

    }

}