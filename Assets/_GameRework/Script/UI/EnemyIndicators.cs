using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using _Game.ScriptRework;
using _Game.ScriptRework.AI;
using UnityEngine.UI;

public class EnemyIndicators : MonoBehaviour {

	public Sprite[] AlertStates = new Sprite[0];
	public UnityEngine.GameObject prefab;

	private readonly Dictionary<EnemyActor, Image> indicators = new Dictionary<EnemyActor, Image>();

	[SerializeField] private TextMeshProUGUI stats;
	private Plane plane = new Plane(Vector3.up, Vector3.zero);

	// Update is called once per frame
	void Update () {
		var activeEnemies = GameTickManager.Instance.activeEnemies;
		
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float enter;
		if (plane.Raycast(ray, out enter)) {
			var field = new NVector2(GridUtil.WorldToGrid(ray.GetPoint(enter)));

			var b = false;
			foreach (var enemy in activeEnemies) {
				if(field != enemy.GridPosition) continue;
				var s = enemy.stats.currentStats;

				stats.text = 
$@"<color=white>HP{"\t"}<color=red>{new string('/', s.hp)}
<color=white>Dmg{"\t"}<color=red>{new string('/', s.atk)}
<color=white>SPD{"\t"}<color=red>{new string('/', s.speed)}
";
				
				((RectTransform)stats.transform.parent).position = Camera.main.WorldToScreenPoint(enemy.transform.position);
				((RectTransform) stats.transform.parent).position += Vector3.up * 50;
				b = true;
				break;
			}
			stats.transform.parent.gameObject.SetActive(b);
		}

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
