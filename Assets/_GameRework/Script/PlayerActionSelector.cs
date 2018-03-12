using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using _Game.ScriptRework.Util;

namespace _Game.ScriptRework {
    public class PlayerActionSelector : MonoBehaviour, IActionSelector {
        
        private readonly Subject<CharacterAction> onActionSelectedObservable = new Subject<CharacterAction>();
        public UniRx.IObservable<CharacterAction> OnActionSelectedObservable { get { return onActionSelectedObservable.AsObservable(); } }

        [HideInInspector] public Mesh mesh;
        public Material material;
        
        public GameObject fieldSelectorPrefab;
        private GameObject fieldSelector;

        private HashSet<NVector2> walkableTiles = new HashSet<NVector2>();
        private Plane plane = new Plane(Vector3.up, Vector3.zero);
        public CharacterAction selectedAction;

        public event Action OnEnableSelector, OnDisableSelector;

        public void UIActionDone(CharacterAction action) {
            onActionSelectedObservable.OnNext(action);
        }
        
        private void Awake(){
            mesh = new Mesh();
            mesh.MarkDynamic();

            if (fieldSelectorPrefab != null) {
                fieldSelector = Instantiate(fieldSelectorPrefab, this.transform, false);
                fieldSelector.SetActive(false);
            }
        }

        private void Update() {
            if(mesh == null || material == null) return;
            Graphics.DrawMesh(mesh, Matrix4x4.Translate(transform.position), material, 0, Camera.current, 0, new MaterialPropertyBlock(), ShadowCastingMode.Off, receiveShadows:false);
            
            /* replace by input provider: */
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float enter;
            if (plane.Raycast(ray, out enter)) {
                var field = new NVector2(GridUtil.WorldToGrid(ray.GetPoint(enter)));
                
                fieldSelector.transform.position = 
                        Vector3.MoveTowards(fieldSelector.transform.position, 
                        GridUtil.GridToWorld(field),
                        32 * Time.deltaTime);
                
                var sel = new NVector2(GridUtil.WorldToGrid(fieldSelector.transform.position));

                if (Math.Abs(sel.x - field.x + sel.y - field.y) > 0) return;
                
                var is_valid = walkableTiles.Contains(sel);
                fieldSelector.SetActive(is_valid);

                if (Input.GetMouseButtonDown(0) && is_valid) {
                    var path = Astar.Path(PlayerActor.Instance.GridPosition, sel);
                    selectedAction = new MoveAction(PlayerActor.Instance.gameObject, path);
                    onActionSelectedObservable.OnNext(selectedAction);
                }
                
            }
            
        }

        private void OnEnable() {
            CalculateWalkableTiles();
            if (OnEnableSelector != null) OnEnableSelector.Invoke();
        }

        private void OnDisable() {
            fieldSelector.SetActive(false);
            if (OnDisableSelector != null) OnDisableSelector.Invoke();
        }

        private void CalculateWalkableTiles() {
            var player = PlayerActor.Instance;
            var distance = player.stats.currentStats.speed;
            var pos = GridUtil.GridToWorld(player.GridPosition);
            
            
            // var VBuffer = new List<Vector3>();
            
            
            var walkableTiles = Astar.Area(player.GridPosition, distance);
            

            var vol = GenerationLib.makeVolume(
                new float[3][] {
                    new float[] {-distance-1, distance+1, 1f}, 
                    new float[] {0, 1, 1f}, 
                    new float[] {-distance-1, distance+1, 1f}
                }, (x, y, z) => {
                    if ((int)y < 0.5f) return 0;
                    return walkableTiles.Contains(new NVector2((int) x+player.GridPosition.x, (int) z+player.GridPosition.y)) ? 1 : 0;
                });

            List<int> iBuffer;
            List<Vector2> uvs;
            List<Vector3> vBuffer; 
            SurfaceNets.GenerateSurfaceMesh(vol, 0.5f, out vBuffer, out iBuffer, out uvs);

            var delta = ((distance + 1) - (-distance - 1))/2.0f +0.5f;
            
            vBuffer = vBuffer.Select(v => new Vector3(v.x -delta, 0.0f, v.z -delta)).ToList();

            this.walkableTiles = new HashSet<NVector2>(walkableTiles); 
            
            mesh.Clear();
            mesh.SetVertices(vBuffer);
            mesh.SetIndices(iBuffer.ToArray(), MeshTopology.Quads, 0);
            mesh.uv = uvs.ToArray();
                
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
        }
    }
}