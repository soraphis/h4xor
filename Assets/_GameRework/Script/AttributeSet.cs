using UnityEngine;

namespace _Game.ScriptRework {
    [CreateAssetMenu()]
    [System.Serializable]
    public class AttributeSet : ScriptableObject {
        public Stats attributes;
    }
}