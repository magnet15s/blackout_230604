using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial2EFlag : MissionEventFlag
{
    // Start is called before the first frame update
    private bool jumpTestCleared = false;
    private bool airAxelTestCleared = false;
    private bool evasionMoveTestCleared = false;
    [SerializeField] private PlayerController pc;

    private HUDMissionList mList;
    private bool subscribed = false;

    // Update is called once per frame
    private void Start() {
        mList = HUDMissionDisplay.mainDisplay.GetMissionList();
        
    }
    void Update()
    {
        if (isActive) {
            if(!subscribed)
            {
                subscribed = true;
                pc.OnJumped += Oj;

                pc.OnAirAxeled += Oaa;

                pc.OnEvasionMoved += Oem;
            }
            //�S�~�b�V���������Ď�
            if(jumpTestCleared && airAxelTestCleared && evasionMoveTestCleared) {
                Debug.Log("Test2 comp");
                OnFlagUp();
                isActive = false;
            }
        }
        else
        {
            if(subscribed)
            {
                subscribed = false;
                pc.OnJumped -= Oj;
                pc.OnAirAxeled -= Oaa;
                pc.OnEvasionMoved -= Oem;
            }
        }
    }

    void Oj()
    {
        jumpTestCleared = true;
        mList.MissionClear(3);
    }

    void Oaa()
    {
        airAxelTestCleared = true;
        mList.MissionClear(4);
    }

    void Oem() {
        evasionMoveTestCleared = true;
        mList.MissionClear(5);
    }
}
