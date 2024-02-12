using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission4flag_3 : MissionEventFlag
{
    private int nowEnemy;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        nowEnemy = GameObject.FindGameObjectsWithTag("structure").Length;
        if (nowEnemy <= 11)
        {
            OnFlagUp();
        }
    }
}
