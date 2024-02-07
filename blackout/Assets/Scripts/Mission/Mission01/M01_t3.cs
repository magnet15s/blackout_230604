using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M01_t3 : MissionEventNode
{
    [SerializeField] private string missionTitle = "火器管制テスト";
    private string[] messageText = new string[]{
        "[キース]\n火器操作異常なし。",
        "[オスカー少佐]\nでは実際に模擬目標の撃破をしてもらおう",
        "[オスカー少佐]\nHUD上に目標の位置を表示している。すべて撃破しろ"
    };
    [SerializeField] private GameObject trackingIcon;
    [SerializeField] private List<GameObject> target;
    [SerializeField] private Transform canvas;
    [SerializeField] private Transform player;
    private string[] missionItems;
    

    private MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;


    public override void EventFire() {
        missionItems = new string[] {
            $"模擬目標をすべて\n撃破する[0/3]"
        };
        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        message = MessageWindow.instance;
        StartCoroutine("Event");
    // Start is called before the first frame update
    }

    // Update is called once per frame
    IEnumerator Event() {
        message.function(messageText[0], 1.5f);
        yield return new WaitForSeconds(1.8f);
        message.function(messageText[1], 2f);
        mList.RemoveMissionItems();
        foreach (GameObject o in target) {
            if(o) o.SetActive(true);
        }

        foreach (GameObject o in target) {
            if (!o) continue;
            TrackingIcon icon = Instantiate(trackingIcon, canvas).GetComponent<TrackingIcon>();
            icon.trackingTarget = o;
            icon.canvas = canvas.GetComponent<RectTransform>();
            icon.player = player.gameObject;
        }
        yield return new WaitForSeconds(2.3f);
        ParmitNext();
        yield break;

    }
}
