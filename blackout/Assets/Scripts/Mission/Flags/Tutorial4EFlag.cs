using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial4EFlag : MissionEventFlag
{
    // Start is called before the first frame update
    private int targetKillCount = 0;

    private bool subscribed = false;
    private HUDMissionList mList;
    string text;
    // Update is called once per frame
    private void Start() {
        mList = HUDMissionDisplay.mainDisplay.GetMissionList();
        
    }
    private void EnemyKillCount(Enemy e) {
        targetKillCount++;
        mList.UpdateMissionItemTextNonCal(8, $"{text}[{targetKillCount}/3]");
        if(targetKillCount >= 3) {
            Enemy.EnemyDestroy -= EnemyKillCount;
            mList.MissionClear(8);
            OnFlagUp();
        }
    }

    

    void Update()
    {
        if (isActive) {
            if(!subscribed)
            {
                Enemy.EnemyDestroy += EnemyKillCount;
                subscribed = true;
                text = mList.getMissionItemText(8);
                text = text.Substring(0,text.IndexOf('['));

            }

        }
    }
}
