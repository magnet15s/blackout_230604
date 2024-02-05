using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AutoGun : MonoBehaviour
{
    [SerializeField] protected EnemyCore core;
    [SerializeField] protected List<Transform> firePosition;
    [Space]
    [SerializeField] protected string gunName = "Big-Gun";
    [Space]
    [SerializeField] protected float firePerSec = 6;
    [SerializeField] protected float burstFireAmount = 4;
    [SerializeField] protected float fireInterval = 1.5f;
    [Space]
    [SerializeField] protected float initialVelocity = 200;
    [SerializeField] protected float fireRange = 150;
    [SerializeField] protected int damage = 10;
    /*[SerializeField] float fireAccuracyFor100 = 1;*/
    
    protected Weapon gun;
    protected bool bursting = false;

    protected void Awake() {
        if(!transform.TryGetComponent<Weapon>(out gun))
            gun = transform.AddComponent<Weapon.Conc>();
        gun.weaponName = gunName;
    }

    public void GunFire()
    {
        if (core.targetFound && core.targetDist < fireRange && !bursting) {
            StartCoroutine("TriggerOne");
        }
    }
    protected virtual void Percussion(Vector3 firePos)
    {
        LiveBullet.BulletInstantiate(gun, firePos, (core.Target.transform.position - firePos).normalized * initialVelocity, damage);
    }

    protected virtual IEnumerator TriggerOne() {
        bursting = true;
        for(int i = 0; i < burstFireAmount; i++) {
            Percussion(firePosition[i % firePosition.Count].position);
            yield return new WaitForSeconds(1 / firePerSec);
        }
        yield return new WaitForSeconds(fireInterval);
        bursting = false;

    }
}
