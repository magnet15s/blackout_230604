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
        "[オスカー少佐]\nミッション開始。",
        "[オスカー少佐]\n今回の作戦の目標は、西岸を衛る無人機連隊の駐留基地。\nこれに損害を与える事だ。",
"[オスカー少佐]\n高い防壁によって地上からの進入口は限られている。\nHUD上の目標地点へ向かって進め。",
"[オスカー少佐]\n目標はあくまで内部設備の破壊だ。\n霧に乗じて接近する以上、無理に敵に構う必要はない。",
        "[キース]\n了解。基地への接近を開始する。"
    };
    [SerializeField] private string missionTitle = "敵基地へ接近";
    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    // Start is called before the first frame update
    public override void EventFire() {

        //基地ゲートにガイドを設定
        tIcon = Instantiate(objectTracker, canvas).GetComponent<TrackingIcon>();
        tIcon.trackingTarget = gateGuide.gameObject;
        tIcon.canvas = canvas;
        tIcon.player = player;

        f3c.SetForcusTarget(gateGuide);
        missionItems = new string[] {
            $"敵の基地に接近する"
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
