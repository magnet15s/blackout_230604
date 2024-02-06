using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigGun : AutoGun
{
    protected override void Percussion(Transform firePos) {
        LiveHeavyBullet.HeavyBulletInstantiate(gun, firePos.position, firePos.forward * initialVelocity, damage);
    }

}
