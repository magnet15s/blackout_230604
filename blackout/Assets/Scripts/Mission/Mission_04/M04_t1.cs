using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M04_t1 : MissionEventNode
{

    [SerializeField] Transform gateGuide;
    [SerializeField] Focus3rdCam f3c;

    private MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    public override void EventFire()
    {
        //��n�Q�[�g�̃K�C�h���폜
        f3c.SetForcusTarget(null);
        Destroy(gateGuide.gameObject);

        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        message = MessageWindow.instance;
        StartCoroutine("Event");
    }

    IEnumerator Event()
    {
        //�Ǐ�̃^���b�g�ɂ��C������`�I�ȃe�L�X�g
        ParmitNext();
        yield return null;
    }
}
