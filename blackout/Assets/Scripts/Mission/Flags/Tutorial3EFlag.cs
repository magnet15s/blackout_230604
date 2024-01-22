using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial3EFlag : MissionEventFlag
{
    // Start is called before the first frame update
    private bool jumpTestCleared = false;
    private bool airAxelTestCleared = false;
    private bool evasionMoveTestCleared = false;
    [SerializeField] private PlayerController pc;

    private HUDMissionList mList;

    // Update is called once per frame
    private void Start() {
        mList = HUDMissionDisplay.mainDisplay.GetMissionList();
        pc.OnJumped += () => {
            jumpTestCleared = true;
            mList.MissionClear(3);
        };
        
        pc.OnAirAxeled += () => { 
            airAxelTestCleared = true;
            mList.MissionClear(4);
        };

        pc.OnEvasionMoved += () => {
            evasionMoveTestCleared = true;
            mList.MissionClear(5);
        };
    }
    void Update()
    {
        if (isActive) {
            //�S�~�b�V���������Ď�
            if(jumpTestCleared && airAxelTestCleared && evasionMoveTestCleared) {
                if (!ignited) ignited = true;
                Debug.Log("Test2 comp");
                targetEventNode.EventFire();
                isActive = false;
            }
        }
    }
}
