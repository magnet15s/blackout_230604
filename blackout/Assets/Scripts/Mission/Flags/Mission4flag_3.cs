using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission4flag_3 : MissionEventFlag
{
    private HUDMissionList mList;
    private int nowEnemy;
    // Start is called before the first frame update
    void Start()
    {
        mList = HUDMissionDisplay.mainDisplay.GetMissionList();
    }

    // Update is called once per frame
    void Update()
    {
        
        string text = "‘S‚Ä‚ÌŒš‘¢•¨‚ÌŒ‚”j" + "[" + (51 - nowEnemy) + "/" + 51 + "]";
        nowEnemy = GameObject.FindGameObjectsWithTag("structure").Length;
        mList.UpdateMissionItemTextNonCal(1, text);
        if (nowEnemy <= 0)
        {
            OnFlagUp();
        }
    }
}
