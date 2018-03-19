using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Game.ScriptRework;
using _Game.ScriptRework.AI;
using UnityEngine.UI;

public class EnemyIndicators : MonoBehaviour {

	public Sprite[] AlertStates = new Sprite[0];
	public UnityEngine.GameObject prefab;

	private readonly Dictionary<EnemyActor, Image> indicators = new Dictionary<EnemyActor, Image>();

	// Update is called once per frame
	void Update () {
		var activeEnemies = GameTickManager.Instance.activeEnemies;

		foreach (var key in indicators.Keys.Where(k => !activeEnemies.Contains(k))) {
			indicators[key].gameObject.SetActive(false);
		}

		foreach (var enemy in activeEnemies) {
			if (!indicators.ContainsKey(enemy)) {
				var go = GameObject.Instantiate(prefab, parent: this.transform);
				indicators.Add(enemy, go.GetComponent<Image>());
			} 
				


			if (enemy.State == AIState.Alterted) {
				int i = enemy.visibleArea.IsPlayerVisible() ? 2 : 1;
				indicators[enemy].gameObject.SetActive(true);
				indicators[enemy].sprite = AlertStates[i];
			} else {
				indicators[enemy].gameObject.SetActive(false);
				
			}
			
			var sp = Camera.main.WorldToScreenPoint(enemy.transform.position + Vector3.up*2.5f);
			indicators[enemy].transform.position = sp;
		}
		
	}
}
