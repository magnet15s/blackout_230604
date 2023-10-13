using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WeaponUser
{
    public abstract Animator getAnim();
    public abstract string getWepUseAnimLayer();
    public abstract GameObject getAimingObj();
    public abstract void ThrowHitResponse();
    public virtual void ThrowHitResponse(GameObject bullet, GameObject hitObject) {
        ThrowHitResponse();
    }
    public virtual bool RequestWepAction(ref bool usable) { return true; }
}
