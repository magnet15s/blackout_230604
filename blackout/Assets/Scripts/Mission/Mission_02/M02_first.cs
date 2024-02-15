using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

public class M02_first : MissionEventNode {
    // Start is called before the first frame update
    public float starttime;
    private string[] missionItems;
    private string[] messageText = new string[]{
        "[オペレーター:ライン]\n目標座標に接近、ターゲット誘導開始。\n観測レベル3。連結、オールグリーン。",
        "[オスカー少佐]\n目標のサーバー棟は、巡回NMUによって\n常時警戒態勢が敷かれている。",
　　　　"[オスカー少佐]\n今回の作戦では、\n少尉が周囲のNMUの掃討後、解析ドローンがデータサーバー棟へ突入する。",
        "[オスカー少佐]\n君の働きが最も重要だ。\n期待している。",
        "[キース]\n了解。"
    };
    [SerializeField] private string missionTitle = "接近";
    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    public override void EventFire() {
        starttime = Time.time;
        missionItems = new string[] {
            $"敵データサーバー棟に\n接近する"
        };
        mDisp = HUDMissionDisplay.mainDisplay;
        StartCoroutine("Event");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Event() {
        yield return new WaitForSeconds(1);
        message.function(messageText[0], 4.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[1], 4.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[2], 6f);
        mDisp.SetMissionTitle(missionTitle);
        foreach (string s in missionItems) mDisp.GetMissionList().AddMissionItem(s);
        yield return new WaitForSeconds(6);
        message.function(messageText[3], 3.7f);
        yield return new WaitForSeconds(3);
        message.function(messageText[4], 1.5f);
        ParmitNext();

    }
}
