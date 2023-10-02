using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public class Conc : Weapon, WeaponUser { //�Ƃ肠����Weapon���������鎞�p��Weapon���p����������ۃN���X
        
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
        void WeaponUser.ThrowHitResponse(GameObject bullet, GameObject hitObject) {//bullet���R�C�c��@��
            OnBulletHit(new BulletHitEventOrgs(bullet, hitObject));//��������C�x���g���΁�Conc��subscribe���Ă�z���EventHandler�����s
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
    /// ����̕\����
    /// </summary>
    public abstract string weaponName { get; set; } 

    /// <summary>
    /// �c�e�@�c�e�����̂Ȃ�����̏ꍇnull
    /// </summary>
    public abstract int? remainAmmo { get; set; }

    /// <summary>
    /// �c�e�̃^�C�v�i���e�A�G�l���M�[���j�̕\����
    /// </summary>
    public abstract string remainAmmoType { get; set; }

    /// <summary>
    /// �N�[���_�E���i���i�Of�`�Pf�j�����[�h�����ł͂Ȃ��ꍇ�P
    /// </summary>
    public abstract float cooldownProgress { get; set; }

    /// <summary>
    /// �N�[���_�E�����̃��b�Z�[�W
    /// </summary>
    public abstract string cooldownMsg { get; set; }

    /// <summary>
    /// HUD�ɕ\�����镐��摜(��K�{)
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
    /// ����I�����A�N�V����
    /// </summary>
    public abstract void Ready();
    
    /// <summary>
    ///����I���������A�N�V���� 
    /// </summary>
    public abstract void PutAway();
    
    /// <summary>
    /// �ˌ����A�N�V����
    /// </summary>
    public abstract void MainAction();
    
    /// <summary>
    /// �Ə��iADS�j�����A�N�V����
    /// </summary>
    public abstract void SubAction();
    
    /// <summary>
    /// �����[�h�A�N�V����
    /// �c�e�����̂Ȃ������return;�݂̂�OK
    /// </summary>
    public abstract void Reload();

}

