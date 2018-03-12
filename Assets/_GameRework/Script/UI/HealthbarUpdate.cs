using System;
using UnityEngine;
using _Game.ScriptRework;


public class HealthbarUpdate : MonoBehaviour {

    public GameObject[] healthIndicators;

    void Update() {
        var hp = PlayerActor.Instance.stats.currentStats.hp;

        for (int i = 0; i < healthIndicators.Length; ++i){
            healthIndicators[i].SetActive(i < hp);
            
        }

    }

}