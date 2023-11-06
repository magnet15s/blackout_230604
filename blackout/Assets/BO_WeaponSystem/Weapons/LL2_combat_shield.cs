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
    private readonly string ANIM_MOTION_PARAM = "CC_motion_number";
    private readonly int ANIM_MOTION_NUMBER = 1;
    private readonly string ANIM_FIRE_TRIGGER = "punch_fire";
    private readonly string ANIM_MOTION_RESET = "punch_reset";

    private readonly string ANIM_DEFENSE_LAYER = "upper_body";
    private readonly string ANIM_DEFENSE_PALAM = "UB_motion_number";
    private readonly int ANIM_DEFENSE_NUMBER = 1;
    private readonly string ANIM_DEFENSE_RESET = "UB_reset"; 

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

    private float defenseEnterWaitCnt = 0;
    private readonly float DEFENSE_MOTION_TRANS_TIME = 0.3f;
    [SerializeField] private float assaultSpeed = 30;

    // Start is called before the first frame update
    void Start() {
        HUDWeaponImage = HUDImage;
    }

    // Update is called once per frame
    void Update() {
        //----Å´äiì¨----

        vAiming = aimingObj.localEulerAngles.x;
        if (vAiming > 180) vAiming = -(360 - vAiming);
        vAiming = vAiming / 160 + 0.5f;
        vAiming = Mathf.Max(Mathf.Min(vAiming * 1.2f, 1), -1);
        if (ready) anim.SetFloat("verticalAiming", vAiming);
        if (robWepUsable)
        {
            if (attacking)
            {
                attackTime += Time.deltaTime;
                attackStartCnt += Time.deltaTime;

                if (attackStartCnt <= ATTACK_MOTION_TRANS_TIME) //èâåÇ
                {
                    MotionLocking = true;
                    animLayerWeight = attackStartCnt / ATTACK_MOTION_TRANS_TIME;
                    sender.SetWepMove(AssaultMove, ATTACK_DELAY + 3);
                    assaultTimeCnt = 0;
                }
                else if (attackTime < ATTACK_DELAY) //çUåÇíÜ
                {
                    MotionLocking = true;
                    if(animLayerWeight < 1)//äiì¨ÉAÉjÉÅÅ[ÉVÉáÉìÉåÉCÉÑÅ[ÇÃweightÇ™1à»â∫ÇÃä‘
                    {
                        animLayerWeight += Time.deltaTime / ATTACK_MOTION_TRANS_TIME;
                        if(animLayerWeight > 1)animLayerWeight = 1;
                    }
                    if(attackTime > ATTACK_HIT_CHK_TIMING && !attackHitChecked) {//çUåÇîªíËÇÃèuä‘
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
                            
                            sender.ThrowHitResponse(lastAttackIsRight ? rightShield : leftShield, dr.transform.gameObject);
                        }


                        attackHitChecked = true;
                    }
                    

                }
                else if (attackTime < ATTACK_DELAY + ATTACK_FOLLOW_THROUGH) //çUåÇå„ÉtÉHÉçÅ[ÉXÉãÅ[
                {
                    MotionLocking = false;
                    animLayerWeight -= Time.deltaTime / ATTACK_FOLLOW_THROUGH;
                }
                else    //çUåÇèIóπ
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

            //----Å™äiì¨----
            //----Å´ñhå‰----

            if((defenseEnterWaitCnt -= Time.deltaTime) > 0) {
                //Debug.Log($"defense{attacking}");
                if (!attacking) {
                    float lw;
                    if((lw = anim.GetLayerWeight(anim.GetLayerIndex(ANIM_DEFENSE_LAYER))) < 1) {
                        lw += Time.deltaTime / DEFENSE_MOTION_TRANS_TIME;
                        if (lw > 1) lw = 1;
                        anim.SetLayerWeight(anim.GetLayerIndex(ANIM_DEFENSE_LAYER), lw);
                    }
                }
            } else {
                float lw;
                if((lw = anim.GetLayerWeight(anim.GetLayerIndex(ANIM_DEFENSE_LAYER))) > 0) {
                    lw -= Time.deltaTime / DEFENSE_MOTION_TRANS_TIME;
                    if (lw < 0) lw = 0;
                    anim.SetLayerWeight(anim.GetLayerIndex(ANIM_DEFENSE_LAYER), lw);
                }
            }
        }
        else
        {
            //----Å´äiì¨----
            if (attacking)
            {
                AttackEnd();
            }
            float lw;
            if((lw = anim.GetLayerWeight(anim.GetLayerIndex(ANIM_MOTION_LAYER))) != 0) {
                lw -= Time.deltaTime / ATTACK_MOTION_TRANS_TIME;
                anim.SetLayerWeight(anim.GetLayerIndex(ANIM_MOTION_LAYER), lw);
            }
            //----Å™äiì¨----
            //----Å´ñhå‰----
            if(defenseEnterWaitCnt > 0) {
                defenseEnterWaitCnt = 0;
            }
        }
    }
    public override void Ready() {
        Debug.Log($"Ready{this}");
        connectShield.SetActive(false);
        leftShield.SetActive(true);
        rightShield.SetActive(true);
        ready = true;
        anim.SetInteger(ANIM_MOTION_PARAM, ANIM_MOTION_NUMBER);
        anim.SetInteger(ANIM_DEFENSE_PALAM, ANIM_DEFENSE_NUMBER);
    }

    public override void PutAway() {
        Debug.Log($"PutAway{this}");
        connectShield.SetActive(true);
        leftShield.SetActive(false);
        rightShield.SetActive(false);
        ready = false;
        anim.SetInteger(ANIM_MOTION_PARAM, -1);
        anim.SetInteger(ANIM_DEFENSE_PALAM, -1);
        anim.SetTrigger(ANIM_MOTION_RESET);
        anim.SetTrigger(ANIM_DEFENSE_RESET);
    }

    public override void MainAction() {

        if (robWepUsable = sender.RequestWepAction())  //senderÇ™çUåÇâ¬î\èÛë‘Ç≈Ç†ÇÍÇŒ
        {
            
            if(attackTime >= ATTACK_DELAY )  //ÉAÉ^ÉbÉNÉfÉBÉåÉCÇ™èIÇÌÇ¡ÇƒÇ¢ÇÍÇŒ
            {
                anim.SetTrigger(ANIM_FIRE_TRIGGER);
                if (!attacking)     //ç≈èâÇÃçUåÇÇ≈Ç†ÇÍÇŒ
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
        if(robWepUsable = sender.RequestWepAction())
        defenseEnterWaitCnt = 0.2f;
        
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

    private float assaultTimeCnt = 0;
    private bool assaultAttack = false;
    private Transform assaultTarget = null;
    private Vector3 AssaultMove(Vector3 movement, bool grounded, Transform user) { 
        if(assaultTimeCnt == 0)
        {
            assaultAttack = grounded;
            if (TrackingIcon.closestIconToCenter == null) assaultAttack = false;
            else
            {
                assaultTarget = TrackingIcon.closestIconToCenter.trackingTarget.transform;
            }
        }

        assaultTimeCnt += Time.deltaTime;

        if (assaultAttack)
        {
            Vector3 dir = user.InverseTransformPoint(assaultTarget.position);
            float dist = Mathf.Min(dir.magnitude * 1.4f + 8.0f, assaultSpeed);
            dir.Normalize();
            Vector3 ret = (Mathf.Max(1.0f - (assaultTimeCnt / ATTACK_DELAY) - -0,2f, 0) * (dir * dist)) + (Mathf.Min(assaultTimeCnt / ATTACK_DELAY + -0.2f, 1) * movement);
            Debug.Log(ret);
            ret.y = movement.y;

            if (assaultTimeCnt > ATTACK_DELAY) sender.removeWepMove(AssaultMove);
            return ret;

        }else return movement;
    }
}
