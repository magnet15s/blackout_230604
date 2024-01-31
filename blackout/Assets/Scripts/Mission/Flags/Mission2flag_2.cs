using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission2flag_2 : MissionEventFlag {
    private HUDMissionList mList;
    private int startEnemy;
    private int nowEnemy;
    // Start is called before the first frame update
    void Start()
    {
        mList = HUDMissionDisplay.mainDisplay.GetMissionList();
        startEnemy= GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(startEnemy+","+nowEnemy);
        nowEnemy = GameObject.FindGameObjectsWithTag("Enemy").Length;
        string text = "‘S‚Ä‚Ì“G‚ÌŒ‚”j"+"["+(startEnemy-nowEnemy)+"/"+startEnemy+"]";
        mList.UpdateMissionItemTextNonCal(2, text);
        if (nowEnemy == 0) {
            OnFlagUp();
        }
    }
}
