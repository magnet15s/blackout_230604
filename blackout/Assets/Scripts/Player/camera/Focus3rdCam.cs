using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focus3rdCam : MonoBehaviour
{
    [SerializeField] Transform defaultFocusTarget = null;
    [SerializeField] CinemachineVirtualCamera vcam;
    
    public void SetForcusTarget(Transform target) {
        if (target == null) vcam.LookAt = defaultFocusTarget;
        else vcam.LookAt = target;
    }

    
}
