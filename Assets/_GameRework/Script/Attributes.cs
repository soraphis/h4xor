using UnityEngine;

namespace _Game.ScriptRework {
	public class Attributes : MonoBehaviour {

		public AttributeSet attributes;
		public Stats currentStats;

		[Range(0, 0.99f)] public float viewAngle; 
		
		private void Awake() {
			currentStats = attributes.attributes;
			
		}
	}
}
