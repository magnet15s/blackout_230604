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

    [Tooltip("���Y����̎ˌ����s����")]
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

    //�ˌ��P��������
    protected virtual void Percussion(Transform firePos)
    {
        LiveBullet.BulletInstantiate(gun, firePos.position, firePos.forward * initialVelocity, damage);
    }

    /// <summary>
    /// 1�g���K�[���i��A�̃o�[�X�g�ˌ��Ǘ��j
    /// �R���[�`����p����
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator TriggerOne() {
        bursting = true;
        //look&fire�x��
        yield return new WaitForSeconds(lookAndFireDeley);

        //�o�[�X�g
        for(int i = 0; i < burstFireAmount; i++) {

            //firePosition�̃C�e���[�^��i�܂��ˌ�

            if (!firePosEnum.MoveNext()) {
                firePosEnum.Reset();
                firePosEnum.MoveNext();
            }
            Percussion(firePosEnum.Current);

            //���˃��[�g���ҋ@
            yield return new WaitForSeconds(1 / firePerSec);
        }

        //�ˌ��Ԋu���ҋ@
        //�i�����look&fire�ɂ��x�����������Ă����j
        yield return new WaitForSeconds(Mathf.Max(0, fireInterval - lookAndFireDeley));
        bursting = false;

    }
}
