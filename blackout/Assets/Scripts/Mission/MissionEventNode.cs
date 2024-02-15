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

    [Tooltip("���̃C�x���g�m�[�h���N�����邽�߂̃C�x���g�t���O"), SerializeField]
    private List<MissionEventFlag> triggerFlags;
    [Tooltip("��ł��t���O�����Ă΃C�x���g�����s���邩"), SerializeField]
    private bool disjunctionTrigger = false;
    [Space]

    [Tooltip("���ɋN��������C�x���g�t���O"), SerializeField]
    private List<MissionEventFlag> nextFlags;

    

    public static List<MissionEventNode> nodeList = new();

    /// <summary>
    /// �p�����Abase.Awake()���ŏ��Ɏ��s���邱��
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
            Debug.LogError($"[{gameObject.name}] > MissionEventNode�̃g���K�[���ݒ肳��Ă��܂���");
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
    /// �C�x���g���L�q�B
    /// </summary>
    public abstract void EventFire();

    /// <summary>
    /// ���̃t���O���A�N�e�B�u��
    /// </summary>
    protected virtual void ParmitNext() {
        foreach(MissionEventFlag ef in nextFlags)
        {
            ef.isActive = true;
        }
    }

}
