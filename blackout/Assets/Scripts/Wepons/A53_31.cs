using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A53_31 : Weapon
{
    public override WeaponUser sender { get; set; }

    public override string weaponName { get; set; } = "A53-31 arm machine gun";
    public override int? remainAmmo { get; set; } = 60;
    public override string remainAmmoType { get; set; } = "AMMO";
    public override float cooldownProgress { get; set; } = 1f;
    public override string cooldownMsg { get; set; } = "EMPTY AMMO";

    private bool senderInitialized = false;
    private bool ready = false;
    private bool lastReady = false;
    private bool fire = false;

    private Animator anim;
    private int RAAnimLayerIdx;
    [SerializeField] private float wepChangeDelay = 0.4f;
    private float wcdCnt = 0;
    private GameObject weaponObject;

    private float verticalAiming = 0.5f;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        
        if(sender != null) {
            if (!senderInitialized) {
                anim = sender.getAnim();
                RAAnimLayerIdx = anim.GetLayerIndex(sender.getWepUseAnimLayer());
                weaponObject = sender.getWeaponObject();
            }
            
            if (ready) {    //---ïêäÌÇëIëÇµÇƒÇ¢ÇÈä‘
                //-----Å´òrí«è]--------
                if (wcdCnt < wepChangeDelay) {
                    wcdCnt += Time.deltaTime;
                    if (wcdCnt > wepChangeDelay) wcdCnt = wepChangeDelay;
                }
                if (anim.GetLayerWeight(RAAnimLayerIdx) != 1) anim.SetLayerWeight(RAAnimLayerIdx, wcdCnt / wepChangeDelay);
                
                if((verticalAiming = weaponObject.transform.eulerAngles.x) > 90) {
                    verticalAiming -= 360;
                }
                anim.SetFloat("verticalAiming", (verticalAiming /0.8f / 180) + 0.5f);
                //------Å™òrí«è]-------

            } else {        //---ïêäÌÇëIëÇµÇƒÇ¢Ç»Ç¢ä‘
                //-----Å´òrí«è]--------
                Debug.Log(anim.GetLayerWeight(RAAnimLayerIdx));
                if(wcdCnt > 0) {
                    wcdCnt -= Time.deltaTime;
                    if (wcdCnt < 0) wcdCnt = 0;
                }
                if (anim.GetLayerWeight(RAAnimLayerIdx) != 0) anim.SetLayerWeight(RAAnimLayerIdx, wcdCnt / wepChangeDelay);
                //------Å™òrí«è]-------
            }
        }
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
        Debug.Log($"MainAct{this}");
    }
    public override void SubAction() {
        Debug.Log($"SubAct{this}");
    }
    public override void Reload() {
        Debug.Log($"Reload{this}");
    }
}
