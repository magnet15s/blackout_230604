using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MiniTank : Enemy {
    public Animator anim;
    public GroundedSensor gg;
    public NavMeshAgent navAgent;
    public GameObject Target;
    public GameObject gunTurretBone;
    public GameObject gunBone;

    private GameObject damageFX;
    private float lastRotate = 1;
    private float oldYAngle = 0;
    private Vector3 turretForward;

    [Space]
    [Tooltip("タレットの旋回速度上限(degree/sec)")]
    public float turretRotationLimit = 0;

    [Tooltip("ターゲットの視認距離")]
    public float sensorRange = 300;

    [Tooltip("ターゲット視認時、他のEnemyにターゲットの位置を共有するか")]
    public bool discoveredTargetShare = true;

    [Tooltip("接近時、ターゲットの側面に回り込む際の角度を指定")]
    [SerializeField] private float flankAttackAngle;    

    [Tooltip("ターゲットとの距離がこの値以上の場合、ターゲットに直進し続けます。")]
    [SerializeField] private float ApproachDist;

    [Tooltip("ターゲットとの距離がこの値以下になると、retreatBoundaryまでターゲットから直線的に距離を取ります。")]
    [SerializeField] private float overApproachDist;

    [Tooltip("ターゲットから距離をとる際、ターゲットとの距離がいくつになるまで下がるか")]
    [SerializeField] private float retreatBoundary;

    // Start is called before the first frame update
    void Start()
    {
        damageFX = (GameObject)Resources.Load("BO_WeaponSystem/Particles/vulletHit");
        modelName = "MiniTank";
        if(maxArmorPoint == 0)maxArmorPoint = 200;
        if(armorPoint == 0)armorPoint = 200;

        if (Target == null && Enemy.sharedTarget != null) Target = Enemy.sharedTarget;

        oldYAngle = navAgent.gameObject.transform.eulerAngles.y;
        turretForward = gunTurretBone.transform.forward;
    }
    public override void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType) {
        armorPoint -=  damage;
        Instantiate(damageFX, hitPosition, Quaternion.identity).transform.LookAt(hitPosition + (hitPosition - transform.position));
        if(armorPoint <= 0) {
            if(Enemy.targetReporter == this)Enemy.targetReporter = null;
            anim.SetBool("Destroy", true);
            
        }
    }
    // Update is called once per frame
    void Update() {



        if (armorPoint > 0) {
            Ray ray = new Ray(transform.position + (Target.transform.position - transform.position).normalized * 2, Target.transform.position - transform.position);
            RaycastHit result;
            Physics.Raycast(ray, out result, sensorRange);
            if (result.transform != null)
                //Debug.Log(result.transform.gameObject.Equals(Target) + " " + result.transform.gameObject);

                //敵を視認している場合
                if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid && (result.transform != null && result.transform.Equals(Target.transform))) {
                    if (discoveredTargetShare) {
                        Enemy.sharedTargetPosition = result.transform.position;
                        Enemy.targetReporter = this;
                    }
                    //gunBone.transform.LookAt(Target.transform);
                    MainFire();
                    //移動パターン↓
                    float targetdist = (Target.transform.position - transform.position).magnitude;
                    if (targetdist > ApproachDist) {
                        navAgent.destination = Target.transform.position;

                    }
                } else if (Enemy.sharedTargetPosition != null) {
                    if (Enemy.targetReporter == this) {
                        Enemy.targetReporter = null;
                    }
                    navAgent.destination = (Vector3)Enemy.sharedTargetPosition;

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

        float oldTurretAngY = gunTurretBone.transform.eulerAngles.y;
        gunTurretBone.transform.LookAt(new Vector3(Target.transform.position.x, gunTurretBone.transform.position.y, Target.transform.position.z));
        gunTurretBone.transform.eulerAngles = new Vector3(gunTurretBone.transform.eulerAngles.x, gunTurretBone.transform.eulerAngles.y, 0);//rotation = Quaternion.LookRotation(new Vector3(Target.transform.position.x, gunTurretBone.transform.position.y, Target.transform.position.z), gunTurretBone.transform.up);
        if (Mathf.Abs(oldTurretAngY - gunTurretBone.transform.eulerAngles.y) > turretRotationLimit * Time.deltaTime) {
            gunTurretBone.transform.eulerAngles = new Vector3(gunTurretBone.transform.eulerAngles.x,
                oldTurretAngY + Mathf.Sign(gunTurretBone.transform.eulerAngles.y - oldTurretAngY) * turretRotationLimit * Time.deltaTime * Mathf.Sign( 180 - Mathf.Abs(gunTurretBone.transform.eulerAngles.y - oldTurretAngY) ),
                gunTurretBone.transform.eulerAngles.z);
        }
    }

}
