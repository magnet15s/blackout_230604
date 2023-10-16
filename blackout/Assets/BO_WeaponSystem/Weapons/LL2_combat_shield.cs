using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LL2_combat_shield : Weapon, ShieldRoot
{
    public PlayerController pc;
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

    private Animator anim;
    private bool robWepUsable = false;
    private bool attacking = false;
    private float attackTime = 0f;
    private float attackDelayCnt = 0f;
    private readonly float ATTACK_MOTION_TRANS_TIME = 0.5f;
    private readonly float ATTACK_DELAY = 0.7f;


    // Start is called before the first frame update
    void Start() {
        HUDWeaponImage = HUDImage;
    }

    // Update is called once per frame
    void Update() {
        attackDelayCnt -= Time.deltaTime;
        if (attacking) {
            attackTime += Time.deltaTime;
            if(attackTime < ATTACK_MOTION_TRANS_TIME) {
                anim.SetLayerWeight(anim.GetLayerIndex("close_combat"), attackTime / ATTACK_MOTION_TRANS_TIME);
            }
        }
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

        if (sender.RequestWepAction())  //UŒ‚‰Â”\ó‘Ô‚Å‚ ‚ê‚Î
        {
            if (!attacking)
            {
                robWepUsable = true;    //sender‚ÅwepActionCancel‚ª”­‰Î‚µ‚½‚Æ‚«false‚É‚È‚é
                attacking = true;
            }
            if (robWepUsable)
            {
                if (attackDelayCnt <= 0) {
                    anim.SetTrigger("punch_fire");
                    attackDelayCnt = ATTACK_DELAY;
                }
            }
            else
            {
                attackTime = 0;
                attacking = false;
            }
        }
        
    }
    public override void SubAction() {
        Debug.Log($"SubAct{this}");
    }
    public override void Reload() {
        Debug.Log($"Reload{this}");
    }

    public void HitReceive(ShieldParts receiver, int damage, Vector3 hitPosition, GameObject source, string damageType) {
        
    }
    public override void setSender(WeaponUser sender) {
        base.setSender(sender);
        anim = sender.getAnim();
        sender.WepActionCancel += (_,_) => { robWepUsable = false; };
    }
}
