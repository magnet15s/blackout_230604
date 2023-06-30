using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public float sensorRange = 300;
    public bool discoveredTargetShare = true;
    private GameObject damageFX;
    private float lastRotate = 1;
    private float oldYAngle = 0;
    private Vector3 turretForward;

    [Space]

    [Tooltip("�ڋߎ��A�^�[�Q�b�g�̑��ʂɉ�荞�ލۂ̊p�x���w��")]
    [SerializeField] private float flankAttackAngle;    

    [Tooltip("�^�[�Q�b�g�Ƃ̋��������̒l�ȏ�̏ꍇ�A�^�[�Q�b�g�ɒ��i�������܂��B")]
    [SerializeField] private float ApproachDist;

    [Tooltip("�^�[�Q�b�g�Ƃ̋��������̒l�ȉ��ɂȂ�ƁAretreatBoundary�܂Ń^�[�Q�b�g���璼���I�ɋ��������܂��B")]
    [SerializeField] private float overApproachDist;

    [Tooltip("�^�[�Q�b�g���狗�����Ƃ�ہA�^�[�Q�b�g�Ƃ̋����������ɂȂ�܂ŉ����邩")]
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

                //�G�����F���Ă���ꍇ
                if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid && (result.transform != null && result.transform.Equals(Target.transform))) {
                    if (discoveredTargetShare) {
                        Enemy.sharedTargetPosition = result.transform.position;
                        Enemy.targetReporter = this;
                    }
                    gunBone.transform.LookAt(Target.transform);
                    MainFire();
                    //�ړ��p�^�[����
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

        ///�ړ�



        //�Ō�ǂ���ɋȂ�������(lastRotate)
        if (Mathf.Abs(navAgent.gameObject.transform.eulerAngles.y - oldYAngle) / Time.deltaTime > 1) {
            lastRotate = navAgent.gameObject.transform.eulerAngles.y - oldYAngle;
            oldYAngle = navAgent.gameObject.transform.eulerAngles.y;
        }
        //Vector3 turretLookVec = 
        //gunTurretBone.transform.rotation = Quaternion.LookRotation()
        
    }

}
