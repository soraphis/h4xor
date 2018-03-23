using System;
using System.Collections;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using _Game.ScriptRework.ActionSelectors;
using _Game.ScriptRework.AI;

namespace _Game.ScriptRework {

    [RequireComponent(typeof(ActionSelectorSM))]
    [RequireComponent(typeof(MovementController))]
    [RequireComponent(typeof(Attributes))]
    public class PlayerActor : Singleton<PlayerActor>, ITickBehaviour, ITakeDamageHandler {

        public bool clipToGrid = false;
        
        public ActionSelectorSM actionSelector;
        public MovementController movementController;
        public Attributes stats;

        public bool HasLearnedAttack { get; set; }

        [SerializeField] private AudioClip[] hitSounds = new AudioClip[0];

        private readonly Subject<CharacterAction> onActionSelectedObservable = new Subject<CharacterAction>();
        
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
            actionSelector.onActionSelectedObservable = onActionSelectedObservable;
            movementController = GetComponent<MovementController>();
            stats = GetComponent<Attributes>();
        }

        private void ComponentsEnabled(bool enabled) {
            movementController.ResetDesiredPosition();
            movementController.enabled = enabled;
            // add components here ... 
        }

        private IEnumerator foo() {
            yield return new WaitForEndOfFrame();
        }
        
        public async Task TickAwaitable() {
            ComponentsEnabled(true);
            
            actionSelector.enabled = true;
            var action = await onActionSelectedObservable.AsObservable();
            actionSelector.enabled = false;

            await Observable.FromCoroutine(foo);
            
            // do action
            await action.Execute();

            ComponentsEnabled(false);
        }


        public async void TakeDamage(int value) {
            AudioSource.PlayClipAtPoint(hitSounds[UnityEngine.Random.Range(0, hitSounds.Length)], this.transform.position);
            this.stats.currentStats.hp -= value;
            
            if (this.stats.currentStats.hp <= 0) {
                await Respawn();
            }
        }

        private async Task Respawn() {
            this.gameObject.SetActive(false);
            await CameraFadeScript.FadeOut(0.1f).ToObservable();
            this.stats.currentStats.hp = this.stats.attributes.attributes.hp;
            this.transform.position = GridUtil.GridToWorld(GameTickManager.Instance.ActiveRoom.playerEnterTile);
            this.movementController.ResetDesiredPosition();
            
            foreach (var activeEnemy in GameTickManager.Instance.activeEnemies) {
                activeEnemy.DoCalm();
            }
            
            this.gameObject.SetActive(true);
            await CameraFadeScript.FadeIn(1.2f).ToObservable();
        }
        
    }
}