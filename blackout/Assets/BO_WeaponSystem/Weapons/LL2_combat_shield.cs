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
    private float attackStartCnt = 0f;
    private float attackDelayCnt = 0f;
    private readonly float ATTACK_MOTION_TRANS_TIME = 0.5f;
    private readonly float ATTACK_DELAY = 0.7f;
    private readonly float ATTACK_FOLLOW_THROUGH = 1f;


    // Start is called before the first frame update
    void Start() {
        HUDWeaponImage = HUDImage;
    }

    // Update is called once per frame
    void Update() {
        Debug.Log(anim.GetLayerWeight(anim.GetLayerIndex("close_combat")));
        attackDelayCnt -= Time.deltaTime;

        if (attacking) {
            attackStartCnt += Time.deltaTime;
            attackTime += Time.deltaTime;
            if(attackStartCnt < ATTACK_MOTION_TRANS_TIME) {
                Debug.Log("amtt");
                anim.SetLayerWeight(anim.GetLayerIndex("close_combat"), Mathf.Min(attackStartCnt / ATTACK_MOTION_TRANS_TIME, 1));
            }
            if(attackTime > ATTACK_DELAY) {
                Debug.Log("ad");
                anim.SetLayerWeight(anim.GetLayerIndex("close_combat"), Mathf.Min(1 - ((attackTime - ATTACK_DELAY) / ATTACK_FOLLOW_THROUGH), 1));
            }
            if(attackTime > ATTACK_DELAY + ATTACK_FOLLOW_THROUGH) {
                attackStartCnt = 0;
                attackTime = 0;
                attackDelayCnt = 0;
                attacking = false;
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

        if (sender.RequestWepAction())  //çUåÇâ¬î\èÛë‘Ç≈Ç†ÇÍÇŒ
        {
            if (!attacking)
            {
                robWepUsable = true;    //senderÇ≈wepActionCancelÇ™î≠âŒÇµÇΩÇ∆Ç´falseÇ…Ç»ÇÈ
                attacking = true;
                attackStartCnt = 0;
            }
            if (robWepUsable)
            {
                if (attackDelayCnt <= 0) {
                    anim.SetTrigger("punch_fire");
                    attackDelayCnt = ATTACK_DELAY;
                    attackTime = 0;
                    
                }
            }
            else
            {
                attackTime = 0;
                attackStartCnt = 0;
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
