using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M01_end : MissionEventNode {

    [SerializeField]MessageWindow mw;
    public override void EventFire() {

        StartCoroutine("Event");
    }

    IEnumerator Event() {
        Debug.Log("Mission End");
        yield return new WaitForSeconds(5);
        mw.function("\nMission Complete!", 3);
        yield return new WaitForSeconds(2);
        Initiate.Fade("menu_02", Color.black, 1.0f);
        yield break;

    }
}
