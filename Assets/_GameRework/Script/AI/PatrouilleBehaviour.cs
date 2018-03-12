using System.Collections.Generic;
using Gamelogic.Extensions;
using UnityEngine;

namespace _Game.ScriptRework.AI {

    [ExecuteInEditMode]
    public class PatrouilleBehaviour : MonoBehaviour {

        public enum PatrouilleType {
            OneWay, Cycle, TurnAround
        }
        
        public Waypoints waypoints;
        // public int current;
        public PatrouilleType type;
        
        // public Vector2 currentWaypoint{ get { return waypoints.points[current];  } }

        void OnValidate() {
            var own_grid_pos = new NVector2(GridUtil.WorldToGrid(this.transform.position));
            if (waypoints.points.Count < 1) waypoints.points.Add(own_grid_pos);
            else {
                if (waypoints.points[0] != own_grid_pos) {
                    waypoints.points.Insert(0, own_grid_pos);
                }
            }
        }

        void Awake() {
            OnValidate();
        }
    }
    
    
    
    [System.Serializable]
    public struct Waypoints {
        public List<NVector2> points;
    }
    
}