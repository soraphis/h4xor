using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Gamelogic.Extensions;
using Gamelogic.Extensions.Algorithms;
using UnityEngine;
using _Game.ScriptRework.AI;

namespace _Game.ScriptRework {
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(Attributes))]
    public class EnemyActor : MonoBehaviour, ITickBehaviour {
        public bool clipToGrid = false;
        
        private MovementController movementController;
        [HideInInspector] public Attributes stats;
        [HideInInspector] public VisibleAreaComponent visibleArea;
        
        public GameObject attackPrefab;
        private AIActionSelector actionSelector;
        
        public NVector2 GridPosition {
            get { return new NVector2(GridUtil.WorldToGrid(this.transform.position)); }
        }
        
        private void OnValidate() {
            if (clipToGrid && Application.isEditor && !Application.isPlaying){
                transform.position = GridUtil.GridToWorld(GridUtil.WorldToGrid(transform.position));
                clipToGrid = false;
            }
        }
        
        private void Awake() {
            movementController = GetComponent<MovementController>();
            stats = GetComponent<Attributes>();
            visibleArea = GetComponent<VisibleAreaComponent>();
            actionSelector = new AIActionSelector(this);
        }
        
        private void ComponentsEnabled(bool enabled) {
            movementController.ResetDesiredPosition();
            movementController.enabled = enabled;
            // add components here ... 
        }

        private void OnEnable() {
            GameTickManager.Instance.activeEnemies.Add(this);
        }

        private void OnDisable() {
            if (GameTickManager.Instance == null) return; // happens if exiting playmode in editor... 
            GameTickManager.Instance.activeEnemies.Remove(this);
        }
        
        public async Task TickAwaitable() {
            ComponentsEnabled(true);
            
            var action = actionSelector.Evaluate();
            await action.Execute();

            ComponentsEnabled(false);
        }

        public void DoAlert() {
            this.actionSelector.state = AIState.Alterted;
        }
    }
}