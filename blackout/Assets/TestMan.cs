using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMan : MonoBehaviour , WeaponUser 
{

    public LineRenderer line;
    float cnt = 0;
    Vector3[] trail;
    public float velocity = 100f;
    public Weapon weapon;
    // Start is called before the first frame update
    void Start()
    {
        trail = new Vector3[20];
        for(int i = 0; i < trail.Length; i++) trail[i] = Vector3.zero;
        cnt = 1;
        weapon.sender = this;
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
            Debug.Log($"{trail[0]} {trail[1]} {trail[2]} {trail[3]} {trail[4]} {trail[5]}");
            LiveBullet.BulletInstantiate(weapon, transform.position + new Vector3(0, 0,0), transform.forward * velocity, 0);
        }

    }

    public Animator getAnim() {
        throw new NotImplementedException();
    }

    public string getWepUseAnimLayer() {
        throw new NotImplementedException();
    }

    public GameObject getAimingObj() {
        throw new NotImplementedException();
    }

    public void ThrowHitResponse() {

    }
}
