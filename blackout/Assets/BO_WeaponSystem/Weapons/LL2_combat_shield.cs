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

    private readonly string ANIM_MOTION_LAYER = "close_combat";
    private readonly string ANIM_FIRE_TRIGGER = "punch_fire";
    private readonly string ANIM_MOTION_RESET = "punch_reset";

    private Animator anim;
    private bool ready = false;
    private Transform aimingObj;
    public float vAiming;
    private bool robWepUsable = false;
    private bool attacking = false;
    private float attackTime = 0f;
    private float attackStartCnt = 0f;
    private float animLayerWeight = 0f;
    private readonly float ATTACK_MOTION_TRANS_TIME = 0.5f;
    private readonly float ATTACK_DELAY = 0.7f;
    private readonly float ATTACK_FOLLOW_THROUGH = 1f;


    // Start is called before the first frame update
    void Start() {
        HUDWeaponImage = HUDImage;
    }

    // Update is called once per frame
    void Update() {

        vAiming = aimingObj.localEulerAngles.x;
        if (vAiming > 180) vAiming = -(360 - vAiming);
        vAiming = vAiming / 160 + 0.5f;
        if (ready) anim.SetFloat("verticalAiming", vAiming);

        if (anim.GetFloat("verticalAiming") < 0.1) Debug.Log("aaaaaaaa");
        if (robWepUsable)
        {
            if (attacking)
            {
                attackTime += Time.deltaTime;
                attackStartCnt += Time.deltaTime;

                if (attackStartCnt <= ATTACK_MOTION_TRANS_TIME) //初撃
                {
                    MotionLocking = true;
                    animLayerWeight = attackStartCnt / ATTACK_MOTION_TRANS_TIME;
                }
                else if (attackTime < ATTACK_DELAY) //攻撃中
                {
                    MotionLocking = true;
                    if(animLayerWeight < 1)
                    {
                        animLayerWeight += Time.deltaTime / ATTACK_MOTION_TRANS_TIME;
                        if(animLayerWeight > 1)animLayerWeight = 1;
                    }
                    

                }
                else if (attackTime < ATTACK_DELAY + ATTACK_FOLLOW_THROUGH) //攻撃後フォロースルー
                {
                    MotionLocking = false;
                    animLayerWeight -= Time.deltaTime / ATTACK_FOLLOW_THROUGH;
                }
                else    //攻撃終了
                {
                    AttackEnd();
                    animLayerWeight = 0;
                }
                
                anim.SetLayerWeight(anim.GetLayerIndex(ANIM_MOTION_LAYER), animLayerWeight);
            }
            else 
            {
                attackTime = ATTACK_DELAY;
            }



        }
        else
        {
            if (attacking)
            {
                AttackEnd();
            }
            float lw;
            if((lw = anim.GetLayerWeight(anim.GetLayerIndex(ANIM_MOTION_LAYER))) != 0) {
                lw -= Time.deltaTime / ATTACK_MOTION_TRANS_TIME;
                anim.SetLayerWeight(anim.GetLayerIndex(ANIM_MOTION_LAYER), lw);
            }
        }
    }
    public override void Ready() {
        Debug.Log($"Ready{this}");
        connectShield.SetActive(false);
        leftShield.SetActive(true);
        rightShield.SetActive(true);
        ready = true;
    }

    public override void PutAway() {
        Debug.Log($"PutAway{this}");
        connectShield.SetActive(true);
        leftShield.SetActive(false);
        rightShield.SetActive(false);
        ready = false;
    }

    public override void MainAction() {

        

        if (robWepUsable = sender.RequestWepAction())  //senderが攻撃可能状態であれば
        {
            
            if(attackTime >= ATTACK_DELAY )  //アタックディレイが終わっていれば
            {
                anim.SetTrigger(ANIM_FIRE_TRIGGER);
                if (!attacking)     //最初の攻撃であれば
                {
                    attackTime = 0;
                    attackStartCnt = 0;
                    attacking = true;
                }
                else
                {
                    attackTime = 0;
                }
                
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
        aimingObj = sender.getAimingObj().transform;
        sender.WepActionCancel += (_,_) => { robWepUsable = false; };
    }

    private void AttackEnd()
    {
        attacking = false;
        attackTime = ATTACK_DELAY + ATTACK_FOLLOW_THROUGH;
        attackStartCnt = 0;
        animLayerWeight = 0;
        anim.SetTrigger(ANIM_MOTION_RESET);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(rightShield.transform.position + rightShield.transform.forward * 1.5f, 0.5f);
    }
}
