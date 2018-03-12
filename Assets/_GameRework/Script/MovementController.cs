using System;
using Gamelogic.Extensions;
using UniRx;
using UnityEngine;
using _Game.ScriptRework.Util;

namespace _Game.ScriptRework {
    public class MovementController : MonoBehaviour{
        public const float MOVEMENT_DEADZONE_SQR = 0.01f ;
        
        private readonly Subject<bool> onTileEnterAsObservable = new Subject<bool>();
        public UniRx.IObservable<bool> OnTileEnterAsObservable { get { return onTileEnterAsObservable.AsObservable(); } }

        private NVector2 desiredPosition;
        public NVector2? nextPosition;
        [Range(0.1f, 20f)]public float speed = 3;
        private float rotationSpeedFactor = 1;

        private NVector2 dir;
        [SerializeField] private Animator animator;
        
        private void Start() {
            ResetDesiredPosition();
        }

        public void ResetDesiredPosition() {
            desiredPosition = new NVector2(GridUtil.WorldToGrid(this.transform.position));
        }

        private bool dirty = true; // quick hack
        
        private void Update() {
            NVector2 gridpos = new NVector2(GridUtil.WorldToGrid(this.transform.position));
            Vector3 desiredWorldPos = GridUtil.GridToWorld(desiredPosition);
            
            animator.SetBool("walking", false);
            
            if (desiredWorldPos.Approx(transform.position, MOVEMENT_DEADZONE_SQR)) {
                // reached field
                this.transform.position = GridUtil.GridToWorld(desiredPosition);

                if(dirty) onTileEnterAsObservable.OnNext(true);
                dirty = false;
                
                if (nextPosition == null) return;
                if (nextPosition == desiredPosition) {
                    onTileEnterAsObservable.OnNext(false);
                    return;
                }
                
                desiredPosition = (NVector2) nextPosition;
                nextPosition = null;
                
                dirty = true;
                dir = (desiredPosition - gridpos);
                
                var mg = (dir.x * dir.x + dir.y * dir.y);
                if (mg != 1) {
                    if(mg == 0) return; // standing still !?
                    throw new ArgumentException(this.name + ": Unknown direction: " + dir);
                }
            }
            
            // rotate towards dir:
            var desiredRotation = Quaternion.LookRotation(((Vector2)dir).To3DXZ(), Vector3.up);
            var desiredAngle = Quaternion.Angle(transform.rotation, desiredRotation);
            if (desiredAngle > 5) {
                var rot = Quaternion.RotateTowards(transform.rotation, desiredRotation, 90* rotationSpeedFactor * Time.deltaTime);
                transform.rotation = rot;
                rotationSpeedFactor += 16 * Time.deltaTime;
            } else {
                animator.SetBool("walking", true);
                transform.rotation = desiredRotation;
                rotationSpeedFactor = 1;
                
                //transform.position = transform.position + (((Vector2)dir) * Time.deltaTime * speed).To3DXZ();
                transform.position = Vector3.MoveTowards(transform.position, desiredWorldPos, Time.deltaTime * speed);
            }

        }
/*
        private void OnDrawGizmos() {
            Vector3 desiredWorldPos = GridUtil.GridToWorld(desiredPosition);
            
            
            Gizmos.color = Color.black;
            Gizmos.DrawCube(desiredWorldPos, Vector3.one * 0.7f);

            
        }*/
        
    }

}