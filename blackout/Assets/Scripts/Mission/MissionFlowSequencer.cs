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
    ミッション進行フロー
    １   firstEventNodeを実行と同時にparmitNextにsubscribe（eventNodeはイベントの内容を持つ）
    ２   eventNodeが実行し終わると、parmitNextが発火するのでunsubscribe
    ３   次のeventFlagをactiveにし、そのeventNodeのtargetEventNodeのparmitNextにsubscribe
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
