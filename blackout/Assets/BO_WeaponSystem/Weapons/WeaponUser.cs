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
    /// Weapon��User�̈ړ��㏑��������ۂ̃R�[���o�b�N�֐��̌^
    /// </summary>
    /// <param name="movement">���[�J���ړ���</param>
    /// <param name="grounded">�ڒn����</param>
    /// <param name="user">user��transform</param>
    /// <returns></returns>
    public delegate Vector3 MoveOverrideForWepAct(Vector3 movement, bool grounded, Transform user);
    /// <summary>
    /// Weapon����User�̈ړ��㏑��������ہA�R�[���o�b�N�֐���n�������ƂȂ�
    /// </summary>
    /// <param name="wepMove">�n���֐�</param>
    /// <returns>�㏑���ɐ��������ꍇtrue</returns>
    public virtual bool SetWepMove(MoveOverrideForWepAct wepMove, float overallTime) { return false; }
    public virtual void removeWepMove(MoveOverrideForWepAct wepMove) { }
}
