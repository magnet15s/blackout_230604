using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AutoGun : MonoBehaviour
{
    [SerializeField] protected EnemyCore core;
    [SerializeField] protected List<Transform> firePosition;
    [Space]
    [SerializeField] protected string gunName = "Big-Gun";
    [Space]
    [SerializeField] protected float firePerSec = 6;
    [SerializeField] protected float burstFireAmount = 4;
    [SerializeField] protected float fireInterval = 1.5f;
    [SerializeField] protected float lookAndFireDeley = 1f;
    [Space]
    [SerializeField] protected float initialVelocity = 200;
    [SerializeField] protected bool useCoreFindRange = true;
    [SerializeField] protected float fireRange = 150;
    [SerializeField] protected int damage = 10;
    /*[SerializeField] float fireAccuracyFor100 = 1;*/
    
    protected Weapon gun;
    protected bool bursting = false;
    protected IEnumerator<Transform> firePosEnum;
    protected void Awake() {
        if(!transform.TryGetComponent<Weapon>(out gun))
            gun = transform.AddComponent<Weapon.Conc>();
        gun.weaponName = gunName;
        if (firePosition != null && firePosition.Count != 0)
            firePosEnum = firePosition.GetEnumerator();
        else firePosEnum = null;
        
    }

    [Tooltip("当該武器の射撃実行処理")]
    public void GunFire()
    {
        if(firePosEnum == null) {
            Debug.LogWarning("firePosition is null");
            return;
        }
        if (core.targetFound && core.targetDist < (useCoreFindRange ? core.findRange : fireRange) && !bursting) {
            StartCoroutine("TriggerOne");
        }
    }

    //射撃１発分処理
    protected virtual void Percussion(Transform firePos)
    {
        LiveBullet.BulletInstantiate(gun, firePos.position, firePos.forward * initialVelocity, damage);
    }

    /// <summary>
    /// 1トリガー分（一連のバースト射撃管理）
    /// コルーチンを用いて
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator TriggerOne() {
        bursting = true;
        //look&fire遅延
        yield return new WaitForSeconds(lookAndFireDeley);

        //バースト
        for(int i = 0; i < burstFireAmount; i++) {

            //firePositionのイテレータを進ませつつ射撃

            if (!firePosEnum.MoveNext()) {
                firePosEnum.Reset();
                firePosEnum.MoveNext();
            }
            Percussion(firePosEnum.Current);

            //発射レート分待機
            yield return new WaitForSeconds(1 / firePerSec);
        }

        //射撃間隔分待機
        //（次回のlook&fireによる遅延分を引いておく）
        yield return new WaitForSeconds(Mathf.Max(0, fireInterval - lookAndFireDeley));
        bursting = false;

    }
}
