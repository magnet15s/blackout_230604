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
    /// 武器の表示名
    /// </summary>
    public abstract string weaponName { get; set; } 

    /// <summary>
    /// 残弾　残弾制限のない武器の場合null
    /// </summary>
    public abstract int? remainAmmo { get; set; }

    /// <summary>
    /// 弾のタイプ（実弾、エネルギー等）の表示名
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
