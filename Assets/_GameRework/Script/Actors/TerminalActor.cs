using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using _Game.ScriptRework;

[RequireComponent(typeof(Animator))]
public class TerminalActor : MonoBehaviour {

	[Header("Hacking Config")] 
	public string code; 
	public int turns; 
	
	[Header("Other Stuff")]
	[SerializeField] private Transform terminalField;
	private Animator animator;
	
	public static TerminalActor currentActiveTerminal;
	
	// public event Action OnDisable;
	public UnityEvent OnTerminalDisabled;

	void OnValidate() {
		terminalField = terminalField ?? transform.Find("TerminalField"); 
		
		code = code ?? "";
		if(code.Length > 0)
			code = code.ToCharArray().Where(c => c > '0' && c <= '6').Select(c => c.ToString()).Aggregate((a, b) => a + b);
	}

	void Awake() {
		// room entered:
		animator = GetComponent<Animator>();
	}

	void Start() {
		PlayerActor.Instance.movementController.OnTileEnterAsObservable.Subscribe(OnEnterField);		
	}

	public void TerminalHacked() {
		// if (OnDisable != null) OnDisable.Invoke();
		OnTerminalDisabled.Invoke();
		this.enabled = false;
		if (currentActiveTerminal == this) currentActiveTerminal = null;
	}

	public void TerminalWireShutdown(LineRenderer ren) {
		ren.materials = new Material[]{ ren.materials[0] };
	}
	
	void OnEnterField(bool step){
		if(! step) return;
		if(! this.enabled) return;
		
		if (PlayerActor.Instance.GridPosition != GridUtil.WorldToGrid(terminalField.position)) {
			animator.SetBool("Active", false);

			if (currentActiveTerminal == this) currentActiveTerminal = null;
			return;
		}
		
		animator.SetBool("Active", true);
		currentActiveTerminal = this;

	}

}
