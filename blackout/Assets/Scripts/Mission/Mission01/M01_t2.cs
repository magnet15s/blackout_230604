using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M01_t2 : MissionEventNode
{
    [SerializeField] private string missionTitle = "�Ί�ǐ��e�X�g";
    private string[] messageText = new string[]{
        "[�L�[�X]\n�����p������A�ǍD�B\n�e���A�u�\�[�o�[�ُ햳���B",
        "[�I�X�J�[]\n�������ȁB���͉Ί�ǐ��n�����؂���B\n���S���u�͉����ς݂��B�e�퍀�ڂ̃e�X�g���J�n���Ă���B"
    };
    private string weaponChangeButtonText = "R,F�L�[";
    private string weaponActionButtonsText = "���A�E�N���b�N";
    private string subActionButtonText = "�E�N���b�N";
    private string[] missionItems;

    private MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;


    public override void EventFire() {
        missionItems = new string[] {
            $"{weaponChangeButtonText}��\nHUD�E���̕�����\n�؂�ւ�鎖���m�F����",
            $"{weaponActionButtonsText}��\n�e�����A�N�V������\n�N�����邱�Ƃ��m�F����\n[0/2]"
        };
        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        message = MessageWindow.instance;
        StartCoroutine("Event");
    }

    // Start is called before the first frame update


    // Update is called once per frame
    IEnumerator Event() {
        message.function(messageText[0], 3.5f);
        yield return new WaitForSeconds(3.8f);
        message.function(messageText[1], 4f);
        mList.RemoveMissionItems();
        foreach (string s in missionItems) mList.AddMissionItem(s);
        ParmitNext();


    }
}
