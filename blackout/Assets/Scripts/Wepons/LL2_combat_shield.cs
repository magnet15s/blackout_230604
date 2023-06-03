using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LL2_combat_shield : Weapon
{
    // Start is called before the first frame update
    public override string weaponName { get; set; } = "LL2 conbat shield";
    public override int? remainAmmo { get; set; } = null;
    public override string remainAmmoType { get; set; } = "";
    public override float cooldownProgress { get; set; } = 1f;
    public override string cooldownMsg { get; set; } = "WAIT";

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
