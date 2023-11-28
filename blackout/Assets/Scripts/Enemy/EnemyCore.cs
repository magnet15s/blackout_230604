using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class EnemyCore : Enemy
{
    [Space(30)]
    [Header("TargetSetPhase")]
    [Space]
    public GameObject Target;
    public bool referenceSheredTarget;
    [SerializeField] private UnityEvent TargetSetPhaseFunction;

    [Space]
    [Header("TargetFindPhase")]
    [Space]
    public bool targetFound;
    private bool _targetFound;
    [SerializeField] private UnityEvent TargetFindPhaseFunction;

    [Space]
    [Header("MovePhase")]
    [Space]
    [SerializeField] private NavMeshAgent NavAgent;
    public float targetDiff;
    private float _targetDiff;
    [SerializeField, Tooltip("距離に応じて実行する処理(実行距離はMovePhaseFunctionsRangeで指定。配列長が合わない場合idx=0の処理を常に実行)")]
    private UnityEvent[] MovePhaseFunctions;
    [SerializeField, Tooltip("インデックスの対応するMovePhaseFunctionの実行距離(targetDiffが値以下の場合対応する処理を実行。-1で距離∞。値の小さい物が優先される)")]
    private float[] MovePhaseFunctionsRange;
    private int[] MoveFuncOrder;

    [Space]
    [Header("AlignPhase")]
    [Space]
    [SerializeField] private GroundedSensor gs;
    public bool grounding;
    public GameObject[] groundingObj;
    public GameObject[] horizontalAlignObj;
    public GameObject[] elevasionAlignObj;
    private Vector3[] hAliObjInitVec;
    private Vector3[] eAliObjInitVec;
    private Vector3 Alignment;
    private Vector3 AlignTarget;
    private Vector3 AimDiffByGrounding;
    public Vector3 alignDiff;
    private Vector3 _alignDiff;
    [SerializeField] private UnityEvent AlignPhaseFunction;

    [Space]
    [Header("AttackPhase")]
    [Space]
    [SerializeField] private UnityEvent AttackPhaseFunction;



    // Start is called before the first frame update
    void Start()
    {
        //MovePhaseの実行順序整理
        if(MovePhaseFunctions.Length != MovePhaseFunctionsRange.Length)
        {
            MovePhaseFunctionsRange = new float[1] { -1 };
            MoveFuncOrder = new int[1] { 0 };
        }
        else
        {
            MoveFuncOrder = new int[MovePhaseFunctionsRange.Length];
            for(int i = 0; i < MoveFuncOrder.Length; i++)
            {
                float minR = float.MaxValue;
                int minIdx = -1;

                for(int j = 0; j < MovePhaseFunctionsRange.Length; j++)
                {
                    bool isDecided = false;
                    for(int k = 0; k < i; k++) if (MoveFuncOrder[k] == j)isDecided= true;
                    if (isDecided) continue;

                    if (minR > (MovePhaseFunctionsRange[j] == -1 ? float.MaxValue * 0.99f : MovePhaseFunctionsRange[j]))
                    {
                        minR = MovePhaseFunctionsRange[j] == -1 ? float.MaxValue * 0.99f : MovePhaseFunctionsRange[j];
                        minIdx = j;
                    }
                }
                MoveFuncOrder[i] = minIdx;
            }
        }

        //Align系初期化
        hAliObjInitVec = new Vector3[horizontalAlignObj.Length];
        eAliObjInitVec = new Vector3[elevasionAlignObj.Length];

        Alignment = transform.forward;
        AlignTarget = transform.forward;


        

    }

    // Update is called once per frame
    void Update()
    {
        //ターゲットの設定
        if (TargetSetPhaseFunction != null) { TargetSetPhaseFunction.Invoke(); }
        else DefaultTargetSet();

        //ターゲットの捜索
        if (TargetFindPhaseFunction != null) { TargetFindPhaseFunction.Invoke(); }
        else DefaultTargetFind();

        //移動
        if(MovePhaseFunctions != null) { 
            foreach(int i in MoveFuncOrder)
            {
                if (MovePhaseFunctionsRange[i] > targetDiff)
                {
                    if (MovePhaseFunctions != null) MovePhaseFunctions[i].Invoke();
                    else DefaultStayMove();
                    break;
                }
            }
        }
        else
        {
            if (targetFound)
            {
                if (targetDiff < 30) DefaultRetreatMove();
                else if (targetDiff < 80) DefaultBattleMove();
                else DefaultApproachMove();
            }
            else DefaultStayMove();
            
        }

        //照準
        if (AlignPhaseFunction != null) { AlignPhaseFunction.Invoke(); }
        else DefaultAlign();

        //攻撃
        if (AttackPhaseFunction != null) { AttackPhaseFunction.Invoke(); }
        else DefaultAttack();


    }


    private void DefaultTargetSet()
    {
        if (referenceSheredTarget)
        {
            Target = Enemy.sharedTarget;
        }
    }

    private void DefaultTargetFind()
    {
        if (Target == null) return;
        Vector3 posDiff = Target.transform.position - transform.position;
        Ray ray = new Ray(transform.position, posDiff);
    }

    private void DefaultApproachMove()
    {

    }

    private void DefaultBattleMove()
    {
        
    }

    private void DefaultRetreatMove()
    {

    }

    private void DefaultStayMove()
    {

    }

    private void DefaultAlign()
    {

    }

    private void DefaultAttack()
    {

    }


    override public void Awake()
    {
        base.Awake();
    }

    private void OnDestroy()
    {
        base.OnEnemyDestroy(this);
    }
}
