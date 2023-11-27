using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class missil_launcher : Weapon
{
    public GameObject prefab;
    Transform thisTransform;

    public GameObject Target;
    public override WeaponUser sender { get; set; }

    public override string weaponName { get; set; } = "E11_GB missile launcher";
    public override int? remainAmmo { get; set; } = 120;
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

    [SerializeField] private float fireInterval = 0.3f;
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
        thisTransform = transform;
    }

    // Update is called once per frame
    void Update() {

        if (TrackingIcon.closestIconToCenter != null) {
            Target = TrackingIcon.closestIconToCenter.trackingTarget;
        }
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
                
                if (trigger == true && remainAmmo > 0 /*&& fireIntCnt == 0*/)
                {
                    Debug.Log(trigger);
                    fireIntCnt = fireInterval;
                    fire();
                    trigger = false;
                }
                
            }

            if(fireIntCnt > 0)
            {
                fireIntCnt -= Time.deltaTime;
                
            }
            if (fireIntCnt < 0) fireIntCnt = 0;
            //------↑射撃---------

            //------↓カメラズーム-
            if (zoomCancCnt > 0) pcc.zoom = true;
            else pcc.zoom = false;

            zoomCancCnt -= Time.deltaTime;
            

        }
    }

    private void fire()
    {
            remainAmmo--;
            Debug.Log($"BANG!! {remainAmmo}");
            Livemissile homing;
            homing = Instantiate(prefab, thisTransform.position, Quaternion.identity).GetComponent<Livemissile>();
            homing.Target = Target.transform;
        
        
        
    }

    public override void Ready() {
        Debug.Log($"Ready{this}");
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
