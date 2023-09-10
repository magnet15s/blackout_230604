using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class MiniTank : Enemy{
    public Animator anim;
    public GroundedSensor gg;
    public NavMeshAgent navAgent;
    public GameObject Target;
    public GameObject gunTurretBone;
    public GameObject gunBone;
    private Weapon cgun;

    private GameObject damageFX;
    private float lastRotate = 1;
    private float oldYAngle = 0;
    private Vector3 turretForward;
    private Vector3 targetFireAngle;
    private float fireIntervalCnt;
    private int curve = 0;
    private bool overApproach = false;
    public float approachPhaseChangeLug = 0;


    [Space]
    [Tooltip("タレットの旋回速度上限(degree/sec)")]
    public float turretRotationLimit = 0;

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

    [Tooltip("弾速")]
    [SerializeField] private float bulletVelocity = 100;

    [Tooltip("弾のダメージ")]
    [SerializeField] private int bulletDamage = 5;

    [Tooltip("弾の発射間隔(s)")]
    [SerializeField] private float fireInterval = 0.2f;

    [Tooltip("射撃時のターゲットとの距離と傾きの比(ターゲットとの距離がこの値の時、45°の角度で射撃します。)")]
    [SerializeField] private float shotTangent2TargetDistRatio = 200f;
    

    // Start is called before the first frame update
    void Start()
    {
        damageFX = (GameObject)Resources.Load("BO_WeaponSystem/Particles/vulletHit");
        base.modelName = this.modelName;
        if(maxArmorPoint == 0)maxArmorPoint = 200;
        if(armorPoint == 0)armorPoint = 200;
        if (onRadarDist == 0) onRadarDist = 500;

        cgun = transform.AddComponent<Weapon.Conc>();
        cgun.weaponName = "mini tank gun";

        if (Target == null && Enemy.sharedTarget != null) Target = Enemy.sharedTarget;

        oldYAngle = navAgent.gameObject.transform.eulerAngles.y;
        turretForward = gunTurretBone.transform.forward;

    }
    public override void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType) {
        armorPoint -=  damage;
        Instantiate(damageFX, hitPosition, Quaternion.identity).transform.LookAt(hitPosition + (hitPosition - transform.position));
        if(armorPoint <= 0) {
            anim.SetBool("Destroy", true);
            OnEnemyDestroy(this);//自分が死んだことを通知
        }
    }
    // Update is called once per frame
    void Update() {

        if (armorPoint > 0) {
            if(fireIntervalCnt > 0) {
                fireIntervalCnt -= Time.deltaTime;
                if(fireIntervalCnt < 0)fireIntervalCnt = 0;
            }

            Ray ray = new Ray(transform.position + (Target.transform.position - transform.position).normalized * 2, Target.transform.position - transform.position);
            RaycastHit result;
            Physics.Raycast(ray, out result, sensorRange);
            if (result.transform != null)
            {
                //Debug.Log(result.transform.gameObject.Equals(Target) + " " + result.transform.gameObject);

                //敵を視認している場合
                if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid && (result.transform != null && result.transform.Equals(Target.transform)))
                {
                    if (discoveredTargetShare)
                    {
                        Enemy.sharedTargetPosition = result.transform.position;
                        Enemy.targetReporter = this;
                    }
                    //照準
                    targetFireAngle = Target.transform.position - transform.position;
                    targetFireAngle.y += targetFireAngle.magnitude * (targetFireAngle.magnitude / shotTangent2TargetDistRatio) - 1;
                    targetFireAngle.Normalize();
                    //砲塔
                    bool aimOn = false;
                    float oldTurretAngY = gunTurretBone.transform.eulerAngles.y;
                    gunTurretBone.transform.LookAt(new Vector3(Target.transform.position.x, gunTurretBone.transform.position.y, Target.transform.position.z));
                    gunTurretBone.transform.eulerAngles = new Vector3(gunTurretBone.transform.eulerAngles.x, gunTurretBone.transform.eulerAngles.y, 0);//rotation = Quaternion.LookRotation(new Vector3(Target.transform.position.x, gunTurretBone.transform.position.y, Target.transform.position.z), gunTurretBone.transform.up);
                    if (Mathf.Abs(oldTurretAngY - gunTurretBone.transform.eulerAngles.y) > turretRotationLimit * Time.deltaTime)
                    {
                        gunTurretBone.transform.eulerAngles = new Vector3(gunTurretBone.transform.eulerAngles.x,
                            oldTurretAngY + Mathf.Sign(gunTurretBone.transform.eulerAngles.y - oldTurretAngY) * turretRotationLimit * Time.deltaTime * Mathf.Sign(180 - Mathf.Abs(gunTurretBone.transform.eulerAngles.y - oldTurretAngY)),
                            gunTurretBone.transform.eulerAngles.z);
                    }
                    else
                    {
                        aimOn = true;
                    }
                    //砲身
                    gunBone.transform.up = gunTurretBone.transform.forward;
                    gunBone.transform.right = gunTurretBone.transform.right;
                    gunBone.transform.localEulerAngles = new Vector3(90 - Mathf.Atan((Target.transform.position - transform.position).magnitude / shotTangent2TargetDistRatio) * Mathf.Rad2Deg, 0, 0);
                    //射撃
                    if (aimOn && result.transform.GetComponent<Enemy>() == null) MainFire();

                    //移動パターン↓
                    float targetdist = (Target.transform.position - transform.position).magnitude;

                    //敵が遠い場合
                    if (targetdist > ApproachDist)
                    {
                        navAgent.destination = Target.transform.position;
                        curve = 0;
                        //敵に接近しすぎた後、後退中の場合
                    }
                    else if (overApproach)
                    {//
                        navAgent.destination = transform.position - ((Target.transform.position - transform.position).normalized);
                        if (targetdist > retreatBoundary) overApproach = false;
                        //交戦距離となり、回避機動中の場合
                    }
                    else if (targetdist > overApproachDist)
                    {
                        if (approachPhaseChangeLug > 0)
                        {
                            approachPhaseChangeLug = approachPhaseChangeLug > Time.deltaTime ? approachPhaseChangeLug - Time.deltaTime : 0;
                        }
                        if (approachPhaseChangeLug <= 0)
                        {
                            if (curve == 0)
                            {
                                curve = (int)(lastRotate / lastRotate * Mathf.Sign(lastRotate));
                            }
                            if (curve < 0)
                            {
                                navAgent.destination = transform.position + -Vector3.Cross(Target.transform.position - transform.position, Vector3.up).normalized + (Target.transform.position - transform.position).normalized * flankAttackAngleTangent;
                            }
                            else
                            {
                                navAgent.destination = transform.position + Vector3.Cross(Target.transform.position - transform.position, Vector3.up).normalized + (Target.transform.position - transform.position).normalized * flankAttackAngleTangent;
                            }
                        }

                        //敵に接近しすぎた場合
                    }
                    else
                    {
                        curve = 0;
                        overApproach = true;
                        navAgent.destination = transform.position - ((Target.transform.position - transform.position).normalized);
                    }


                }
                else
                {
                    approachPhaseChangeLug += approachPhaseChangeLug < 1f ? Time.deltaTime : 0;
                    if (Enemy.sharedTargetPosition != null)
                    {
                        if (Enemy.targetReporter == this)
                        {
                            Enemy.targetReporter = null;
                        }
                        navAgent.destination = (Vector3)Enemy.sharedTargetPosition;

                    }
                }
            }
        } else {
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
    public override void MainFire() {
        if(fireIntervalCnt == 0) {
            LiveBullet.BulletInstantiate(cgun, gunBone.transform.position + gunBone.transform.up * 2.5f, targetFireAngle * bulletVelocity, bulletDamage);
            fireIntervalCnt = fireInterval;
        }
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

    public void ThrowHitResponse() {
    }
}
