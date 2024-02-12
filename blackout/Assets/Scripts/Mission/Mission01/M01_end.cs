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
        yield return new WaitForSeconds(1);

        mw.function("[オスカー少佐]\n全機の撃破を確認した。", 3);
        yield return new WaitForSeconds(3.3f);
        mw.function("[オスカー少佐]\nデータは十分だ。以上で試験を終了する。\nご苦労。", 3);
        yield return new WaitForSeconds(4);
        mw.function("\nMission Complete!", 3);
        yield return new WaitForSeconds(2);
        Initiate.Fade("menu_02", Color.black, 1.0f);
        yield break;

    }
}
