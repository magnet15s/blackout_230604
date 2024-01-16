using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M01_end : MissionEventNode {

    [SerializeField]MessageWindow mw;
    public override void EventFire() {
        Debug.Log("Mission End");
        mw.function("Mssion Complete!", 2);
        parmitNext.Invoke(this, EventArgs.Empty);
    }
}
