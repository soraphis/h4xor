using UnityEngine.EventSystems;

namespace _Game.ScriptRework {
    public interface ITakeDamageHandler : IEventSystemHandler {
        void TakeDamage(int value);
    }
}