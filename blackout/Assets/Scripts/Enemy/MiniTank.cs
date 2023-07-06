using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class MiniTank : Enemy , WeaponUser{
    public Animator anim;
    public GroundedSensor gg;
    public NavMeshAgent navAgent;
    public GameObject Target;
    public GameObject gunTurretBone;
    public GameObject gunBone;
    public EmptyWeaponSlot gun;

    private GameObject damageFX;
    private float lastRotate = 1;
    private float oldYAngle = 0;
    private Vector3 turretForward;
    private Vector3 targetFireAngle;
    private float fireIntervalCnt;


    [Space]
    [Tooltip("�^���b�g�̐��񑬓x���(degree/sec)")]
    public float turretRotationLimit = 0;

    [Tooltip("�^�[�Q�b�g�̎��F����")]
    public float sensorRange = 300;

    [Tooltip("�^�[�Q�b�g���F���A����Enemy�Ƀ^�[�Q�b�g�̈ʒu�����L���邩")]
    public bool discoveredTargetShare = true;

    [Tooltip("�ڋߎ��A�^�[�Q�b�g�̑��ʂɉ�荞�ލۂ̊p�x���w��")]
    [SerializeField] private float flankAttackAngle;    

    [Tooltip("�^�[�Q�b�g�Ƃ̋��������̒l�ȏ�̏ꍇ�A�^�[�Q�b�g�ɒ��i�������܂��B")]
    [SerializeField] private float ApproachDist;

    [Tooltip("�^�[�Q�b�g�Ƃ̋��������̒l�ȉ��ɂȂ�ƁAretreatBoundary�܂Ń^�[�Q�b�g���璼���I�ɋ��������܂��B")]
    [SerializeField] private float overApproachDist;

    [Tooltip("�^�[�Q�b�g���狗�����Ƃ�ہA�^�[�Q�b�g�Ƃ̋����������ɂȂ�܂ŉ����邩")]
    [SerializeField] private float retreatBoundary;

    [Tooltip("�e��")]
    [SerializeField] private float bulletVelocity = 100;

    [Tooltip("�e�̃_���[�W")]
    [SerializeField] private int bulletDamage = 5;

    [Tooltip("�e�̔��ˊԊu(s)")]
    [SerializeField] private float fireInterval = 0.2f;

    [Tooltip("�ˌ����̃^�[�Q�b�g�Ƃ̋����ƌX���̔�(�^�[�Q�b�g�Ƃ̋��������̒l�̎��A45���̊p�x�Ŏˌ����܂��B)")]
    [SerializeField] private float shotTangent2TargetDistRatio = 200f;
    

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

        gun = gameObject.AddComponent<EmptyWeaponSlot>();
        gun.weaponName = "mini tank gun";
        gun.sender = this;
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
            if(fireIntervalCnt > 0) {
                fireIntervalCnt -= Time.deltaTime;
                if(fireIntervalCnt < 0)fireIntervalCnt = 0;
            }

            Ray ray = new Ray(transform.position + (Target.transform.position - transform.position).normalized * 2, Target.transform.position - transform.position);
            RaycastHit result;
            Physics.Raycast(ray, out result, sensorRange);
            if (result.transform != null)
                //Debug.Log(result.transform.gameObject.Equals(Target) + " " + result.transform.gameObject);

                //�G�����F���Ă���ꍇ
                if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid && (result.transform != null && result.transform.Equals(Target.transform))) {
                    if (discoveredTargetShare) {
                        Enemy.sharedTargetPosition = result.transform.position;
                        Enemy.targetReporter = this;
                    }
                    //�Ə�
                    targetFireAngle = Target.transform.position - transform.position;
                    targetFireAngle.y = targetFireAngle.magnitude * (targetFireAngle.magnitude / shotTangent2TargetDistRatio);
                    targetFireAngle.Normalize();
                    //�C��
                    bool aimOn = false;
                    float oldTurretAngY = gunTurretBone.transform.eulerAngles.y;
                    gunTurretBone.transform.LookAt(new Vector3(Target.transform.position.x, gunTurretBone.transform.position.y, Target.transform.position.z));
                    gunTurretBone.transform.eulerAngles = new Vector3(gunTurretBone.transform.eulerAngles.x, gunTurretBone.transform.eulerAngles.y, 0);//rotation = Quaternion.LookRotation(new Vector3(Target.transform.position.x, gunTurretBone.transform.position.y, Target.transform.position.z), gunTurretBone.transform.up);
                    if (Mathf.Abs(oldTurretAngY - gunTurretBone.transform.eulerAngles.y) > turretRotationLimit * Time.deltaTime) {
                        gunTurretBone.transform.eulerAngles = new Vector3(gunTurretBone.transform.eulerAngles.x,
                            oldTurretAngY + Mathf.Sign(gunTurretBone.transform.eulerAngles.y - oldTurretAngY) * turretRotationLimit * Time.deltaTime * Mathf.Sign(180 - Mathf.Abs(gunTurretBone.transform.eulerAngles.y - oldTurretAngY)),
                            gunTurretBone.transform.eulerAngles.z);
                    } else {
                        aimOn = true;
                    }
                    //�C�g
                    gunBone.transform.up = gunTurretBone.transform.forward;
                    gunBone.transform.right = gunTurretBone.transform.right;
                    gunBone.transform.localEulerAngles = new Vector3(90-Mathf.Atan( (Target.transform.position - transform.position).magnitude / shotTangent2TargetDistRatio) * Mathf.Rad2Deg, 0, 0);
                    //�ˌ�
                    if(aimOn && result.transform.GetComponent<Enemy>() == null)MainFire();

                    //�ړ��p�^�[����
                    float targetdist = (Target.transform.position - transform.position).magnitude;
                    if (targetdist > ApproachDist) {
                        navAgent.destination = Target.transform.position;

                    }else if(targetdist > overApproachDist) {
                        if (lastRotate < 0) {
                            navAgent.destination = gunBone.transform.right * 0.1f;
                        } else {
                            navAgent.destination = -gunBone.transform.right * 0.1f;
                        }
                    }else {
                        navAgent.destination = -gunTurretBone.transform.forward * 0.1f;
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

        ///�ړ�



        //�Ō�ǂ���ɋȂ�������(lastRotate)
        if (Mathf.Abs(navAgent.gameObject.transform.eulerAngles.y - oldYAngle) / Time.deltaTime > 1) {
            lastRotate = navAgent.gameObject.transform.eulerAngles.y - oldYAngle;
            oldYAngle = navAgent.gameObject.transform.eulerAngles.y;
        }

        
    }
    public override void MainFire() {
        if(fireIntervalCnt == 0) {
            LiveBullet.BulletInstantiate(gun, gunBone.transform.position + gunBone.transform.up * 2.5f, targetFireAngle * bulletVelocity, bulletDamage);
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
}
