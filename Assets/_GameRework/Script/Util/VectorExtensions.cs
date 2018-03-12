using UnityEngine;

namespace _Game.ScriptRework.Util {
    public static class VectorMathExtension {
        public static bool Approx(this Vector2 v, Vector2 other, float off) { return (other - v).sqrMagnitude - off * off <= 0; }
        public static bool Approx(this Vector3 v, Vector3 other, float off) { return (other - v).sqrMagnitude - off * off <= 0; }
    }
}