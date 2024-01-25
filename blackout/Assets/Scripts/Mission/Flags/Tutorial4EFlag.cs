using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial4EFlag : MissionEventFlag {
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
                pc.OnWeaponChange += () => {
                    wepChTestCleared = true;
                    mList.MissionClear(6);
                };

                pc.OnWeaponMainAct += () => {
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
                };

                pc.OnWeaponSubAct += () => {
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
                };

            }
            //全ミッション完了監視
            if (wepChTestCleared && subActTestCleared && fireTestCleared) {
                if (!ignited) ignited = true;
                Debug.Log("Test2 comp");
                targetEventNode.EventFire();
                isActive = false;
            }
        }
    }
}
