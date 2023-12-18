using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class A53_31 : Weapon
{
    public override WeaponUser sender { get; set; }

    public override string weaponName { get; set; } = "A53-31 arm machine gun";
    public override int? remainAmmo { get; set; } = 600;
    public override string remainAmmoType { get; set; } = "AMMO";
    public override float cooldownProgress { get; set; } = 1f;
    public override string cooldownMsg { get; set; } = "EMPTY AMMO";

    private bool senderInitialized = false;
    private bool ready = false;
    private bool trigger = false;

    [SerializeField] private PlayerCameraCood pcc;
    [Space]

    private Animator anim;
    private int RAAnimLayerIdx;
    [SerializeField] private float wepChangeDelay = 0.4f;
    private float wcdCnt = 0;
    public GameObject AimingObj;

    private float verticalAiming = 0.5f;

    [SerializeField] private float fireInterval = 0.07f;
    private float fireIntCnt = 0;

    private int remainMaxAmmo;
    [SerializeField] private float autoReloadInterval = 0.3f;
    private float aRIntCnt = 0;

    [SerializeField] private GameObject firePoint = null;
    [SerializeField] private float bulletInitialVelocity;
    [SerializeField] private int bulletDamage;

    [Space]
    public Material HUDImage = null;

    private float zoomCancCnt = 0;

    
    void Start() {
        HUDWeaponImage = HUDImage;
    }

    // Update is called once per frame
    void Update() {
        
        if(sender != null) {
            if (!senderInitialized) {
                anim = sender.getAnim();
                RAAnimLayerIdx = anim.GetLayerIndex(sender.getWepUseAnimLayer());
                AimingObj = sender.getAimingObj();
                remainMaxAmmo = (int)(remainAmmo ?? 0);
                senderInitialized = true;
            }
            
            //-----���r�Ǐ]--------
            if (ready) {    //---�����I�����Ă����
                if (wcdCnt < wepChangeDelay) {
                    wcdCnt += Time.deltaTime;
                    if (wcdCnt > wepChangeDelay) wcdCnt = wepChangeDelay;
                }
                if (anim.GetLayerWeight(RAAnimLayerIdx) != 1) anim.SetLayerWeight(RAAnimLayerIdx, wcdCnt / wepChangeDelay);
                
                if((verticalAiming = AimingObj.transform.eulerAngles.x) > 90) {
                    verticalAiming -= 360;
                }
                anim.SetFloat("verticalAiming", (verticalAiming /0.8f / 180) + 0.5f);

            } else {        //---�����I�����Ă��Ȃ���
                //Debug.Log(anim.GetLayerWeight(RAAnimLayerIdx));
                if(wcdCnt > 0) {
                    wcdCnt -= Time.deltaTime;
                    if (wcdCnt < 0) wcdCnt = 0;
                }
                if (anim.GetLayerWeight(RAAnimLayerIdx) != 0) anim.SetLayerWeight(RAAnimLayerIdx, wcdCnt / wepChangeDelay);
            }
            //------���r�Ǐ]-------
            //------���ˌ�---------
            if (ready)
            {
                if (trigger && remainAmmo > 0 && fireIntCnt <= 0)
                {
                    fireIntCnt = fireInterval;
                    fire();
                    trigger = false;
                }
                if (Input.GetMouseButtonDown(0)) {
                    GetComponent<AudioSource>().Play();
                }

                if (Input.GetMouseButtonUp(0)) {
                    GetComponent<AudioSource>().Stop();
                }

            }

            if(fireIntCnt > 0)
            {
                fireIntCnt -= Time.deltaTime;
                if(fireIntCnt < 0 ) fireIntCnt = 0;
            }
            //------���ˌ�---------
            //------���������U-----
            
            if(aRIntCnt <= 0)
            {
                if (remainAmmo < remainMaxAmmo)
                {
                    remainAmmo++;
                    aRIntCnt = autoReloadInterval;
                }
            }
            if (aRIntCnt > 0)
            {
                aRIntCnt -= Time.deltaTime;
                if (aRIntCnt < 0) aRIntCnt = 0;
                if (remainAmmo <= 0)
                {
                    cooldownProgress = aRIntCnt / autoReloadInterval;
                }
            }
            //------���������U-----
            //------���J�����Y�[��-
            if(zoomCancCnt > 0) pcc.zoom = true;
            else pcc.zoom = false;

            zoomCancCnt -= Time.deltaTime;
            

        }
    }

    private void fire()
    {
        remainAmmo--;
        //Debug.Log($"BANG!! {remainAmmo}");
        LiveBullet LB = LiveBullet.BulletInstantiate(this, (firePoint ?? gameObject).transform.position, AimingObj.transform.forward * bulletInitialVelocity,  bulletDamage);
        
        
    }

    public override void Ready() {
        //Debug.Log($"Ready{this}");
        ready = true;
    }
    public override void PutAway() {
        //Debug.Log($"PutAway{this}");
        ready = false;
    }
    public override void MainAction() {
        //Debug.Log($"MainAct{this}");
        if (ready) {

            trigger= true;
        }
    }
    public override void SubAction() {
        zoomCancCnt = 0.05f;

        //Debug.Log($"SubAct{this}");
    }
    public override void Reload() {
        //Debug.Log($"Reload{this}");
    }
}
