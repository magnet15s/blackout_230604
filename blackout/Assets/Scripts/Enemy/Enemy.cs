using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, DamageReceiver
{
    [SerializeField] public abstract string modelName { get; set; }
    [SerializeField] public abstract int maxArmorPoint { get; set; }
    [SerializeField] public abstract int armorPoint { get; set; }
    //[SerializeField] public abstract bool 


    public void MainFire() {
        Debug.LogWarning($"メイン兵装未実装 : {this}");
    }

    public void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType) {
        Debug.LogWarning($"ダメージ処理未実装 : {this} <- {source}[source] ");
    }

}
