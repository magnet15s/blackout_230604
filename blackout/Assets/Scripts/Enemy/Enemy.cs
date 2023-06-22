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
        Debug.LogWarning($"メイン兵装未実装 : {this}");
    }

    public virtual void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType) {
        Debug.LogWarning($"ダメージ処理未実装 : {this} <- {source}[source] ");
    }

}
