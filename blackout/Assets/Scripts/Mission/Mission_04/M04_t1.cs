using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M04_t1 : MissionEventNode
{

    [SerializeField] Transform gateGuide;
    [SerializeField] Focus3rdCam f3c;
    private string[] missionItems;
    private string[] messageText = new string[]{
        "[�L�[�X]\n�ڕW�n�_�ɓ����B�U�����J�n����B",
        "[�I�X�J�[�卲]\n�h�Ǐ�̃^���b�g�͋��ЂɂȂ�B\n�K�v�ɉ����Ĕr������B"
    };
    [SerializeField] private string missionTitle = "�j��";
    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    public override void EventFire()
    {
        //��n�Q�[�g�̃K�C�h���폜
        f3c.SetForcusTarget(null);
        Destroy(gateGuide.gameObject);
        missionItems = new string[] {
            $"�������̔j��"
        };
        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        StartCoroutine("Event");
        message = MessageWindow.instance;
    }

    IEnumerator Event()
    {
        yield return new WaitForSeconds(1);
        mList.RemoveMissionItems();
        foreach (string s in missionItems) mList.AddMissionItem(s);
        message.function(messageText[0], 3.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[1], 4.7f);
        ParmitNext();
    }
}
