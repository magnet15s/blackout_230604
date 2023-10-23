using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] private int damage = 200;

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
    private bool attackHitChecked = false;
    private float attackTime = 0f;
    private float attackStartCnt = 0f;
    public bool lastAttackIsRight = false;
    private float animLayerWeight = 0f;
    private readonly float ATTACK_MOTION_TRANS_TIME = 0.5f;
    private readonly float ATTACK_DELAY = 0.7f;
    private readonly float ATTACK_FOLLOW_THROUGH = 1f;
    private readonly float ATTACK_HIT_CHK_TIMING = 0.5f;


    // Start is called before the first frame update
    void Start() {
        HUDWeaponImage = HUDImage;
    }

    // Update is called once per frame
    void Update() {

        vAiming = aimingObj.localEulerAngles.x;
        if (vAiming > 180) vAiming = -(360 - vAiming);
        vAiming = vAiming / 160 + 0.5f;
        vAiming = Mathf.Max(Mathf.Min(vAiming * 1.2f, 1), -1);
        if (ready) anim.SetFloat("verticalAiming", vAiming);

        if (anim.GetFloat("verticalAiming") < 0.1) Debug.Log("aaaaaaaa");
        if (robWepUsable)
        {
            if (attacking)
            {
                attackTime += Time.deltaTime;
                attackStartCnt += Time.deltaTime;

                if (attackStartCnt <= ATTACK_MOTION_TRANS_TIME) //����
                {
                    MotionLocking = true;
                    animLayerWeight = attackStartCnt / ATTACK_MOTION_TRANS_TIME;
                }
                else if (attackTime < ATTACK_DELAY) //�U����
                {
                    MotionLocking = true;
                    if(animLayerWeight < 1)//�i���A�j���[�V�������C���[��weight��1�ȉ��̊�
                    {
                        animLayerWeight += Time.deltaTime / ATTACK_MOTION_TRANS_TIME;
                        if(animLayerWeight > 1)animLayerWeight = 1;
                    }
                    if(attackTime > ATTACK_HIT_CHK_TIMING && !attackHitChecked) {//�U������̏u��
                        RaycastHit[] results = 
                        Physics.SphereCastAll(lastAttackIsRight ? rightShield.transform.position : leftShield.transform.position,
                            2f, lastAttackIsRight ? rightShield.transform.forward : leftShield.transform.forward,
                            2f
                        );
                        List<RaycastHit> drList = new();
                        foreach(RaycastHit res in results) {
                            DamageReceiver dr;
                            if((dr = res.transform.GetComponent<DamageReceiver>()) != null) {
                                drList.Add(res);
                            }
                        }
                        foreach(RaycastHit dr in drList) {
                            dr.transform.GetComponent<DamageReceiver>().Damage(damage, dr.point, gameObject, "ArmStrike");
                        }


                        attackHitChecked = true;
                    }
                    

                }
                else if (attackTime < ATTACK_DELAY + ATTACK_FOLLOW_THROUGH) //�U����t�H���[�X���[
                {
                    MotionLocking = false;
                    animLayerWeight -= Time.deltaTime / ATTACK_FOLLOW_THROUGH;
                }
                else    //�U���I��
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

        

        if (robWepUsable = sender.RequestWepAction())  //sender���U���\��Ԃł����
        {
            
            if(attackTime >= ATTACK_DELAY )  //�A�^�b�N�f�B���C���I����Ă����
            {
                anim.SetTrigger(ANIM_FIRE_TRIGGER);
                if (!attacking)     //�ŏ��̍U���ł����
                {
                    attackTime = 0;
                    attackStartCnt = 0;
                    attacking = true;
                    attackHitChecked = false;
                    lastAttackIsRight = true;
                }
                else
                {
                    attackTime = 0;
                    attackHitChecked = false;
                    lastAttackIsRight = !lastAttackIsRight;
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
        lastAttackIsRight = false;
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
