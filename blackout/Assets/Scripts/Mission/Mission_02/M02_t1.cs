using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M02_t1 : MissionEventNode {
    // Start is called before the first frame update
    private string[] missionItems;
    private string[] messageText = new string[]{
        "[キース]\n目標地点付近に到着。掃討を開始する"
    };
    [SerializeField] private string missionTitle = "制圧";
    [SerializeField] MessageWindow message;
    private HUDMissionDisplay mDisp;
    private HUDMissionList mList;
    public override void EventFire() {
        missionItems = new string[] {
            $"全ての敵の撃破"
        };
        mDisp = HUDMissionDisplay.mainDisplay;
        mList = mDisp.GetMissionList();
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
        mList.RemoveMissionItems();
        foreach (string s in missionItems) mList.AddMissionItem(s);
        message.function(messageText[0], 4.7f);
        ParmitNext();

    }
}
