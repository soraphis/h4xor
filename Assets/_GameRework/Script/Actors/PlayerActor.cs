using System.Threading.Tasks;
using UnityEngine;
using _Game.ScriptRework.ActionSelectors;

namespace _Game.ScriptRework {

    [RequireComponent(typeof(ActionSelectorSM))]
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(Attributes))]
    public class PlayerActor : Singleton<PlayerActor>, ITickBehaviour, ITakeDamageHandler {

        public bool clipToGrid = false;
        
        public ActionSelectorSM actionSelector;
        public MovementController movementController;
        public Attributes stats;

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
            actionSelector = GetComponent<ActionSelectorSM>();
            movementController = GetComponent<MovementController>();
            stats = GetComponent<Attributes>();
        }

        private void ComponentsEnabled(bool enabled) {
            movementController.ResetDesiredPosition();
            movementController.enabled = enabled;
            // add components here ... 
        }
        
        public async Task TickAwaitable() {
            ComponentsEnabled(true);
            
            actionSelector.enabled = true;
            var action = await actionSelector.OnActionSelectedObservable;
            actionSelector.enabled = false;
            
            // do action
            await action.Execute();

            ComponentsEnabled(false);
        }


        public void TakeDamage(int value) {
            this.stats.currentStats.hp -= value;
            if (this.stats.currentStats.hp <= 0) {
                
            }
        }
    }
}