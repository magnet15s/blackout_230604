using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileCarrier : AutoGun {
    // Start is called before the first frame update
    [SerializeField] GameObject missile;
    protected override void Percussion(Transform firePos) {
        Livemissile homing;
        homing = Instantiate(missile, firePos.position, Quaternion.identity).GetComponent<Livemissile>();
        homing.Target = core.Target.transform;
    }
}
