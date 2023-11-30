using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class DamageEventArgs : EventArgs
{
    public int damage;
    public Vector3 hitPosition;
    public GameObject source;
    public string damageType;

    public DamageEventArgs(int damage, Vector3 hitPosition, GameObject source, string damageType){
        this.damage = damage;
        this.hitPosition = hitPosition;
        this.source = source;
        this.damageType = damageType;
    }
}

public class EnemyCore : Enemy
{
    [Space(30)]
    [Header("TargetSetPhase")]
    [Space]
    public GameObject Target;
    public bool referenceSheredTarget;
    /// <summary>
    /// ターゲットの決定をするフェーズで呼ばれる処理
    /// 設定しない場合EnemyCore.DefaultTargetSet()が呼ばれる
    /// </summary>
    [SerializeField, Tooltip("ターゲットの決定をするフェーズで呼ばれる処理　設定しない場合EnemyCore.DefaultTargetSet()が呼ばれる")] 
    private UnityEvent TargetSetPhaseFunction;

    [Space]
    [Header("TargetFindPhase")]
    [Space]
    public bool targetFound;
    private bool _targetFound;
    [SerializeField] private float findRange;
    [SerializeField, Tooltip("ターゲットが目視出来ているかを判断する処理　設定しない場合EnemyCore.DefaultTargetFind()が呼ばれる")] 
    private UnityEvent TargetFindPhaseFunction;

    [Space]
    [Header("MovePhase")]
    [Space]
    [SerializeField] private NavMeshAgent navAgent;
    public float targetDist;
    private float _targetDist;
    [SerializeField, Tooltip("距離に応じて実行する処理(実行距離はMovePhaseFunctionsRangeで指定。配列長が合わない場合idx=0の処理を常に実行)　配列が空の場合デフォルトの行動パターンが設定される")]
    private UnityEvent[] MovePhaseFunctions;
    [SerializeField, Tooltip("インデックスの対応するMovePhaseFunctionの実行距離(targetDistが値以下の場合対応する処理を実行。-1で距離∞。値の小さい物が優先される)")]
    private float[] MovePhaseFunctionsRange;
    private int[] MoveFuncOrder;
    [SerializeField, Tooltip("移動を行わない時の待機処理　デフォルトの移動が実行不能だった場合、フォールバック的に呼ばれる事もある")]
    private UnityEvent MoveStayFunctions;

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
    /// <summary>
    /// ターゲットが目視出来ているかを判断する処理
    /// 設定しない場合EnemyCore.DefaultTargetFind()が呼ばれる
    /// </summary>
    [SerializeField] private UnityEvent AlignPhaseFunction;

    [Space]
    [Header("AttackPhase")]
    [Space]
    [SerializeField] private UnityEvent AttackPhaseFunction;

    [Space]
    [Header("Other")]
    [Space]

    [SerializeField, Tooltip("ダメージを受けた時の処理　引数としてDamageEventArgsを持つ関数のみ設定可能")]
    private UnityEvent<DamageEventArgs> DamageFunction; 


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
        if (TargetSetPhaseFunction != null) { TargetSetPhaseFunction?.Invoke(); }
        else DefaultTargetSet();

        //ターゲットの捜索
        if (TargetFindPhaseFunction != null) { TargetFindPhaseFunction?.Invoke(); }
        else DefaultTargetFind();

        //移動
        if(MovePhaseFunctions != null) { 
            foreach(int i in MoveFuncOrder)
            {
                if (MovePhaseFunctionsRange[i] > targetDist)
                {
                    if (MovePhaseFunctions != null) MovePhaseFunctions[i].Invoke();
                    else DefaultStayMove();
                    break;
                }
            }
        }
        else
        {
            if (_targetFound || (referenceSheredTarget && Enemy.sharedTargetPosition != null))
            {
                if (targetDist < 30) DefaultRetreatMove();
                else if (targetDist < 80) DefaultBattleMove();
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

    public override void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType)
    {
        if (DamageFunction != null) DamageFunction.Invoke(new DamageEventArgs(damage, hitPosition, source, damageType));
        else DefaultDamage(new DamageEventArgs(damage, hitPosition, source, damageType));
    }  


    /// <summary>
    /// EnemyCoreのデフォルトのターゲット決定処理
    /// referenceSharedTargetがtrueの場合Enemy.SheredTargetをターゲットに設定する
    /// </summary>
    public void DefaultTargetSet()
    {
        if (referenceSheredTarget)
        {
            Target = Enemy.sharedTarget;
        }
    }

    /// <summary>
    /// EnemyCoreのデフォルトのターゲット認識処理
    /// ターゲットの位置までレイを飛ばし、途中に視界を遮る物が無いかを調べTargetFoundに反映する
    /// </summary>
    public void DefaultTargetFind()
    {
        if (Target == null) {
            targetFound = false;
            _targetFound = false;
            return;
        }
        //プレイヤーの方向にレイを飛ばす
        Vector3 posDiff = Target.transform.position - transform.position;
        _targetDist = posDiff.magnitude;
        if(_targetDist > findRange) {
            targetDist = _targetDist;
            _targetFound = false;
            targetFound = false;
            return;
        }
        Ray ray = new Ray(transform.position, posDiff);

        RaycastHit[] results = new RaycastHit[20];
        int len = Physics.RaycastNonAlloc(ray, results, targetDist);
        //リザルトに視界を塞ぐ物があるか調べる
        bool blocked = false;

        for(int i = 0; i < len; i++)
        {
            RaycastHit res = results[i];
            
            //リザルトが自分orターゲットの場合無視して次のリザルトへ
            if (res.transform == transform　|| res.transform == Target.transform) continue;

            //親を遡って自分orターゲットに当たるか
            Transform p = res.transform;
            while (p != null && !p.Equals(Target.transform) && !p.Equals(this.transform)) {
                //Debug.Log($"p = {p} : parent = {p.parent}  {p.parent == Target.transform}");
                if(p.parent == null) break;
                else p = p.parent;
            }

            if (p.Equals(Target.transform) || p.Equals(this.transform)) {
                continue;
            } else {
                blocked = true; break;
            }
        }
        _targetFound = !blocked;

        //公開用public変数の更新
        targetFound = _targetFound;
        targetDist = _targetDist;
    }


    /// <summary>
    /// EnemyCoreのデフォルトの移動処理の内の接近処理
    /// デフォルトではtargetDistが80以上の時呼ばれる
    /// </summary>
    public void DefaultApproachMove()
    {
        if(navAgent == null)
        {
            Debug.LogError("[EnemyCore.DefaultApproachMove] > navAgentがセットされていません　デフォルト接近処理を利用する場合はnavAgentをセットしてください");
            DefaultStayMove();
            return;
        }
        if(Target == null)
        {
            Debug.LogError("[EnemyCore.DefaultApproachMove] > Targetがnullです");
            DefaultStayMove();
            return;
        }

        if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid) {
            if(Target != null && targetFound) {
                Debug.Log("うごくぜ！");
                navAgent.destination = Target.transform.position;
            } else if(referenceSheredTarget && Enemy.sharedTargetPosition != null) {
                navAgent.destination = (Vector3)sharedTargetPosition;
            }
        }

    }


    /// <summary>
    /// EnemyCoreのデフォルトの移動処理の内の戦闘機動処理
    /// デフォルトではtargetDistが30以上80未満の時呼ばれる
    /// </summary>
    public void DefaultBattleMove()
    {
        if(navAgent == null)
        {
            Debug.LogError("[EnemyCore.DefaultBattleMove] > navAgentがセットされていません　デフォルト戦闘機動処理を利用する場合はnavAgentをセットしてください");
            DefaultStayMove();
            return;
        }
        if (Target == null)
        {
            Debug.LogError("[EnemyCore.DefaultBattleMove] > Targetがnullです");
            DefaultStayMove();
            return;
        }
    }


    /// <summary>
    /// EnemyCoreのデフォルトの移動処理の内の後退処理
    /// デフォルトではtargetDistが30未満の時呼ばれる
    /// </summary>
    public void DefaultRetreatMove()
    {
        if(navAgent == null) {
            Debug.LogError("[EnemyCore.DefaultRetreatMove] > navAgentがセットされていません　デフォルト後退処理を利用する場合はnavAgentをセットしてください");
            DefaultStayMove();
            return;
        }
        if (Target == null)
        {
            Debug.LogError("[EnemyCore.DefaultRetreatMove] > Targetがnullです");
            DefaultStayMove();
            return;
        }

    }


    /// <summary>
    /// EnemyCoreのデフォルトの移動処理のうちの待機処理
    /// 他のデフォルト移動処理が実行不能（エラー等）な時にも呼ばれる
    /// </summary>
    public void DefaultStayMove()
    {
        if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid) {
            navAgent.destination = transform.position;
        }
    }

    public void DefaultAlign()
    {

    }

    public void DefaultAttack()
    {

    }

    public void DefaultDamage(DamageEventArgs e)
    {
        base.Damage(e.damage, e.hitPosition, e.source, e.damageType);
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
