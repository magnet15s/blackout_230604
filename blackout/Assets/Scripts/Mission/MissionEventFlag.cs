using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionEventFlag : MonoBehaviour
{
    public bool isActive = false;
    public bool ignited { get; protected set; }
    [SerializeField] protected MissionEventNode targetEventNode;
    public MissionEventNode getTargetEventNode() { return targetEventNode; }
    
}
