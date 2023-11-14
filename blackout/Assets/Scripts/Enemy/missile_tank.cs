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
    

    [Tooltip("�^�[�Q�b�g�̎��F����")]
    public float sensorRange = 300;

    [Tooltip("�^�[�Q�b�g���F���A����Enemy�Ƀ^�[�Q�b�g�̈ʒu�����L���邩")]
    public bool discoveredTargetShare = true;

    [Tooltip("�ڋߎ��A�^�[�Q�b�g�̑��ʂɉ�荞�ލۂ̊p�x���w��(�I����� / ���i����)")]
    [SerializeField] private float flankAttackAngleTangent;

    [Tooltip("�^�[�Q�b�g�Ƃ̋��������̒l�ȏ�̏ꍇ�A�^�[�Q�b�g�ɒ��i�������܂��B")]
    [SerializeField] private float ApproachDist;

    [Tooltip("�^�[�Q�b�g�Ƃ̋��������̒l�ȉ��ɂȂ�ƁAretreatBoundary�܂Ń^�[�Q�b�g���璼���I�ɋ��������܂��B")]
    [SerializeField] private float overApproachDist;

    [Tooltip("�^�[�Q�b�g���狗�����Ƃ�ہA�^�[�Q�b�g�Ƃ̋����������ɂȂ�܂ŉ����邩")]
    [SerializeField] private float retreatBoundary;

    

    [Tooltip("�e�̔��ˊԊu(s)")]
    [SerializeField] float interval = 0.1f;

    [Tooltip("�e�̓������ː�")]
    [SerializeField] int iterationCount = 3;

    [Tooltip("���˂��n�߂鋗��")]
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
            OnEnemyDestroy(this);//���������񂾂��Ƃ�ʒm
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

                //�G�����F���Ă���ꍇ
                if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid && (result.transform != null && result.transform.Equals(Target.transform))) {
                    if (discoveredTargetShare) {
                        Enemy.sharedTargetPosition = result.transform.position;
                        Enemy.targetReporter = this;
                    }
                    
                    //�ˌ�
                    if (result.transform.GetComponent<Enemy>() == null) MainFire();

                    //�ړ��p�^�[����
                    float targetdist = (Target.transform.position - transform.position).magnitude;

                    if (targetdist <= weaponrange&&targetdist>=30) {
                        fire = true;
                    }
                    else {
                        fire = false;
                    }

                    //�G�������ꍇ
                    if (targetdist > ApproachDist) {
                        navAgent.destination = Target.transform.position;
                        curve = 0;
                        //�G�ɐڋ߂���������A��ޒ��̏ꍇ
                    }
                    else if (overApproach) {//
                        navAgent.destination = transform.position - ((Target.transform.position - transform.position).normalized);
                        if (targetdist > retreatBoundary) overApproach = false;
                        //��틗���ƂȂ�A����@�����̏ꍇ
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

                        //�G�ɐڋ߂��������ꍇ
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

        ///�ړ�



        //�Ō�ǂ���ɋȂ�������(lastRotate)
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
