using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M04_t1 : MissionEventNode
{

    [SerializeField] Transform gateGuide;

    private MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    public override void EventFire()
    {
        //基地ゲートのガイドを削除
        Destroy(gateGuide.gameObject);

        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        message = MessageWindow.instance;
        StartCoroutine("Event");
    }

    IEnumerator Event()
    {
        //壁上のタレットにも気をつけろ～的なテキスト
        ParmitNext();
        yield return null;
    }
}
