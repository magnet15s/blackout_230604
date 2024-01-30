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

    /// <summary>
    /// 継承時、base.Awake()を最初に実行すること
    /// </summary>
    private void Awake()
    {
        foreach(MissionEventFlag ef in TriggerFlags)
        {
            ef.onFlagUp += TryEventFire;
            //Debug.Log(ef.gameObject.name);
        }
    }

    
    private void TryEventFire(object o, EventArgs e)
    {
        (o as MissionEventFlag).isActive = false;
        (o as MissionEventFlag).onFlagUp -= TryEventFire;


        if (TriggerFlags == null || TriggerFlags.Count == 0)
        {
            Debug.LogError($"[{gameObject.name}] > MissionEventNodeのトリガーが設定されていません");
            return;
        }

        bool ign = !disjunctionTrigger;

        if (disjunctionTrigger)
        {
            foreach (MissionEventFlag item in TriggerFlags) ign |= item.ignited;
        }
        else
        {
            foreach (MissionEventFlag item in TriggerFlags) ign &= item.ignited;
        }
        if (ign) EventFire();
    }

    /// <summary>
    /// イベントを記述。
    /// </summary>
    public abstract void EventFire();

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
