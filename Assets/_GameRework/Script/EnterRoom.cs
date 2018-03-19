using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Game.ScriptRework;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EnterRoom : MonoBehaviour {

    [SerializeField] private AudioClip scanClip;
    [SerializeField] private GameObject minimapMask;

    public NVector2 playerEnterTile { get; private set; }
    
    private List<EnemyActor> roomEnemies = new List<EnemyActor>();
    private List<Vector3> positions = new List<Vector3>();

    IEnumerator Start() {
        yield return null;
        DeactivateRoom();
    }
    
    public void ActivateRoom() {
        AudioSource.PlayClipAtPoint(scanClip, PlayerActor.Instance.transform.position);

        minimapMask.SetActive(false);
        for (var i = 0; i < roomEnemies.Count; ++i) {
            // GameTickManager.Instance.activeEnemies.AddRange(roomEnemies);
            roomEnemies[i].gameObject.SetActive(true);
        }
    }
    
    public void DeactivateRoom() {
        for (var i = 0; i < roomEnemies.Count; ++i) {
            roomEnemies[i].gameObject.SetActive(false);
            roomEnemies[i].transform.position = positions[i];
        }
    }
    
    void OnTriggerEnter(Collider other) {
        MonoBehaviour c = other.GetComponent<EnemyActor>();
        if (c != null) {
            roomEnemies.Add((EnemyActor)c);
            positions.Add(c.transform.position);
            return;
        }
        c = other.GetComponent<PlayerActor>();
        if (c != null) {
            GameTickManager.Instance.ActiveRoom = this;
            playerEnterTile = ((PlayerActor) c).GridPosition;
            return;
        }
    }

}
