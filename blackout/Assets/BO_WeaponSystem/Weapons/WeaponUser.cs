using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WeaponUser
{
    public abstract Animator getAnim();
    public abstract string getWepUseAnimLayer();
    public abstract GameObject getAimingObj();
    public abstract void ThrowHitResponse();
}
