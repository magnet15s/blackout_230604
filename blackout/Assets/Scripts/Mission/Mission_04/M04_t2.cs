using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M04_t2 : MissionEventNode

{
    private MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    public override void EventFire()
    {

        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        message = MessageWindow.instance;
        StartCoroutine("Event");
    }

    IEnumerator Event()
    {
        //��������Ă����`�I�ȃe�L�X�g
        ParmitNext();
        yield return null;
    }

}
