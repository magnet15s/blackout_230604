using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public class Conc : Weapon, WeaponUser { //とりあえずWeaponを持たせる時用のWeaponを継承させた具象クラス
        
        public class BulletHitEventOrgs : EventArgs {
            public GameObject bullet { get; private set; }
            public GameObject hitObject { get; private set; }
            public static new BulletHitEventOrgs Empty { get { return new BulletHitEventOrgs(null, null); } }
            public BulletHitEventOrgs(GameObject _bullet, GameObject _hitObject) {
                bullet = _bullet;
                hitObject = _hitObject;
            }
        }

        public event EventHandler bulletHit;

        protected virtual void OnBulletHit(BulletHitEventOrgs e) {
            bulletHit?.Invoke(this, BulletHitEventOrgs.Empty);
        }

        public override WeaponUser sender { get { return this; } set {} }
        public override string weaponName { get; set; }
        public override int? remainAmmo { get; set; }
        public override string remainAmmoType { get; set; }
        public override float cooldownProgress { get; set; }
        public override string cooldownMsg { get; set; }
        public override void Ready() { }
        public override void PutAway() { }
        public override void MainAction() { }
        public override void SubAction() { }
        public override void Reload() { }
        Animator WeaponUser.getAnim() { return null; }
        string WeaponUser.getWepUseAnimLayer() { return null; }

        GameObject WeaponUser.getAimingObj() { return null; }

        void WeaponUser.ThrowHitResponse() {
            OnBulletHit(BulletHitEventOrgs.Empty);
        }
        void WeaponUser.ThrowHitResponse(GameObject bullet, GameObject hitObject) {//bulletがコイツを叩く
            OnBulletHit(new BulletHitEventOrgs(bullet, hitObject));//そしたらイベント発火→Concにsubscribeしてる奴らのEventHandlerを実行
        }
    }


    public static Conc GetWeapon() {
        return new Conc();
    }
    
    public abstract WeaponUser sender { get; set; } 
    public void setSender(WeaponUser sender) {
        this.sender = sender;
    }

    // Start is called before the first frame update
    /// <summary>
    /// 武器の表示名
    /// </summary>
    public abstract string weaponName { get; set; } 

    /// <summary>
    /// 残弾　残弾制限のない武器の場合null
    /// </summary>
    public abstract int? remainAmmo { get; set; }

    /// <summary>
    /// 残弾のタイプ（実弾、エネルギー等）の表示名
    /// </summary>
    public abstract string remainAmmoType { get; set; }

    /// <summary>
    /// クールダウン進捗（０f～１f）リロード中等ではない場合１
    /// </summary>
    public abstract float cooldownProgress { get; set; }

    /// <summary>
    /// クールダウン時のメッセージ
    /// </summary>
    public abstract string cooldownMsg { get; set; }

    /// <summary>
    /// HUDに表示する武器画像(非必須)
    /// </summary>
    public virtual Material HUDWeaponImage { get; set; }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 武器選択時アクション
    /// </summary>
    public abstract void Ready();
    
    /// <summary>
    ///武器選択解除時アクション 
    /// </summary>
    public abstract void PutAway();
    
    /// <summary>
    /// 射撃等アクション
    /// </summary>
    public abstract void MainAction();
    
    /// <summary>
    /// 照準（ADS）時等アクション
    /// </summary>
    public abstract void SubAction();
    
    /// <summary>
    /// リロードアクション
    /// 残弾制限のない武器はreturn;のみでOK
    /// </summary>
    public abstract void Reload();

}

