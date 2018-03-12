namespace _Game.ScriptRework {
    public interface IActionSelector {
        UniRx.IObservable<CharacterAction> OnActionSelectedObservable { get; }
    }
}