using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class missile_tank : Enemy {
    public Animator anim;
    [SerializeField]
    Transform target;
    public GroundedSensor gg;
    public NavMeshAgent navAgent;
    public GameObject Target;
    private Weapon cgun;
    public GameObject prefab;
    private GameObject damageFX;
    private float lastRotate = 1;
    private float oldYAngle = 0;
    private Vector3 turretForward;
    private Vector3 targetFireAngle;
    private float fireIntervalCnt;
    private int curve = 0;
    private bool overApproach = false;
    public float approachPhaseChangeLug = 0;
    bool isSpawning = false;
    Transform thisTransform;
    WaitForSeconds intervalWait;
    bool fire = false;
    

    [Tooltip("ターゲットの視認距離")]
    public float sensorRange = 300;

    [Tooltip("ターゲット視認時、他のEnemyにターゲットの位置を共有するか")]
    public bool discoveredTargetShare = true;

    [Tooltip("接近時、ターゲットの側面に回り込む際の角度を指定(迂回方向 / 直進方向)")]
    [SerializeField] private float flankAttackAngleTangent;

    [Tooltip("ターゲットとの距離がこの値以上の場合、ターゲットに直進し続けます。")]
    [SerializeField] private float ApproachDist;

    [Tooltip("ターゲットとの距離がこの値以下になると、retreatBoundaryまでターゲットから直線的に距離を取ります。")]
    [SerializeField] private float overApproachDist;

    [Tooltip("ターゲットから距離をとる際、ターゲットとの距離がいくつになるまで下がるか")]
    [SerializeField] private float retreatBoundary;

    

    [Tooltip("弾の発射間隔(s)")]
    [SerializeField] float interval = 0.1f;

    [Tooltip("弾の同時発射数")]
    [SerializeField] int iterationCount = 3;

    [Tooltip("発射し始める距離")]
    [SerializeField] int weaponrange = 100;


    // Start is called before the first frame update
    void Start() {
        thisTransform = transform;
        intervalWait = new WaitForSeconds(interval);
        damageFX = (GameObject)Resources.Load("BO_WeaponSystem/Particles/vulletHit");
        base.modelName = this.modelName;
        if (maxArmorPoint == 0) maxArmorPoint = 200;
        if (armorPoint == 0) armorPoint = 200;
        if (onRadarDist == 0) onRadarDist = 500;

        cgun = transform.AddComponent<Weapon.Conc>();
        cgun.weaponName = "mini tank gun";

        if (Target == null && Enemy.sharedTarget != null) Target = Enemy.sharedTarget;

        oldYAngle = navAgent.gameObject.transform.eulerAngles.y;
        

    }
    public override void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType) {
        armorPoint -= damage;
        Instantiate(damageFX, hitPosition, Quaternion.identity).transform.LookAt(hitPosition + (hitPosition - transform.position));
        if (armorPoint <= 0) {
            anim.SetBool("Destroy", true);
            OnEnemyDestroy(this);//自分が死んだことを通知
        }
    }
    // Update is called once per frame
    void Update() {
        if (isSpawning) {
            return;
        }

        StartCoroutine(nameof(SpawnMissile));
        if (armorPoint > 0) {
            if (fireIntervalCnt > 0) {
                fireIntervalCnt -= Time.deltaTime;
                if (fireIntervalCnt < 0) fireIntervalCnt = 0;
            }

            Ray ray = new Ray(transform.position + (Target.transform.position - transform.position).normalized * 2, Target.transform.position - transform.position);
            RaycastHit result;
            Physics.Raycast(ray, out result, sensorRange);
            if (result.transform != null) {
                //Debug.Log(result.transform.gameObject.Equals(Target) + " " + result.transform.gameObject);

                //敵を視認している場合
                if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid && (result.transform != null && result.transform.Equals(Target.transform))) {
                    if (discoveredTargetShare) {
                        Enemy.sharedTargetPosition = result.transform.position;
                        Enemy.targetReporter = this;
                    }
                    
                    //射撃
                    if (result.transform.GetComponent<Enemy>() == null) MainFire();

                    //移動パターン↓
                    float targetdist = (Target.transform.position - transform.position).magnitude;

                    if (targetdist <= weaponrange&&targetdist>=30) {
                        fire = true;
                    }
                    else {
                        fire = false;
                    }

                    //敵が遠い場合
                    if (targetdist > ApproachDist) {
                        navAgent.destination = Target.transform.position;
                        curve = 0;
                        //敵に接近しすぎた後、後退中の場合
                    }
                    else if (overApproach) {//
                        navAgent.destination = transform.position - ((Target.transform.position - transform.position).normalized);
                        if (targetdist > retreatBoundary) overApproach = false;
                        //交戦距離となり、回避機動中の場合
                    }
                    else if (targetdist > overApproachDist) {
                        if (approachPhaseChangeLug > 0) {
                            approachPhaseChangeLug = approachPhaseChangeLug > Time.deltaTime ? approachPhaseChangeLug - Time.deltaTime : 0;
                        }
                        if (approachPhaseChangeLug <= 0) {
                            if (curve == 0) {
                                curve = (int)(lastRotate / lastRotate * Mathf.Sign(lastRotate));
                            }
                            if (curve < 0) {
                                navAgent.destination = transform.position + -Vector3.Cross(Target.transform.position - transform.position, Vector3.up).normalized + (Target.transform.position - transform.position).normalized * flankAttackAngleTangent;
                            }
                            else {
                                navAgent.destination = transform.position + Vector3.Cross(Target.transform.position - transform.position, Vector3.up).normalized + (Target.transform.position - transform.position).normalized * flankAttackAngleTangent;
                            }
                        }

                        //敵に接近しすぎた場合
                    }
                    else {
                        curve = 0;
                        overApproach = true;
                        navAgent.destination = transform.position - ((Target.transform.position - transform.position).normalized);
                    }


                }
                else {
                    approachPhaseChangeLug += approachPhaseChangeLug < 1f ? Time.deltaTime : 0;
                    if (Enemy.sharedTargetPosition != null) {
                        if (Enemy.targetReporter == this) {
                            Enemy.targetReporter = null;
                        }
                        navAgent.destination = (Vector3)Enemy.sharedTargetPosition;

                    }
                }
            }
        }
        else {
            if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid) {
                navAgent.destination = transform.position;
            }
        }

        ///移動



        //最後どちらに曲がったか(lastRotate)
        if (Mathf.Abs(navAgent.gameObject.transform.eulerAngles.y - oldYAngle) / Time.deltaTime > 1) {
            lastRotate = navAgent.gameObject.transform.eulerAngles.y - oldYAngle;
            oldYAngle = navAgent.gameObject.transform.eulerAngles.y;
        }


    }
    IEnumerator SpawnMissile() {
        isSpawning = true;

        Vector3 euler;
        Quaternion rot;
        Livemissile homing;

        for (int i = 0; i < iterationCount; i++) {
            if (fire == true) {
                homing = Instantiate(prefab, thisTransform.position, Quaternion.identity).GetComponent<Livemissile>();
                homing.Target = target;
            }
            
        }

        yield return intervalWait;

        isSpawning = false;
    }

    public Animator getAnim() {
        throw new System.NotImplementedException();
    }

    public string getWepUseAnimLayer() {
        throw new System.NotImplementedException();
    }

    public GameObject getAimingObj() {
        throw new System.NotImplementedException();
    }
}
