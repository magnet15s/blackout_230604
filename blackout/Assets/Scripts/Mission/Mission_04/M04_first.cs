using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M04_first : MissionEventNode
{
    [SerializeField] Transform gateGuide;
    [SerializeField] RectTransform canvas;
    [SerializeField] GameObject player;
    [SerializeField] GameObject objectTracker;
    [SerializeField] Focus3rdCam f3c;
    private TrackingIcon tIcon;
    private string[] missionItems;
    private string[] messageText = new string[]{
        "[�I�X�J�[����]\n�~�b�V�����J�n�B",
        "[�I�X�J�[����]\n����̍��̖ڕW�́A���݂��q�閳�l�@�A���̒�����n�B\n����ɑ��Q��^���鎖���B",
"[�I�X�J�[����]\n�����h�ǂɂ���Ēn�ォ��̐i�����͌����Ă���B\nHUD��̖ڕW�n�_�֌������Đi�߁B",
"[�I�X�J�[����]\n�ڕW�͂����܂œ����ݔ��̔j�󂾁B\n���ɏ悶�Đڋ߂���ȏ�A�����ɓG�ɍ\���K�v�͂Ȃ��B",
        "[�L�[�X]\n�����B��n�ւ̐ڋ߂��J�n����B"
    };
    [SerializeField] private string missionTitle = "�G��n�֐ڋ�";
    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    // Start is called before the first frame update
    public override void EventFire() {

        //��n�Q�[�g�ɃK�C�h��ݒ�
        tIcon = Instantiate(objectTracker, canvas).GetComponent<TrackingIcon>();
        tIcon.trackingTarget = gateGuide.gameObject;
        tIcon.canvas = canvas;
        tIcon.player = player;

        f3c.SetForcusTarget(gateGuide);
        missionItems = new string[] {
            $"�G�̊�n�ɐڋ߂���"
        };
        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        message = MessageWindow.instance;
        
        StartCoroutine("Event");
    }
    IEnumerator Event() {
        yield return new WaitForSeconds(1);
        message.function(messageText[0], 4.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[1], 4.7f);
        
        yield return new WaitForSeconds(5);
        message.function(messageText[2], 4.7f);
        mDisp.SetMissionTitle(missionTitle);
        foreach (string s in missionItems) mDisp.GetMissionList().AddMissionItem(s);
        yield return new WaitForSeconds(5);
        message.function(messageText[3], 4.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[4], 2.5f);
        ParmitNext();
        yield return null;
    }
}
