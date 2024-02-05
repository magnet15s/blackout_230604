using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigGun : AutoGun
{
    protected override void Percussion(Vector3 firePos) {
        LiveHeavyBullet.HeavyBulletInstantiate(gun, firePos, (core.Target.transform.position - firePos).normalized * initialVelocity, damage);
    }

}
