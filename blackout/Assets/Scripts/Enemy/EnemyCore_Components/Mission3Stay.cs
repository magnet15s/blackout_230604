using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mission3Stay : MonoBehaviour {
    [SerializeField] EnemyCore core;
    [SerializeField] NavMeshAgent agent;
    //[SerializeField] float secPerTan = 0.1f; 
    private bool notTargetContact = true;
    private delegate void M3sm();
    private M3sm m3sm;
    public void Mission3StayMove() {
        m3sm?.Invoke();
    }

    private void MoveField() {
        if (agent.pathStatus != NavMeshPathStatus.PathInvalid) {

        }
    }


    private void Update() {
        if (core.targetFound && notTargetContact) {
            notTargetContact = false;
            //core.
        }
    }
}
