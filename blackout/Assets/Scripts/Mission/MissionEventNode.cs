using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MissionEventNode : MonoBehaviour
{

    public abstract void EventFire();
    public EventHandler parmitNext;
    
    protected virtual void OnParmitNext() {
        parmitNext.Invoke(this, EventArgs.Empty);
    }

}
