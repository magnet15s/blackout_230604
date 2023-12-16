using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M01_t1 : MissionEventNode
{
    public override void EventFire() {
        GameObject o = GameObject.Find("mimimi");
        GameObject o2 = new GameObject();
        o2.transform.SetParent(o.transform);
        o2.transform.localPosition = new Vector3(0,1,0);

        parmitNext?.Invoke(this, EventArgs.Empty);
    }

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        
    }
}
