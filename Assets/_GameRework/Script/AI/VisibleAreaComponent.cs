using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.Experimental.U2D;
using UnityEngine.Rendering;
using _Game.ScriptRework.Util;

namespace _Game.ScriptRework.AI {
/*
    public struct Centered2DArray<T> {
        private T[,] array;
        private NVector2 center;

        public Centered2DArray(NVector2 center, int size_x, int size_y) : this() {
            this.center = center;
            array = new T[size_x,size_y];
        }
        
        public Centered2DArray(NVector2 center, T[,] array) : this() {
            this.center = center;
            this.array = array;
        }
        
        public T this[int x, int y]{
            get { return array[center.x + x, center.y + y]; }
            set { array[center.x + x, center.y + y] = value; }
        }
        
        public int GetLength(int i) { return array.GetLength(i);  }
    }
*/    
    public class VisibleAreaComponent : MonoBehaviour{
        [HideInInspector] public Mesh mesh;
        public Material material;

        private EnemyActor enemyActor;
        private MovementController movementController;

        private bool[,] visibleArea;

        //TODO private bool listen = false; // listens to player steps
        
        void Awake() {
            mesh = new Mesh();
            mesh.MarkDynamic();
            
            enemyActor = GetComponent<EnemyActor>();
            
            movementController = GetComponent<MovementController>();
            movementController.OnTileEnterAsObservable.Subscribe(RecalculateVisibleArea);
        }


        private void Update() {
            // RecalculateVisibleArea(true); // todo: remove, its just debug
            
            
            if(mesh == null || material == null) return;
            Graphics.DrawMesh(mesh, Matrix4x4.Translate(GridUtil.GridToWorld(enemyActor.GridPosition)), material, 0, Camera.current, 0, new MaterialPropertyBlock(), ShadowCastingMode.Off, receiveShadows:false);

            if (IsPlayerVisible()) {
                    // todo: this should not be needed every frame ... 
                enemyActor.DoAlert();
            }

        }

        public bool IsPlayerVisible() {
            if(visibleArea == null) return false;
            
            var forward = new NVector2(this.transform.forward.To2DXZ());
            var right = new NVector2(forward.y, -forward.x);
            var pgrid_pos = PlayerActor.Instance.GridPosition - enemyActor.GridPosition;

            var dx = right.x * pgrid_pos.x + right.y * pgrid_pos.y + visibleArea.GetLength(0)/2;
            var dy = forward.x * pgrid_pos.x + forward.y * pgrid_pos.y;
            
            return visibleArea.GetLength(0) > dx && dx >= 0 && 
                   visibleArea.GetLength(1) > dy && dy >= 0 && 
                   visibleArea[dx, dy];
        }

        /*void OnDrawGizmos() {
            Gizmos.color = new Color(1, 0.45f, 0.25f, 0.5f);
            
            if(visibleArea == null) return;

            var awareness = enemyActor.stats.currentStats.awareness;
            var pos = enemyActor.GridPosition;
            var forward = new NVector2(this.transform.forward.To2DXZ());
            var right = new NVector2(forward.y, -forward.x);

            for (int x = 0; x < (awareness*2)+1; ++x) {
                for (int y = 0; y < awareness+1; ++y) {
                    if(! visibleArea[x, y]) continue;

                    var wpos = GridUtil.GridToWorld((pos + forward*y + right*(x-awareness-1)));
                    
                    Gizmos.DrawCube(wpos, Vector3.one * 0.25f);
                }
            }
            /*
            for (int x = -visibleArea.GetLength(0) / 2; x < visibleArea.GetLength(0) / 2; ++x) {
                for (int y = -visibleArea.GetLength(1) / 2; y < visibleArea.GetLength(1) / 2; ++y) {
                    if(visibleArea[x + visibleArea.GetLength(0) / 2, y + visibleArea.GetLength(1) / 2])
                        
                        
                        Gizmos.DrawCube(this.transform.position - new Vector3(x, 0, y), Vector3.one * 0.25f);
                } 
            }
            #1#
        }*/
        
        public void RecalculateVisibleArea(bool step) {
            if(!step) return;
            
            var awareness = enemyActor.stats.currentStats.awareness;
            
            
            visibleArea = VisibleArea.CalculateVisibleArea(
                enemyActor.GridPosition, 
                awareness, 
                new NVector2(this.transform.forward.To2DXZ()),
                viewAngle: enemyActor.stats.viewAngle
            );

            var vol = GenerationLib.makeVolume(
                new float[3][] {
                    new float[] {-awareness, awareness+1, 1f}, 
                    new float[] {0, 1, 1f}, 
                    new float[] {0, awareness+1, 1f}
                }, (x, y, z) => {
                    if ((int)y < 0.5f) return 0;
                    
                    if ((awareness + x) < 0 || (awareness + x) >= visibleArea.GetLength(0)) return 0;
                    if ((z) < 0 || (z) >= visibleArea.GetLength(1)) return 0;
                    return visibleArea[awareness+(int)x, (int)z] ? 1 : 0;
                });

            
            // copied from PlayerActionSelector:
                // potential refactor to an own method
            
            List<int> iBuffer;
            List<Vector2> uvs;
            List<Vector3> vBuffer; 
            SurfaceNets.GenerateSurfaceMesh(vol, 0.5f, out vBuffer, out iBuffer, out uvs);

      //      var delta = 0f;//(awareness - awareness)/2.0f +0.5f;
            
    //        var gridpos = enemyActor.GridPosition;
            var forward = new NVector2(this.transform.forward.To2DXZ());
     //       var right = new NVector2(forward.y, -forward.x);
            
            vBuffer = vBuffer.Select(v => this.transform.forward * (v.z -0.5f) + this.transform.right * (v.x - awareness - 0.5f)).ToList();

            // this.walkableTiles = new HashSet<NVector2>(walkableTiles); 
            
            mesh.Clear();
            mesh.SetVertices(vBuffer);
            mesh.SetIndices(iBuffer.ToArray(), MeshTopology.Quads, 0);
            mesh.uv = uvs.ToArray();
                
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
        
    }
}