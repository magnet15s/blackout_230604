using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obj2ObjDistEFlag : MissionEventFlag
{
    [SerializeField] Transform object1;
    [SerializeField] Transform object2;
    [SerializeField] float flagUpDistance;
    [SerializeField] bool flagUpOnInbound = true;
    

    // Update is called once per frame
    void Update()
    {
        if (isActive) {
            if (flagUpOnInbound == ((object1.position - object2.position).magnitude < flagUpDistance)){
                OnFlagUp();
            }
        }
    }
}
