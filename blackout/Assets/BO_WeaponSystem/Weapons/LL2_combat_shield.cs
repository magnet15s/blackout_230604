using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LL2_combat_shield : Weapon, ShieldRoot
{
    // Start is called before the first frame update
    public override WeaponUser sender { get; set; }
    
    public override string weaponName { get; set; } = "LL2 conbat shield";
    public override int? remainAmmo { get; set; } = null;
    public override string remainAmmoType { get; set; } = "";
    public override float cooldownProgress { get; set; } = 1f;
    public override string cooldownMsg { get; set; } = "WAIT";

    public Material HUDImage;

    [SerializeField] GameObject connectShield;
    [SerializeField] GameObject leftShield;
    [SerializeField] GameObject rightShield;

    // Start is called before the first frame update
    void Start() {
        HUDWeaponImage = HUDImage;
    }

    // Update is called once per frame
    void Update() {

    }
    public override void Ready() {
        Debug.Log($"Ready{this}");
        connectShield.SetActive(false);
        leftShield.SetActive(true);
        rightShield.SetActive(true);
    }

    public override void PutAway() {
        Debug.Log($"PutAway{this}");
        connectShield.SetActive(true);
        leftShield.SetActive(false);
        rightShield.SetActive(false);
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

    public void HitReceive(ShieldParts receiver, int damage, Vector3 hitPosition, GameObject source, string damageType) {
        
    }
}
