using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial3EFlag : MissionEventFlag {
    // Start is called before the first frame update
    private bool wepChTestCleared = false;
    private bool fireTestCleared = false;
    private bool subActTestCleared = false;
    [SerializeField] private PlayerController pc;

    private HUDMissionList mList;
    private bool subscribed = false;


    // Update is called once per frame
    private void Start() {
        mList = HUDMissionDisplay.mainDisplay.GetMissionList();
        
    }
    void Update() {
        if (isActive) {
            if (!subscribed)
            {
                subscribed = true;
                pc.OnWeaponChange += owc;

                pc.OnWeaponMainAct += owma;

                pc.OnWeaponSubAct += owsa;

            }
            //�S�~�b�V���������Ď�
            if (wepChTestCleared && subActTestCleared && fireTestCleared) {
                Debug.Log("Test2 comp");
                OnFlagUp();
                isActive = false;
            }
        }
        else
        {
            if (subscribed)
            {
                subscribed = false;
                pc.OnWeaponChange -= owc;
                pc.OnWeaponMainAct -= owma;
                pc.OnWeaponSubAct -= owsa;
            }
        }
    }

    private void owc()
    {
        wepChTestCleared = true;
        mList.MissionClear(6);
        pc.OnWeaponChange -= owc;
    }

    private void owma()
    {
        fireTestCleared = true;
        if (subActTestCleared)
        {
            mList.MissionClear(7);
            string text = mList.getMissionItemText(7);
            text = text.Substring(0, text.IndexOf("[1/2]"));
            text += "[2/2]";
            mList.UpdateMissionItemTextNonCal(7, text);
        }
        else
        {
            string text = mList.getMissionItemText(7);
            text = text.Substring(0, text.IndexOf("[0/2]"));
            text += "[1/2]";
            mList.UpdateMissionItemTextNonCal(7, text);

        }
        pc.OnWeaponMainAct -= owma;
    }

    private void owsa()
    {
        
        subActTestCleared = true;
        if (fireTestCleared)
        {
            mList.MissionClear(7);
            string text = mList.getMissionItemText(7);
            text = text.Substring(0, text.IndexOf("[1/2]"));
            text += "[2/2]";
            mList.UpdateMissionItemTextNonCal(7, text);
        }
        else
        {
            string text = mList.getMissionItemText(7);
            text = text.Substring(0, text.IndexOf("[0/2]"));
            text += "[1/2]";
            mList.UpdateMissionItemTextNonCal(7, text);

        }

        pc.OnWeaponSubAct -= owsa;
        
    }
}
