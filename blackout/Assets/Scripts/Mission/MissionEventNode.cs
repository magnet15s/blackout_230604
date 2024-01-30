using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class MissionEventNode : MonoBehaviour
{


    
    public EventHandler parmitNext;


    [Tooltip("このイベントノードが起動するためのイベントフラグ"), SerializeField]
    private List<MissionEventFlag> TriggerFlags;
    [Tooltip("一つでもフラグが立てばイベントを実行するか"), SerializeField]
    private bool disjunctionTrigger = false;
    [Space]

    [Tooltip("次に起動させるイベントフラグ"), SerializeField]
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
            Debug.LogError($"[{gameObject.name}] > MissionEventNodeのトリガーが設定されていません");
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
    /// イベントを記述。override時、最初にbase.Awake()を実行すること
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
    /// 次のフラグをアクティブ化
    /// </summary>
    protected virtual void ParmitNext() {
        foreach(MissionEventFlag ef in NextFlags)
        {
            ef.isActive = true;
        }
    }

}
