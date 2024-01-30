using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class M01_first : MissionEventNode
{
    [SerializeField] private string missionTitle = "駆動系テスト";
    private string[] missionItems;
    public string viewControllerText = "マウス";
    public string moveButtonText = "W,A,S,D";
    public string dashButtonText = "移動＋左Shift";
    private string[] messageText = new string[]{
        "[キース]\nコンソール異常無し。操縦系接続確認。HUD同期確認、異常無し。",
        "[オスカー]\nテスト内容がHUD左側に表示されているはずだ。それを元に各項目のテストを遂行してくれ。",
        "[キース]\n了解。"
    };
    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    public override void EventFire() {
        missionItems = new string[] {
            $"{viewControllerText}と照準系の\n同期を確認する",
            $"{moveButtonText}と移動系の\n同期を確認する",
            $"{dashButtonText}で\nダッシュモータが\n起動する事を確認する"
        };


        mDisp = HUDMissionDisplay.mainDisplay;
        StartCoroutine("Event");
    }
    IEnumerator Event (){
        yield return new WaitForSeconds(1);
        message.function(messageText[0], 4.7f);
        yield return new WaitForSeconds(5);
        mDisp.SetMissionTitle(missionTitle);
        foreach (string s in missionItems) mDisp.GetMissionList().AddMissionItem(s);
        message.function(messageText[1], 4.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[2], 2f);
        ParmitNext();
    }

    // Start is called before the first frame update


    // Update is called once per frame

}
