using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, DamageReceiver
{
    [SerializeField] public string modelName;
    [SerializeField] public int maxArmorPoint;
    [SerializeField] public int armorPoint;
    //[SerializeField] public abstract bool 


    public void MainFire() {
        Debug.LogWarning($"���C������������ : {this}");
    }

    public virtual void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType) {
        Debug.LogWarning($"�_���[�W���������� : {this} <- {source}[source] ");
    }

}
