using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A53_31 : Weapon
{
    public override string weaponName { get; set; } = "A53-31 arm machine gun";
    public override int? remainAmmo { get; set; } = 60;
    public override string remainAmmoType { get; set; } = "AMMO";
    public override float cooldownProgress { get; set; } = 1f;
    public override string cooldownMsg { get; set; } = "EMPTY AMMO";

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    public override void Ready() {
        Debug.Log($"Ready{this}");
    }

    public override void PutAway() {
        Debug.Log($"PutAway{this}");
    }

    public override void MainAction() {
        Debug.Log($"MainAct{this}");
    }
    public override void SubAction() {
        Debug.Log($"SubAct{this}");
    }
    public override void Reload() {
        Debug.Log($"Reload{this}");
    }
}
