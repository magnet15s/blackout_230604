using System.Collections;
using UnityEngine;

public class MissileSpawner : MonoBehaviour {

    [SerializeField]
    Transform target;
    [SerializeField]
    GameObject prefab;
    [SerializeField, Min(1)]
    int iterationCount = 3;
    [SerializeField]
    float interval = 0.1f;

    bool isSpawning = false;
    Transform thisTransform;
    WaitForSeconds intervalWait;

    void Start() {
        thisTransform = transform;
        intervalWait = new WaitForSeconds(interval);
    }

    void Update() {
        if (isSpawning) {
            return;
        }

        StartCoroutine(nameof(SpawnMissile));
    }

    IEnumerator SpawnMissile() {
        isSpawning = true;

        Vector3 euler;
        Quaternion rot;
        HomingSample homing;

        for (int i = 0; i < iterationCount; i++) {
            homing = Instantiate(prefab, thisTransform.position, Quaternion.identity).GetComponent<HomingSample>();
            homing.Target = target;
        }

        yield return intervalWait;

        isSpawning = false;
    }

}