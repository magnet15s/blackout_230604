using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M01_t1 : MissionEventNode
{
    [SerializeField] private string missionTitle = "�@���e�X�g";
    private string[] messageText = new string[]{
        "[�L�[�X]\n�쓮�n�A��薳���B",
        "[�I�X�J�[]\n�e�X�g���ڂ��X�V�����B���̃e�X�g�Ɉڂ��Ă���B"
    };
    private string jumpButtonText = "Space";
    private string airAxelButtonText = "Space";
    private string evationMoveButtonText = "�W�����v��������\n�ēxSpace";
    private string[] missionItems;

    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;


    public override void EventFire() {
        missionItems = new string[] {
            $"{jumpButtonText}�ŃW�����v��\n�������鎖���m�F����",
            $"�󒆂�{airAxelButtonText}��\n�G�A�A�N�Z����\n���삷�鎖���m�F����",
            $"{evationMoveButtonText}��\n�ً}����@����\n�������鎖���m�F����"
        };
        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        StartCoroutine("Event");
        parmitNext?.Invoke(this, EventArgs.Empty);
    }

    // Start is called before the first frame update


    // Update is called once per frame
    IEnumerator Event() {
        message.function(messageText[0], 3.5f);
        yield return new WaitForSeconds(3.8f);
        message.function(messageText[1], 4f);
        mList.RemoveMissionItems();
        foreach (string s in missionItems) mList.AddMissionItem(s);
        parmitNext?.Invoke(this, EventArgs.Empty);


    }
}
