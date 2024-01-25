using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M01_t3 : MissionEventNode
{
    /*[SerializeField] private string missionTitle = "�Ί�ǐ��e�X�g";
    private string[] messageText = new string[]{
        "[�L�[�X]\n�����p������A�ǍD�B\n�e���A�u�\�[�o�[�ُ햳���B",
        "[�I�X�J�[]\n�������ȁB���͉Ί�ǐ��n�����؂���B\n���S���u�͉����ς݂��B�e�퍀�ڂ̃e�X�g���J�n���Ă���B"
    };
    private string weaponChangeButtonText = "R,F�L�[";
    private string fireButtonText = "���N���b�N";
    private string subActionButtonText = "�E�N���b�N";
    private string[] missionItems;*/

    private MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;


    public override void EventFire() {
        /*missionItems = new string[] {
            $"{weaponChangeButtonText}��\nHUD�E���̑I�𕐑���\n�؂�ւ�鎖���m�F����",
            $"{fireButtonText},{subActionButtonText}��\n�I�𒆂̕����A�N�V������\n�N�����邱�Ƃ��m�F����\n[0/2]"
        };*/
        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        message = MessageWindow.instance;
        StartCoroutine("Event");
        parmitNext?.Invoke(this, EventArgs.Empty);
    // Start is called before the first frame update
    }



    // Update is called once per frame
    IEnumerator Event() {
        mList.RemoveMissionItems();
        parmitNext?.Invoke(this, EventArgs.Empty);
        yield break;

    }
}
