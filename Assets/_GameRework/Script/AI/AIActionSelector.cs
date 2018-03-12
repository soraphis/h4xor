using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UniRx;
using UnityEngine;

namespace _Game.ScriptRework.AI {

    public enum AIState {
        Default,
        Alterted,
        Enraged // maybe a boss state?
    }
    
    [System.Serializable]
    public class AIActionSelector {
        /* This is the AI Brain, it knows its state and its behaviour tree and chooses an action depending on it */

        //private readonly Subject<CharacterAction> onActionSelectedObservable = new Subject<CharacterAction>();
        //public IObservable<CharacterAction> OnActionSelectedObservable { get { return onActionSelectedObservable; } }


        public AIActionSelector(EnemyActor actor) {
            this.actor = actor;
            this.state = AIState.Default;

            this.patrouille = actor.GetComponent<PatrouilleBehaviour>();
        }

        private EnemyActor actor;
        public AIState state;
        private PatrouilleBehaviour patrouille;
        private int lastWaypoint = 0;
        private int currentWaypoint = 0;

        private void CalculateNextWaypoint() {
            if(patrouille == null) return;

            if (patrouille.type == PatrouilleBehaviour.PatrouilleType.Cycle) {
                lastWaypoint = currentWaypoint;
                currentWaypoint = (currentWaypoint + 1) % patrouille.waypoints.points.Count;
            }else if (patrouille.type == PatrouilleBehaviour.PatrouilleType.TurnAround) {
                int delta = lastWaypoint > currentWaypoint ? -1 : 1;

                lastWaypoint = currentWaypoint;
                currentWaypoint = (int)Mathf.PingPong(currentWaypoint + delta, patrouille.waypoints.points.Count - 1);
            }else if (patrouille.type == PatrouilleBehaviour.PatrouilleType.OneWay) {
                
                lastWaypoint = currentWaypoint;
                currentWaypoint = (int)Mathf.MoveTowards(currentWaypoint, patrouille.waypoints.points.Count - 1, 1);
                
            }
            
        }
        
        private bool isPlayerVisible() {
            return actor.visibleArea.IsPlayerVisible();
        }

        /// <returns>An A* Path to the Target, from the AI position, considering the Speed Attribute</returns>
        private List<NVector2> ClampedPathToTarget(NVector2 target) {
            var path = Astar.Path(actor.GridPosition, target, distance: 1);
            if (path.Count > actor.stats.currentStats.speed) { path = path.GetRange(0, actor.stats.currentStats.speed); }
            return path;
        }
        
        public CharacterAction Evaluate() {

            if (state == AIState.Alterted) {
                if (isPlayerVisible()) {
                    Debug.Log("attack!");
                    return new AttackAction(actor.gameObject, PlayerActor.Instance.gameObject, actor.attackPrefab, actor.stats.currentStats.atk);
                    
                } else {
                    // chase!
                    Debug.Log("chase");
                    var path = ClampedPathToTarget(PlayerActor.Instance.GridPosition);

                    if (path.Count == 0) {
                        Debug.Log("If this Line happens, there should be a BUG in the AI behaviour ... ");
                        return new IdleAction();
                    }
                    return new MoveAction(actor.gameObject, path);
                }
            } else {
                if (patrouille != null) {
                    var next_point = patrouille.waypoints.points[currentWaypoint];
                    if (actor.GridPosition == next_point) {
                        CalculateNextWaypoint();
                        next_point = patrouille.waypoints.points[currentWaypoint];
                    }

                    var path = ClampedPathToTarget(next_point);


                    if (path.Count == 0) {
                        Debug.Log("already there! : " + currentWaypoint);
                        return new IdleAction();
                    }
                    return new MoveAction(actor.gameObject, path);
                } else {
                    Debug.Log("i dont know what to do ... ");
                    return new IdleAction();
                }
            }
            
        }


    }
}