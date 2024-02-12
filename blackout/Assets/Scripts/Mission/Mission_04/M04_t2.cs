using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M04_t2 : MissionEventNode

{
    private MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    private string[] messageText = new string[]{
        "[オスカー大佐]\n想定より霧の引きが早い。任務の遂行を急げ。"
    };
    public override void EventFire()
    {

        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        StartCoroutine("Event");
        message = MessageWindow.instance;
    }

    IEnumerator Event()
    {
        yield return new WaitForSeconds(1);
        message.function(messageText[0], 3.7f);
        ParmitNext();
    }

}
