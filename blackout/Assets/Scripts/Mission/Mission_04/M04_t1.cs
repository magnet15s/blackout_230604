using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M04_t1 : MissionEventNode
{

    [SerializeField] Transform gateGuide;
    [SerializeField] Focus3rdCam f3c;
    private string[] missionItems;
    private string[] messageText = new string[]{
        "[キース]\n目標地点に到着。攻撃を開始する。",
        "[オスカー大佐]\n防壁上のタレットは脅威になる。\n必要に応じて排除しろ。"
    };
    [SerializeField] private string missionTitle = "破壊";
    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    public override void EventFire()
    {
        //基地ゲートのガイドを削除
        f3c.SetForcusTarget(null);
        Destroy(gateGuide.gameObject);
        missionItems = new string[] {
            $"建造物の破壊"
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
