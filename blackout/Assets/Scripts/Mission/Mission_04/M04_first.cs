using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M04_first : MissionEventNode
{
    [SerializeField] Transform gateGuide;
    [SerializeField] RectTransform canvas;
    [SerializeField] GameObject player;
    [SerializeField] GameObject objectTracker;
    private TrackingIcon tIcon;

    private MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    // Start is called before the first frame update
    public override void EventFire() {

        //基地ゲートにガイドを設定
        tIcon = Instantiate(objectTracker, canvas).GetComponent<TrackingIcon>();
        tIcon.trackingTarget = gateGuide.gameObject;
        tIcon.canvas = canvas;
        tIcon.player = player;


        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        message = MessageWindow.instance;
        StartCoroutine("Event");
    }
    IEnumerator Event() {
        ParmitNext();
        yield return null;
    }
}
