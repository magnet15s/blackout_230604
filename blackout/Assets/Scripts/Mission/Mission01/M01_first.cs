using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M01_first : MissionEventNode
{
    public override void EventFire() {
        GameObject o = new GameObject();
        o.name = "mimimi";
        parmitNext.Invoke(this, EventArgs.Empty);
    }

    // Start is called before the first frame update
    

    // Update is called once per frame
    
}
