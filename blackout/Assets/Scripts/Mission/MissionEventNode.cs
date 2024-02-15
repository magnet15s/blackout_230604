using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public abstract class MissionEventNode : MonoBehaviour
{
    public static bool missionsAllCleared = false;
    protected static float missionStartTime;

    [Tooltip("このイベントノードが起動するためのイベントフラグ"), SerializeField]
    private List<MissionEventFlag> triggerFlags;
    [Tooltip("一つでもフラグが立てばイベントを実行するか"), SerializeField]
    private bool disjunctionTrigger = false;
    [Space]

    [Tooltip("次に起動させるイベントフラグ"), SerializeField]
    private List<MissionEventFlag> nextFlags;

    

    public static List<MissionEventNode> nodeList = new();

    /// <summary>
    /// 継承時、base.Awake()を最初に実行すること
    /// </summary>
    protected virtual void Awake()
    {
        missionStartTime = Time.time;
        if(missionsAllCleared)missionsAllCleared = false;
        nodeList.Add(this);

        foreach(MissionEventFlag ef in triggerFlags)
        {
            ef.onFlagUp += TryEventFire;
            //Debug.Log(ef.gameObject.name);
        }
    }

    protected void AllMissionClear() {
        missionsAllCleared = true;
        MissionScoreData.lastMissionDatum = new MissionScoreData.ClearDatum(
            SceneManager.GetActiveScene().name,
            PlayerController.instance.armorPoint,
            PlayerController.instance.maxArmorPoint,
            Time.time - missionStartTime
        );
        nodeList.Clear();
    }

    
    private void TryEventFire(object o, EventArgs e)
    {
        (o as MissionEventFlag).isActive = false;
        (o as MissionEventFlag).onFlagUp -= TryEventFire;


        if (triggerFlags == null || triggerFlags.Count == 0)
        {
            Debug.LogError($"[{gameObject.name}] > MissionEventNodeのトリガーが設定されていません");
            return;
        }

        bool ign = !disjunctionTrigger;

        if (disjunctionTrigger)
        {
            foreach (MissionEventFlag item in triggerFlags) ign |= item.ignited;
        }
        else
        {
            foreach (MissionEventFlag item in triggerFlags) ign &= item.ignited;
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
        foreach(MissionEventFlag ef in nextFlags)
        {
            ef.isActive = true;
        }
    }

}
