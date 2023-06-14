using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float bulletDamage;

    /*[SerializeField] private ParticleSystem ps1;
    [SerializeField] private ParticleSystem ps2;*/
    // Start is called before the first frame update
    void Start() {
        /*if(ps1 == null)
        {
            try
            {
                ps1 = Instantiate(((GameObject)Resources.Load("BO_WeaponSystem/Particles/ElectricalSparksEffect"))).GetComponent<ParticleSystem>();
            }
            catch
            {
                Debug.LogError("particle system catch fail : BO_WeaponSystem/Particles/ElectricalSparkEffects");
            }
        }
        if (ps1 == null)
        {
            try
            {
                ps2 = Instantiate(((GameObject)Resources.Load("BO_WeaponSystem/Particles/MuzzleFlash"))).GetComponent<ParticleSystem>();
            }
            catch
            {
                Debug.LogError("particle system catch fail : BO_WeaponSystem/Particles/MuzzleFlash");
            }
        }*/
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
            
            //-----↓腕追従--------
            if (ready) {    //---武器を選択している間
                if (wcdCnt < wepChangeDelay) {
                    wcdCnt += Time.deltaTime;
                    if (wcdCnt > wepChangeDelay) wcdCnt = wepChangeDelay;
                }
                if (anim.GetLayerWeight(RAAnimLayerIdx) != 1) anim.SetLayerWeight(RAAnimLayerIdx, wcdCnt / wepChangeDelay);
                
                if((verticalAiming = AimingObj.transform.eulerAngles.x) > 90) {
                    verticalAiming -= 360;
                }
                anim.SetFloat("verticalAiming", (verticalAiming /0.8f / 180) + 0.5f);

            } else {        //---武器を選択していない間
                //Debug.Log(anim.GetLayerWeight(RAAnimLayerIdx));
                if(wcdCnt > 0) {
                    wcdCnt -= Time.deltaTime;
                    if (wcdCnt < 0) wcdCnt = 0;
                }
                if (anim.GetLayerWeight(RAAnimLayerIdx) != 0) anim.SetLayerWeight(RAAnimLayerIdx, wcdCnt / wepChangeDelay);
            }
            //------↑腕追従-------
            //------↓射撃---------
            if (ready)
            {
                if (trigger && remainAmmo > 0 && fireIntCnt <= 0)
                {
                    fireIntCnt = fireInterval;
                    fire();
                    trigger = false;
                }
                
            }

            if(fireIntCnt > 0)
            {
                fireIntCnt -= Time.deltaTime;
                if(fireIntCnt < 0 ) fireIntCnt = 0;
            }
            //------↑射撃---------
            //------↓自動装填-----
            
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
            

        }
    }

    private void fire()
    {
        remainAmmo--;
        Debug.Log($"BANG!! {remainAmmo}");
        LiveBullet LB = LiveBullet.BulletInstantiate(this, (firePoint ?? gameObject).transform.position, AimingObj.transform.forward * bulletInitialVelocity,  bulletDamage);
        /*ps1.Play();
        ps2.Play();*/
    }

    public override void Ready() {
        Debug.Log($"Ready{this}");
        ready = true;
    }
    public override void PutAway() {
        Debug.Log($"PutAway{this}");
        ready = false;
    }
    public override void MainAction() {
        //Debug.Log($"MainAct{this}");
        trigger= true;
    }
    public override void SubAction() {
        Debug.Log($"SubAct{this}");
    }
    public override void Reload() {
        Debug.Log($"Reload{this}");
    }
}
