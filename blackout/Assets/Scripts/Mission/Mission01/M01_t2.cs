using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M01_t2 : MissionEventNode
{
    [SerializeField] private string missionTitle = "火器管制テスト";
    private string[] messageText = new string[]{
        "[キース]\n統合姿勢制御、良好。\n各部アブソーバー異常無し。",
        "[オスカー]\n順調だな。次は火器管制系を検証する。\n安全装置は解除済みだ。各種項目のテストを開始してくれ。"
    };
    private string weaponChangeButtonText = "R,Fキー";
    private string weaponActionButtonsText = "左、右クリック";
    private string subActionButtonText = "右クリック";
    private string[] missionItems;

    private MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;


    public override void EventFire() {
        missionItems = new string[] {
            $"{weaponChangeButtonText}で\nHUD右下の武装が\n切り替わる事を確認する",
            $"{weaponActionButtonsText}で\n各装備アクションが\n起動することを確認する\n[0/2]"
        };
        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
        message = MessageWindow.instance;
        StartCoroutine("Event");
    }

    // Start is called before the first frame update


    // Update is called once per frame
    IEnumerator Event() {
        message.function(messageText[0], 3.5f);
        yield return new WaitForSeconds(3.8f);
        message.function(messageText[1], 4f);
        mList.RemoveMissionItems();
        foreach (string s in missionItems) mList.AddMissionItem(s);
        ParmitNext();


    }
}
