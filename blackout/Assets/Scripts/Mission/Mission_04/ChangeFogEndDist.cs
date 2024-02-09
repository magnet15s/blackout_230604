using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFogEndDist : MissionEventNode {
    [SerializeField] float fogEndDist = 90;
    [SerializeField] float fogChangePersec = 20;

    
    public override void EventFire() {
        StartCoroutine("Event");
        
    }
    IEnumerator Event() {
        while(Mathf.Abs(RenderSettings.fogEndDistance - fogEndDist) > 1) {
            float oldfed = RenderSettings.fogEndDistance;
            float diff = fogEndDist - oldfed;

            RenderSettings.fogEndDistance = oldfed +  Mathf.Min(Mathf.Abs(diff), fogChangePersec / 5) * Mathf.Sign(diff);
            yield return new WaitForSeconds(0.2f);
        }
        
    }
    
}
