using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileCarrier : AutoGun {
    // Start is called before the first frame update
    [SerializeField] GameObject missile;
    protected override void Percussion(Vector3 firePos) {
        Debug.Log("misssssssssaaaaaaaa");
        Livemissile homing;
        homing = Instantiate(missile, firePos, Quaternion.identity).GetComponent<Livemissile>();
        homing.Target = core.Target.transform;
    }
}
