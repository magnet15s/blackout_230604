using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesViewRangeManager : MonoBehaviour
{
    // Start is called before the first frame update

    private float oldFogEnd;
    [SerializeField] float FogendDist2ViewRangeRatio = 0.8f;
    // Update is called once per frame
    void Update()
    {
        if(RenderSettings.fogEndDistance != oldFogEnd)
        {
            oldFogEnd = RenderSettings.fogEndDistance;

            foreach (Enemy e in Enemy.EnemiesList)
            {
                if(e is EnemyCore)
                {
                    (e as EnemyCore).findRange = oldFogEnd * FogendDist2ViewRangeRatio;
                }
            }
        }
    }
}
