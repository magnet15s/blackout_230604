using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{

    
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
    /// �e�̃^�C�v�i���e�A�G�l���M�[���j�̕\����
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
