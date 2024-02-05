using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGun : MonoBehaviour
{
    [SerializeField] EnemyCore core;
    [SerializeField] Transform firePosition;
    [Space]
    [SerializeField] float firePerSec = 6;
    [SerializeField] float burstFireAmount = 4;
    [SerializeField] float fireInterval = 1.5f;
    [Space]
    [SerializeField] float initialVelocity = 200;
    [SerializeField] int damage = 10;
    [SerializeField] float fireAccuracyFor100 = 1;

    public void GunFire()
    {

    }
    private void Percussion()
    {

    }
}
