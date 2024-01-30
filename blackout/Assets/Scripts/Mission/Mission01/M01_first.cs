using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class M01_first : MissionEventNode
{
    [SerializeField] private string missionTitle = "�쓮�n�e�X�g";
    private string[] missionItems;
    public string viewControllerText = "�}�E�X";
    public string moveButtonText = "W,A,S,D";
    public string dashButtonText = "�ړ��{��Shift";
    private string[] messageText = new string[]{
        "[�L�[�X]\n�R���\�[���ُ햳���B���c�n�ڑ��m�F�BHUD�����m�F�A�ُ햳���B",
        "[�I�X�J�[]\n�e�X�g���e��HUD�����ɕ\������Ă���͂����B��������Ɋe���ڂ̃e�X�g�𐋍s���Ă���B",
        "[�L�[�X]\n�����B"
    };
    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    public override void EventFire() {
        missionItems = new string[] {
            $"{viewControllerText}�ƏƏ��n��\n�������m�F����",
            $"{moveButtonText}�ƈړ��n��\n�������m�F����",
            $"{dashButtonText}��\n�_�b�V�����[�^��\n�N�����鎖���m�F����"
        };


        mDisp = HUDMissionDisplay.mainDisplay;
        StartCoroutine("Event");
    }
    IEnumerator Event (){
        yield return new WaitForSeconds(1);
        message.function(messageText[0], 4.7f);
        yield return new WaitForSeconds(5);
        mDisp.SetMissionTitle(missionTitle);
        foreach (string s in missionItems) mDisp.GetMissionList().AddMissionItem(s);
        message.function(messageText[1], 4.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[2], 2f);
        ParmitNext();
    }

    // Start is called before the first frame update


    // Update is called once per frame

}
