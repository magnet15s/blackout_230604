using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMan : MonoBehaviour
{
    public LineRenderer line;
    float cnt = 0;
    Vector3[] trail;
    public float velocity = 100f;
    A53_31 a5331;
    // Start is called before the first frame update
    void Start()
    {
        a5331 = gameObject.AddComponent<A53_31>();
        trail = new Vector3[20];
        for(int i = 0; i < trail.Length; i++) trail[i] = Vector3.zero;
        cnt = 1;
    }

    // Update is called once per frame
    void Update()
    {
        cnt += Time.deltaTime;
        if (cnt > 0.5) { 
            cnt = 0;
            for(int i = 0; i < trail.Length; i++)
            {
                trail[i] = LiveBullet.BullisticCalc(transform.forward * velocity, i * 0.2f);
            }
            line.SetPositions(trail);
            LiveBullet.BulletInstantiate(a5331, transform.position + new Vector3(0, 5,0), transform.forward * velocity, 0);
        }

    }
}
