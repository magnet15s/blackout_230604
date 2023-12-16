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
    [Space(10)]
    [Tooltip("trueの間各フェーズを繰り返す")]public bool alive　= true;
    [Space(20)]
    [SerializeField] private bool useDefaultFuncForEmptyPhase;
    [Space(20)]
    [Header("-----TargetSetPhase-----")]
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
    [Header("-----TargetFindPhase-----")]
    [Space]
    public bool targetFound;            //外部参照用
    private bool _targetFound;
    public Vector3 lastFoundPosition;   //外部参照用
    private Vector3 _lastFoundPosition;
    public float targetDist;            //外部参照用
    private float _targetDist;          
    [SerializeField] private float findRange = 200;
    public bool targetShare = false;
    [SerializeField, Tooltip("ターゲットが目視出来ているかを判断する処理　設定しない場合EnemyCore.DefaultTargetFind()が呼ばれる")] 
    private UnityEvent TargetFindPhaseFunction;

    [Space]
    [Header("-----MovePhase-----")]
    [Space]
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField, Tooltip("距離に応じて実行する処理(実行距離はMovePhaseFunctionsRangeで指定。配列長が合わない場合idx=0の処理を常に実行)　配列が空の場合デフォルトの行動パターンが設定される")]
    private UnityEvent[] MovePhaseFunctions;
    [SerializeField, Tooltip("インデックスの対応するMovePhaseFunctionの実行距離(targetDistが値以下の場合対応する処理を実行。-1で距離∞。値の小さい物が優先される)")]
    private float[] MovePhaseFunctionsRange;
    private int[] MoveFuncOrder;
    [SerializeField, Tooltip("移動を行わない時の待機処理　デフォルトの移動が実行不能だった場合、フォールバック的に呼ばれる事もある")]
    private UnityEvent MoveStayFunctions;

    [Space]
    [Header("-----AlignPhase-----")]
    [Space]
    [SerializeField] private GroundedSensor gs;
    [Tooltip("姿勢を地形に沿わせるか　gs及びnavAgantが必要")]public bool grounding;
    [Tooltip("地形に沿わせるオブジェクト（子を含む）")] public GameObject[] groundingObj;
    [Tooltip("水平照準（y軸回転）を行うオブジェクト（子を含む）（再生途中で追加しないこと）")] public GameObject[] horizontalAlignObj;
    public float leftAlignLimit = 180;
    public float rightAlignLimit = 180;
    private float[] hInitY;
    [Tooltip("垂直照準（x軸回転）を行うオブジェクト（子を含む）（再生途中で追加しないこと）")] public GameObject[] elevasionAlignObj;
    public float elevasionLimit = 30;
    public float depressionLimit = 30;
    private float[] eInitX;
    private Quaternion[] groundingInitRot;
    private Quaternion[] hInitAli;
    private Quaternion[] eInitAli;
    public float alignmentSpeed = 100;
    [SerializeField, Tooltip("姿勢を地形に沿わせたり、水平照準、府仰角照準を行う処理。")]
    private UnityEvent AlignPhaseFunction;

    [Space]
    [Header("-----AttackPhase-----")]
    [Space]
    [SerializeField] private UnityEvent AttackPhaseFunction;
    private Weapon defWep;

    [Space]
    [Header("-----Other-----")]
    [Space]

    [SerializeField, Tooltip("ダメージを受けた時の処理 (引数：int damage, Vector3 hitPosition, GameObject source, string damageType)")]
    private UnityEvent<int,Vector3,GameObject,string> DamageFunction;
    [SerializeField, Tooltip("ダメージを受けてarmorPointが0になったときの処理（enemyCore.InvokeDestroy()で他スクリプトから実行可能）")]
    private UnityEvent DestroyFunction;


    // Start is called before the first frame update
    void Start()
    {
        defWep = transform.AddComponent<Weapon.Conc>();

        //MovePhaseの実行順序整理
        if (MovePhaseFunctions.Length != MovePhaseFunctionsRange.Length)
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
        groundingInitRot = new Quaternion[groundingObj.Length];
        hInitAli = new Quaternion[horizontalAlignObj.Length];
        eInitAli = new Quaternion[elevasionAlignObj.Length];

        for (int i = 0; i < groundingInitRot.Length; i++) groundingInitRot[i] = groundingObj[i].transform.rotation;
        for (int i = 0; i < hInitAli.Length; i++) hInitAli[i] = Quaternion.Inverse(horizontalAlignObj[i].transform.parent.rotation) * horizontalAlignObj[i].transform.rotation;
        for (int i = 0; i < eInitAli.Length; i++) eInitAli[i] = Quaternion.Inverse(elevasionAlignObj[i].transform.parent.rotation) * elevasionAlignObj[i].transform.rotation ;

        hInitY = new float[hInitAli.Length];
        eInitX = new float[eInitAli.Length];

        for(int i = 0; i < hInitY.Length; i++) hInitY[i] = horizontalAlignObj[i].transform.localEulerAngles.y;
        for(int i = 0; i < eInitX.Length; i++) eInitX[i] = elevasionAlignObj[i].transform.localEulerAngles.x;


        if (navAgent != null)
        {
        }
        else
        {
            Debug.Log("[EnemyCore.DefaultAlign] > navMeshがnullです");
        }
        


        

    }

    // Update is called once per frame
    void Update()
    {
        if (alive) {

            //ターゲットの設定
            if (TargetSetPhaseFunction.GetPersistentEventCount() >= 1) { TargetSetPhaseFunction?.Invoke(); } else if (useDefaultFuncForEmptyPhase)
                DefaultTargetSet();

            //ターゲットの捜索
            if (TargetFindPhaseFunction.GetPersistentEventCount() >= 1) { TargetFindPhaseFunction?.Invoke(); } else if (useDefaultFuncForEmptyPhase)
                DefaultTargetFind();

            //移動
            if (MovePhaseFunctions.Length >= 1) {
                foreach (int i in MoveFuncOrder) {
                    if (MovePhaseFunctionsRange[i] > targetDist || MovePhaseFunctionsRange[i] == -1) {
                        if (MovePhaseFunctions[i].GetPersistentEventCount() >= 1) MovePhaseFunctions[i].Invoke();
                        else DefaultStayMove();
                        break;
                    } else DefaultStayMove();
                }
            } else if (useDefaultFuncForEmptyPhase) {
                if (targetFound || (referenceSheredTarget && Enemy.sharedTargetPosition != null)) {
                    if (targetDist < 30) DefaultRetreatMove();
                    else if (targetDist < 80) DefaultBattleMove();
                    else DefaultApproachMove();
                } else DefaultStayMove();

            }

            //照準
            if (AlignPhaseFunction.GetPersistentEventCount() >= 1) { AlignPhaseFunction.Invoke(); } else if (useDefaultFuncForEmptyPhase)
                DefaultAlign();

            //攻撃
            if (AttackPhaseFunction.GetPersistentEventCount() >= 1) { AttackPhaseFunction.Invoke(); } else if (useDefaultFuncForEmptyPhase)
                DefaultAttack();


        }
    }

    public override void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType)
    {
        if (DamageFunction.GetPersistentEventCount() >= 1) DamageFunction.Invoke(damage, hitPosition, source, damageType);
        else if(useDefaultFuncForEmptyPhase) DefaultDamage(damage, hitPosition, source, damageType);
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
    /// 任意の座標まで視線が通るかを返す
    /// </summary>
    /// <param name="point">座標（ワールド空間）</param>
    /// <param name="ignoreObject">視線を遮らないオブジェクト</param>
    /// <returns></returns>
    public bool VisibleThisToPoint(Vector3 targetPoint, float range, Transform[] ignoreObject) {
        Vector3 posDiff = targetPoint - transform.position;
        if (posDiff.magnitude > range) {
            return false;
        }
        Ray ray = new Ray(transform.position, posDiff);
        RaycastHit[] results = new RaycastHit[20];
        int len = Physics.RaycastNonAlloc(ray, results, targetDist);
        //リザルトに視界を塞ぐ物があるか調べる
        bool blocked = false;
        for (int i = 0; i < len; i++) {
            RaycastHit res = results[i];

            //リザルトがignoreListに当たるか
            bool isIgnoreObj = false;
            foreach (Transform io in ignoreObject) {
                if (io.Equals(res.transform)) {
                    isIgnoreObj = true; break;
                }
            }
            if (isIgnoreObj) continue;

            //親を遡ってignoreOnjectに当たるか
            Transform p = res.transform;
            while(!(p.parent == null)) {
                foreach(Transform io in ignoreObject) {
                    if (io.Equals(p.parent)) { isIgnoreObj = true; break; }
                }
                if (isIgnoreObj) break;
                else p = p.parent;
            }

            if (isIgnoreObj) continue;
            else {
                blocked = true;
                break;
            }
        }
        return !blocked;
    }


    /// <summary>
    /// EnemyCoreのデフォルトのターゲット認識処理
    /// ターゲットの位置までレイを飛ばし、途中に視界を遮る物が無いかを調べTargetFoundに反映する
    /// (自身とターゲット間に20個以上のコライダーが存在する場合正常に動作しない可能性がある)
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
                blocked = true; 
                break;
            }
        }
        //結果を格納
        _targetFound = !blocked;
        if (_targetFound)
        {
            _lastFoundPosition = Target.transform.position;
            lastFoundPosition = _lastFoundPosition;
            if (targetShare) {
                Enemy.sharedTargetPosition = lastFoundPosition;
                Enemy.targetReporter = this;
            }

        }

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
            if(targetFound) {
                navAgent.destination = Target.transform.position;
            } else if(referenceSheredTarget && Enemy.sharedTargetPosition != null) {
                if(!VisibleThisToPoint((Vector3)Enemy.sharedTargetPosition, findRange, new Transform[] { this.transform, Target.transform }))
                    navAgent.destination = (Vector3)sharedTargetPosition;
            }else if(lastFoundPosition != new Vector3(0,0,0)) {
                if (!VisibleThisToPoint(Target.transform.position, findRange, new Transform[]{ this.transform, Target.transform })) {
                    navAgent.destination = lastFoundPosition;
                }
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

        //右回りor左回りを決定
        //自分から見てターゲットが左右どちらにいるか取得
        Vector3 tPosOnLocSpc = transform.InverseTransformPoint(Target.transform.position);

       
        //移動方向を決定
        if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            if(tPosOnLocSpc.x > 0)
                navAgent.destination = transform.position + (Vector3.Cross(Target.transform.position - transform.position, Vector3.up).normalized);
            else
                navAgent.destination = transform.position + (-Vector3.Cross(Target.transform.position - transform.position, Vector3.up).normalized);

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
        if(navAgent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            navAgent.destination = transform.position - ((Target.transform.position - transform.position).normalized);
        }

    }
    public Transform deb1;
    public Transform deb2;

    /// <summary>
    /// EnemyCoreのデフォルトの移動処理のうちの待機処理
    /// 他のデフォルト移動処理が実行不能（エラー等）な時にも呼ばれる
    /// </summary>
    public void DefaultStayMove()
    {
        if (navAgent == null) return;
        if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid) {
            navAgent.destination = transform.position;
        }
    }

    public void DefaultAlign()
    {
        if(Target == null)
        {
            Debug.LogError("[EnemyCore.DefaultAlign] > Targetがnullです");
            return;
        }

        //grounding
        if (grounding)
        {
            if (gs == null)
            {
                Debug.LogWarning("[EnemyCore.DefaultAlign] > gsがnullです groundingは実行できません");
            }
            else
            {
                Vector3 gNormal;
                if(navAgent == null)
                {
                    Debug.LogWarning("[EnemyCore.DefaultAlign] > navMeshがnullです groundingは実行できません");
                }
                if(gs.isGrounded(out gNormal))
                {
                    //向く方向を決定
                    


                    Vector3 oldForward = -navAgent.transform.forward;
                    Vector3 oldUp = navAgent.transform.up;
                    Vector3 newRight = Vector3.Cross(oldForward, gNormal).normalized;
                    Vector3 newForward = Vector3.Cross(newRight, gNormal).normalized;

                    //navAgent.transformとのギャップ
                    Debug.DrawRay(transform.position, navAgent.transform.up, Color.gray);
                    Debug.DrawRay(transform.position, navAgent.transform.forward, Color.gray);
                    Debug.DrawRay(transform.position, navAgent.transform.right, Color.gray);

                    Debug.DrawRay(transform.position, gNormal, Color.green);
                    Debug.DrawRay(transform.position, newRight, Color.red);
                    Debug.DrawRay(transform.position, newForward, Color.blue);

                    //回転を生成
                    Quaternion groundingRot = Quaternion.LookRotation(newForward, gNormal);
                    //Quaternion diffRot = Quaternion.Inverse(navAgent.transform.rotation) * groundingRot;
                    
                    for(int i = 0; i < groundingObj.Length; i++)
                    {
                        Quaternion or = groundingObj[i].transform.rotation;
                        groundingObj[i].transform.rotation = Quaternion.Lerp(or, groundingRot, Time.deltaTime * 10);

                    }
                }

                //horizontal align
                if(targetFound) {
                    for(int i = 0; i < horizontalAlignObj.Length; i++) {
                        Transform hObj = horizontalAlignObj[i].transform;
                        Vector3 oldAng = hObj.localEulerAngles;

                        //計算の為一旦初期回転分を解消
                        hObj.rotation *= Quaternion.Inverse(hInitAli[i]);
                        Vector3 objFw = hObj.forward;
                        Vector3 td = hObj.InverseTransformPoint(Target.transform.position).normalized;
                        td.y = 0;
                        float diffAng = Mathf.Min(Vector3.Angle(objFw, hObj.TransformDirection(td)), alignmentSpeed * Time.deltaTime);

                        if (td.x < 0)
                        {
                            hObj.localEulerAngles = new Vector3(oldAng.x, oldAng.y - diffAng, oldAng.z);
                        }
                        else
                        {
                            hObj.localEulerAngles = new Vector3(oldAng.x, oldAng.y + diffAng, oldAng.z);
                        }

                    }
                }

                //elevasion align
                if (targetFound)
                {
                    for (int i = 0; i < elevasionAlignObj.Length; i++)
                    {
                        Transform eObj = elevasionAlignObj[i].transform;
                        Vector3 oldAng = eObj.localEulerAngles;

                        //計算の為一旦初期回転分を解消
                        eObj.rotation *= Quaternion.Inverse(eInitAli[i]);
                        Vector3 objFw = eObj.forward;
                        Vector3 td = eObj.InverseTransformPoint(Target.transform.position).normalized;
                        td.x = 0;
                        float diffAng = Mathf.Min(Vector3.Angle(objFw, eObj.TransformDirection(td)), alignmentSpeed * Time.deltaTime);
                        Quaternion diffRot = Quaternion.Euler(diffAng, 0, 0);
                        Debug.DrawRay(eObj.position, objFw, Color.red);
                        Debug.DrawRay(eObj.position, eObj.TransformDirection(td) * 3, Color.cyan);
                        if (td.y < 0)
                        {
                            //Debug.Log($"{(Quaternion.Inverse(eObj.parent.rotation) * eObj.rotation).eulerAngles.x}");
                            if(true/*!(eObj.eulerAngles.x < depressionLimit && (eObj.rotation * diffRot).eulerAngles.x >= depressionLimit)*/)
                            {
                                eObj.localEulerAngles = oldAng;
                                eObj.rotation *= diffRot;
                            }
                            else
                            {
                                eObj.localEulerAngles = oldAng;
                            }
                        }
                        else
                        {
                            //Debug.Log("upppppp"+(Quaternion.Inverse(eObj.parent.rotation) * eObj.rotation).eulerAngles.x);

                            if (true/*!(eObj.eulerAngles.x > 360 - elevasionLimit && (eObj.rotation * Quaternion.Inverse(diffRot)).eulerAngles.x <= 360 - elevasionLimit)*/)
                            {
                                eObj.localEulerAngles = oldAng;
                                eObj.rotation *= Quaternion.Inverse(diffRot);
                            }
                            else
                            {
                                eObj.localEulerAngles = oldAng;
                            }
                        }
                    }
                }
            }
        }
    }


    private float fireItv;
    public void DefaultAttack()
    {
        if (targetFound)
        {
            fireItv += Time.deltaTime;
            if(fireItv > 1)
            {
                fireItv = 0;
                LiveBullet.BulletInstantiate(defWep, transform.position + transform.forward * 2, (Target.transform.position - (transform.position + transform.forward * 2)).normalized * 200, 10);
            }
        
        }
    }

    /// <summary>
    /// デフォルトの被ダメージ処理　armorPointが0になった時InvokeDestroy()を実行する
    /// </summary>
    /// <param name="damage">ダメージの値</param>
    /// <param name="hitPosition">攻撃の当たった場所</param>
    /// <param name="source">攻撃を行ったオブジェクト</param>
    /// <param name="damageType">ダメージのタイプ</param>
    public void DefaultDamage(int damage, Vector3 hitPosition, GameObject source, string damageType)
    {
        armorPoint -= damage;
        if (armorPoint <= 0)
        {
            InvokeDestroy();
        }
    }

    public void DefaultDestroy()
    {
        if (alive) {
            if(navAgent != null) {
                navAgent.destination = transform.position;
            }
            alive = false;
            OnEnemyDestroy(this);
        }
        
    }

    /// <summary>
    /// 登録されているDestroy処理を実行 処理が登録されておらずかつuseDefaultFuncForEmptyPhaseがtrueの場合DefaultDestroyを実行
    /// </summary>
    public void InvokeDestroy()
    {
        if (DestroyFunction.GetPersistentEventCount() >= 1)
        {
            DestroyFunction.Invoke();
        }else if(useDefaultFuncForEmptyPhase) DefaultDestroy();
    }

    public void NoAction() { }

    override public void Awake()
    {
        base.Awake();
    }

    private void OnDestroy()
    {
        base.OnEnemyDestroy(this);
    }
}
