using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mission3Stay : MonoBehaviour {
    [SerializeField] EnemyCore core;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform center;
    [SerializeField] float radius;
    [SerializeField] float radDamp;
    private bool notTargetContact = true;
    private delegate void M3sm();
    private M3sm m3sm;
    private void Awake() {
        m3sm += MoveField;
    }
    public void Mission3StayMove() {
        m3sm?.Invoke();
    }

    private void MoveField() {
        if (agent.pathStatus != NavMeshPathStatus.PathInvalid) {
            Vector3 targetForward;
            float outin;

            if (Mathf.Abs(outin = ((center.position - transform.position).magnitude - radius)) > radDamp) {
                //半径修正
                targetForward = center.position - transform.position * Mathf.Sign(outin);
                targetForward.Normalize();
            } else {
                //巡回
                //upベクトルと自ワールド座標 - 中央座標ベクトルの外積方向へ
                Debug.Log("ddd");
                targetForward = Vector3.Cross(Vector3.up, (transform.position - center.position).normalized);
            }

            agent.destination = targetForward;
        }
    }


    private void Update() {
        if (core.targetFound && notTargetContact) {
            notTargetContact = false;
            m3sm -= MoveField;
        }
    }
}
