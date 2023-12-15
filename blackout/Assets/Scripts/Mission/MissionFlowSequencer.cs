using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionFlowSequencer : MonoBehaviour
{
    [SerializeField] private MissionEventNode firstEventNode;
    [SerializeField] private MissionEventFlag[] eventFlags;
    [SerializeField] private int eventPhase = -1;
    public MissionEventNode activeEvent { get; private set; }

    [Space(20)]
    [SerializeField] private bool initSetFirstEvent = true;
    /*
    �~�b�V�����i�s�t���[
    �P   firstEventNode�����s�Ɠ�����parmitNext��subscribe�ieventNode�̓C�x���g�̓��e�����j
    �Q   eventNode�����s���I���ƁAparmitNext�����΂���̂�unsubscribe
    �R   ����eventFlag��active�ɂ��A����eventNode��targetEventNode��parmitNext��subscribe
     */

    // Start is called before the first frame update
    void Start()
    {
        if (initSetFirstEvent) {
            activeEvent = firstEventNode;
            firstEventNode.parmitNext += EventNext;
            firstEventNode.EventFire();

        } else {
            activeEvent = eventFlags[eventPhase].getTargetEventNode();
            activeEvent.parmitNext += EventNext;
            activeEvent.EventFire();
        }
        
    }

    // Update is called once per frame
    private void EventNext(object o, EventArgs e) {
        activeEvent.parmitNext -= EventNext;
        eventPhase++;
        eventFlags[eventPhase].isActive = true;
        activeEvent = eventFlags[eventPhase].getTargetEventNode();
        activeEvent.parmitNext += EventNext;
    }
}
