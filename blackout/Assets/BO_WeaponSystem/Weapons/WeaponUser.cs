using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WeaponUser {
    public abstract event EventHandler WepActionCancel;

    protected virtual void OnWepActionCancel(EventArgs e) { }
    public abstract Animator getAnim();
    public abstract string getWepUseAnimLayer();
    public abstract GameObject getAimingObj();
    public abstract void ThrowHitResponse();
    public virtual void ThrowHitResponse(GameObject bullet, GameObject hitObject) {
        ThrowHitResponse();
    }
    public virtual bool RequestWepAction() { return true; }
    public virtual void ReqMoveOverrideForWepAct(Vector3 localMovement, float time, Vector3 overrideWeight, bool groundedOnly) {
        return;
    }
    /// <summary>
    /// WeaponがUserの移動上書きをする際のコールバック関数の型
    /// </summary>
    /// <param name="movement">ローカル移動量</param>
    /// <param name="grounded">接地中か</param>
    /// <param name="user">userのtransform</param>
    /// <returns></returns>
    public delegate Vector3 MoveOverrideForWepAct(Vector3 movement, bool grounded, Transform user);
    /// <summary>
    /// Weapon側がUserの移動上書きをする際、コールバック関数を渡す窓口となる
    /// </summary>
    /// <param name="wepMove">渡す関数</param>
    /// <returns>上書きに成功した場合true</returns>
    public virtual bool SetWepMove(MoveOverrideForWepAct wepMove, float overallTime) { return false; }
    public virtual void removeWepMove(MoveOverrideForWepAct wepMove) { }
}
