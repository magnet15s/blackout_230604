using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M01_t1 : MissionEventNode
{
    [SerializeField] private string missionTitle = "機動テスト";
    private string[] messageText = new string[]{
        "[キース]\n駆動系、問題無し。",
        "[オスカー]\nテスト項目を更新した。次のテストに移ってくれ。"
    };
    private string jumpButtonText = "Space";
    private string airAxelButtonText = "Space";
    private string evationMoveButtonText = "ジャンプ準備中に\n再度Space";
    private string[] missionItems;

    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;


    public override void EventFire() {
        missionItems = new string[] {
            $"{jumpButtonText}でジャンプが\n発動する事を確認する",
            $"空中で{airAxelButtonText}で\nエアアクセルが\n動作する事を確認する",
            $"{evationMoveButtonText}で\n緊急回避機動が\n発動する事を確認する"
        };
        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        StartCoroutine("Event");
        parmitNext?.Invoke(this, EventArgs.Empty);
    }

    // Start is called before the first frame update


    // Update is called once per frame
    IEnumerator Event() {
        message.function(messageText[0], 3.5f);
        yield return new WaitForSeconds(3.8f);
        message.function(messageText[1], 4f);
        mList.RemoveMissionItems();
        foreach (string s in missionItems) mList.AddMissionItem(s);
        parmitNext?.Invoke(this, EventArgs.Empty);


    }
}
