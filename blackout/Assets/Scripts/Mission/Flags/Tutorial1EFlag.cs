using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial2EFlag : MissionEventFlag
{
    // Start is called before the first frame update
    private bool moveTestCleared;
    private bool alignTestCleared;
    private bool dashTestCleared;
    [SerializeField] private PlayerController pc;

    HUDMissionList mList;

    // Update is called once per frame
    private void Start() {
        mList = HUDMissionDisplay.mainDisplay.GetMissionList();
    }
    void Update()
    {
        if (isActive) {
            
            
            //視点移動ミッション監視
            if (!alignTestCleared) {
                if (pc.aligning > 0.5) {
                       alignTestCleared = true;
                    mList.MissionClear(0);
                }
            }
            
            //移動ミッション監視
            if (!moveTestCleared) {
                if(pc.moving > 0.5) {
                    moveTestCleared = true;
                    mList.MissionClear(1);
                }
            }

            //移動ミッション監視
            if (!dashTestCleared) {
                if (pc.dashing) {
                    dashTestCleared = true;
                    mList.MissionClear(2);
                }
            }

            //全ミッション完了監視
            if(alignTestCleared && moveTestCleared && dashTestCleared) {
                if (!ignited) ignited = true;
                Debug.Log("Test1 comp");
                targetEventNode.EventFire();
                isActive = false;

            }
        }
    }
}
