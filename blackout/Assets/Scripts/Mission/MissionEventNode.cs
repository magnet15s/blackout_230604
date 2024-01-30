using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class MissionEventNode : MonoBehaviour
{


    
    public EventHandler parmitNext;


    [Tooltip("���̃C�x���g�m�[�h���N�����邽�߂̃C�x���g�t���O"), SerializeField]
    private List<MissionEventFlag> TriggerFlags;
    [Tooltip("��ł��t���O�����Ă΃C�x���g�����s���邩"), SerializeField]
    private bool disjunctionTrigger = false;
    [Space]

    [Tooltip("���ɋN��������C�x���g�t���O"), SerializeField]
    private List<MissionEventFlag> NextFlags;

    private void Awake()
    {
        foreach(MissionEventFlag ef in TriggerFlags)
        {
            ef.onFlagUp += TryEventFire;
        }
    }

    
    private void TryEventFire(object o, EventArgs e)
    {
        if (TriggerFlags == null || TriggerFlags.Count == 0)
        {
            Debug.LogError($"[{gameObject.name}] > MissionEventNode�̃g���K�[���ݒ肳��Ă��܂���");
            return;
        }

        bool ign = !disjunctionTrigger;

        if (disjunctionTrigger)
        {
            foreach (MissionEventFlag ef in TriggerFlags) ign |= ef.ignited;
        }
        else
        {
            foreach (MissionEventFlag ef in TriggerFlags) ign &= ef.ignited;
        }
        if (ign) EventFire();
    }

    /// <summary>
    /// �C�x���g���L�q�Boverride���A�ŏ���base.Awake()�����s���邱��
    /// </summary>
    public virtual void EventFire()
    {
        foreach (MissionEventFlag ef in TriggerFlags)
        {
            ef.isActive = false;
            ef.onFlagUp -= TryEventFire;
        }
    }

    /// <summary>
    /// ���̃t���O���A�N�e�B�u��
    /// </summary>
    protected virtual void ParmitNext() {
        foreach(MissionEventFlag ef in NextFlags)
        {
            ef.isActive = true;
        }
    }

}
