using System.Linq;
using UniRx;
using UnityEngine;

namespace _Game.ScriptRework.ActionSelectors {
    public class SelectEnemyActionSelector : ActionSelectorState {

        private static MaterialPropertyBlock activeCircle = new MaterialPropertyBlock();

        static SelectEnemyActionSelector() {
            activeCircle.SetColor("_TintColor", Color.red);
        }

        private Plane plane = new Plane(Vector3.up, Vector3.zero);

        public readonly Subject<CharacterAction> onActionSelectedObservable;

        public SelectEnemyActionSelector(Subject<CharacterAction> onActionSelectedObservable) {
            this.onActionSelectedObservable = onActionSelectedObservable;
        }

        public override void OnEnable() {}
        public override void OnDisable() {}

        public override void Update(ActionSelectorSM self) {
            var mesh = self.selectionCircleMesh;

            var enemiesInRange = GameTickManager.Instance.activeEnemies.Where(
                e => Astar.CalculateHeuristic(e.GridPosition, PlayerActor.Instance.GridPosition) 
                     < PlayerActor.Instance.stats.currentStats.awareness).ToList();
            
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float enter;
            if (! plane.Raycast(ray, out enter)) return;
            
            var field = new NVector2(GridUtil.WorldToGrid(ray.GetPoint(enter)));

            int? is_valid = null;
            for (var i = 0; i < enemiesInRange.Count; ++i) {
                if (enemiesInRange[i].GridPosition == field) {
                    is_valid = i;
                    Graphics.DrawMesh(mesh, 
                        Matrix4x4.Translate(enemiesInRange[i].transform.position), self.selectionCircleMaterial, 0, Camera.current, 0, activeCircle);
                }else 
                    Graphics.DrawMesh(mesh, 
                        Matrix4x4.Translate(enemiesInRange[i].transform.position), self.selectionCircleMaterial, 0, Camera.current, 0);
            }
            
            if (Input.GetMouseButtonDown(0) && is_valid != null) {
                var enemy = enemiesInRange[(int) is_valid];
                var selectedAction = new AttackAction(PlayerActor.Instance.gameObject, enemy.gameObject, enemy.attackPrefab, 1);
                onActionSelectedObservable.OnNext(selectedAction);
            }
            
        }

        public override void UIActionDone(CharacterAction action) { onActionSelectedObservable.OnNext(action); }
    }
}