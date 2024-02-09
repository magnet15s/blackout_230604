using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M03_first : MissionEventNode
{
    // Start is called before the first frame update
    private string[] missionItems;
    private string[] messageText = new string[]{
        "[オスカー少佐]\nミッション開始。",
        "[オスカー少佐]\n今回の作戦は敵基地攻略の布石だ。\n輸送中の無人機へ奇襲をかけて、打撃を与える。",
"[オスカー少佐]\n敵部隊はこの地のどこかにいる。\nまずは捜索から始めるんだ。",
        "[キース]\n了解。"
    };
    [SerializeField] private string missionTitle = "接近";
    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    public override void EventFire()
    {
        missionItems = new string[] {
            $"敵の部隊に接近する"
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
    IEnumerator Event()
    {
        yield return new WaitForSeconds(1);
        message.function(messageText[0], 2.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[1], 4.7f);
        yield return new WaitForSeconds(5);
        message.function(messageText[2], 3.7f);
        mDisp.SetMissionTitle(missionTitle);
        foreach (string s in missionItems) mDisp.GetMissionList().AddMissionItem(s);
        yield return new WaitForSeconds(5);
        message.function(messageText[3], 1.5f);
        ParmitNext();

    }
}
