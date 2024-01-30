using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDestroyEFlag : MissionEventFlag
{
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        if (isActive) {
            Enemy.EnemyDestroy += catchDestroy;
        }
    }

    private void catchDestroy(Enemy e) {
        if(!ignited) {
            OnFlagUp();
        }
    }

    
}
