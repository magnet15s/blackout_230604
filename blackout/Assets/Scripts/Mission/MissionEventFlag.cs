using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MissionEventFlag : MonoBehaviour
{
    public bool isActive = false;
    public static List<MissionEventFlag> flagList = new();

    public EventHandler onFlagUp;

    protected virtual void Awake() {
        flagList.Add(this);
    }
    protected virtual void OnDestroy() {
        flagList.Remove(this);
    }
    protected virtual void OnFlagUp()
    {
        Debug.Log("onflugup");
        ignited = true;
        isActive = false;
        onFlagUp?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// そのフラグがすでに発動しているか（OnFlagUp()でtrueに）
    /// </summary>
    public bool ignited { get; private set; }

    
}
