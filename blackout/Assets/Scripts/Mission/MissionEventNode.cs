using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionEventNode : MonoBehaviour
{
    [SerializeField] private bool firstEvent = false;

    public abstract void EventFire();

    // Start is called before the first frame update
    void Start() {
        if (firstEvent) {
            this.EventFire();
        }
    }

}
