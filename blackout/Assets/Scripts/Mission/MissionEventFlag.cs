using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MissionEventFlag : MonoBehaviour
{
    public bool isActive = false;


    public EventHandler onFlagUp;
    protected virtual void OnFlagUp()
    {
        Debug.Log("onflugup");
        ignited = true;
        onFlagUp?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// ���̃t���O�����łɔ������Ă��邩�iOnFlagUp()��true�Ɂj
    /// </summary>
    public bool ignited { get; private set; }




/*
    [SerializeField] protected MissionEventNode targetEventNode;

    public MissionEventNode getTargetEventNode() { return targetEventNode; }*/
    
}
