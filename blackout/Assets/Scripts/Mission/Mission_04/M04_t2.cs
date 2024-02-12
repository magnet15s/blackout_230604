using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M04_t2 : MissionEventNode

{
    private MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    private string[] messageText = new string[]{
        "[�I�X�J�[�卲]\n�z���薶�̈����������B�C���̐��s���}���B"
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
