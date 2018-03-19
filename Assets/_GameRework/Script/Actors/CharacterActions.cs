using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public class AttackAction : CharacterAction {
        private readonly GameObject self;
        private readonly GameObject other;
        private readonly GameObject attackPrefab;
        private readonly int damage;

        public AttackAction(GameObject self, GameObject other, GameObject attackPrefab, int damage) {
            this.self = self;
            this.other = other;
            this.attackPrefab = attackPrefab;
            this.damage = damage;
        }

        public override async Task Execute() {
//            await Task.Run(() => {  });
            var b = GameObject.Instantiate(attackPrefab, self.transform.position, Quaternion.identity);
            
            for (float f = 0; f < 1; f += 2*Time.deltaTime) {
                b.transform.position = Vector3.Lerp(self.transform.position + Vector3.up, 
                                                    other.transform.position + Vector3.up, 
                    f);
                
                await Task.Run(() => new WaitForEndOfFrame());
            }
            
            for (float f = 1; f < 3; f += 0.2f * f+Time.deltaTime) {
                b.transform.localScale = Vector3.one * f;
                await Task.Run(() => new WaitForEndOfFrame());
            }

            ExecuteEvents.Execute<ITakeDamageHandler>(other, null, (handler, data) => handler.TakeDamage(damage));
            GameObject.Destroy(b);
            
        }

    }
    
}