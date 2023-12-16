using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M01_end : MissionEventNode {
    public override void EventFire() {
        Debug.Log("Mission End");

        parmitNext.Invoke(this, EventArgs.Empty);
    }
}
