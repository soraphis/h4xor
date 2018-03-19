using System;
using UniRx;
using UnityEngine;

namespace _Game.ScriptRework.ActionSelectors {
    
    public class ActionSelectorSM : MonoBehaviour, IActionSelector{

        /**    INSPECTOR VARIABLES    */
        public Material walk_material;
        public Material attack_material;
        public Mesh selectionCircleMesh;
        public Material selectionCircleMaterial;
        
        public GameObject fieldSelectorPrefab;
        
        /**    *******************    */

        [HideInInspector] public Mesh mesh;
        private GameObject fieldSelector;
        
        public ActionSelectorState currentState = null;
        public Subject<CharacterAction> onActionSelectedObservable; // set from outside

        public event Action OnEnableSelector, OnDisableSelector;
        
        private void OnValidate() {
            Debug.Assert(walk_material != null, "Action Selector needs a material for the walkable tiles", this);
            Debug.Assert(attack_material != null, "Action Selector needs a material for attack-range tiles", this);
            Debug.Assert(attack_material != null, "Action Selector needs a prefab for the hovered tile", this);
        }
        
        /**    Generic Implementation    */
        
        private void Awake(){
            mesh = new Mesh();
            mesh.MarkDynamic();

            if (fieldSelectorPrefab != null) {
                fieldSelector = Instantiate(fieldSelectorPrefab, this.transform, false);
                fieldSelector.SetActive(false);
            }
        }
        
        private void OnEnable() {
            if (currentState == null) {
                SwitchToMovementSelection();
            }
            OnEnableSelector?.Invoke();
            currentState.OnEnable();
        }

        private void OnDisable() {
            OnDisableSelector?.Invoke();
            currentState?.OnDisable();
        }

        private void Update() {
            currentState.Update(this);
            
        }

        /**    Detail Implementation    */

        public void UIActionDone(CharacterAction action) { currentState.UIActionDone(action); }

        private void ChangeState(Action action) {
            currentState?.OnDisable();
            action();
            if(this.enabled) currentState.OnEnable();
        }
        
        public void SwitchToMovementSelection() {
            ChangeState(() => currentState = new MoveActionSelectorState(ref mesh, ref fieldSelector, ref walk_material, onActionSelectedObservable));
        }

        public void SwitchToAttackSelection() {
            ChangeState(() => currentState = new SelectEnemyActionSelector(onActionSelectedObservable));
        }
        
    }
    
    public abstract class ActionSelectorState : IActionSelector{
        public virtual UniRx.IObservable<CharacterAction> OnActionSelectedObservable { get; }

        public abstract void OnEnable();
        public abstract void OnDisable();
        public abstract void Update(ActionSelectorSM self);
        public abstract void UIActionDone(CharacterAction action);
    }
}