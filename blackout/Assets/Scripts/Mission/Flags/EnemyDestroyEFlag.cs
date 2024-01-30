using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDestroyEFlag : MissionEventFlag
{
    // Start is called before the first frame update

    bool subscribed = false;
    // Update is called once per frame
    void Update()
    {
        if (isActive) {
            if (!subscribed)
            {
                subscribed = true;
                Enemy.EnemyDestroy += catchDestroy;
            }
        }
        else
        {
            if (subscribed)
            {
                subscribed = false;
                Enemy.EnemyDestroy -= catchDestroy;
            }
        }
    }

    private void catchDestroy(Enemy e) {
        OnFlagUp();
    }

    
}
