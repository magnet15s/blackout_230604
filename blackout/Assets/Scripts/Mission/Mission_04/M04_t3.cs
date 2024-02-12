using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M04_t3 : MissionEventNode

{
    private MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    private string[] messageText = new string[]{
        "[オスカー大佐]\n成果は十分だ。\n霧がもう晴れている、早急に離脱してくれ。",
        "[オスカー大佐]\nミッション完了だ。"
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
        yield return new WaitForSeconds(4);
        message.function(messageText[1], 2.7f);
        yield return new WaitForSeconds(3);
        message.function("\nMission Complete!", 3);
        yield return new WaitForSeconds(2);
        Initiate.Fade("menu_02", Color.black, 1.0f);
        yield break;
    }

}
