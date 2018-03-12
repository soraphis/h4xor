using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace _Game.ScriptRework {
    
    
    public abstract class CharacterAction {
        public abstract Task Execute();
    }

    public class IdleAction : CharacterAction {
        public override async Task Execute() {
            await Task.Run(() => {});
        }
    }
    
    public class MoveAction : CharacterAction {

        private GameObject self;
        private List<NVector2> path;
        private int current;

        public MoveAction(GameObject self, List<NVector2> path) {
            this.self = self;
            this.path = path;
        }

        /* private async Task DoStep(NVector2 dir) {
            
            
        }*/
        
        public override async Task Execute() {
            var mvc = self.GetComponent<MovementController>();

            while (current < path.Count) {
                
                mvc.nextPosition = path[current];
                
                await mvc.OnTileEnterAsObservable;
                ++current;
            }
            return;
        }
    }
    
}