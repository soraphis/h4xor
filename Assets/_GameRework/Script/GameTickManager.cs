using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Game.ScriptRework.AI;

namespace _Game.ScriptRework {
    public class GameTickManager : Singleton<GameTickManager> {

        public HashSet<EnemyActor> activeEnemies = new HashSet<EnemyActor>();
        private PlayerActor player;

        public bool exitGame = false;

        public event Action Gizmos;


        void Awake() {
            var x = CameraFadeScript.SetupCameraFade();
            x.intensity = 1;
        }
        
        private IEnumerator Start() {
            var loadUIScene = SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);
            player = PlayerActor.Instance;
            
            yield return null; // wait for first frame
            while (!loadUIScene.isDone) { yield return null; }
            yield return CameraFadeScript.FadeIn(1.4f);
            
            Tick();
        }

        
        public async Task Tick() {
            while (!exitGame) {
                // evaluate player
                await player.TickAwaitable();

                // evaluate Enemies
                foreach (var enemy in activeEnemies) {
                    await enemy.TickAwaitable();                    
                }
            }
            Debug.Log("i probably should not end here ... ");
            Application.Quit();
        }
        
        void OnDrawGizmos() {
            if (Gizmos != null) Gizmos.Invoke();
            
        }

        
    }

    public interface ITickBehaviour {
        Task TickAwaitable();
    }
    
    
}