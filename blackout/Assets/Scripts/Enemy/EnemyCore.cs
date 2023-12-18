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
    [Tooltip("true�̊Ԋe�t�F�[�Y���J��Ԃ�")]public bool alive�@= true;
    [Space(20)]
    [SerializeField] private bool useDefaultFuncForEmptyPhase;
    [Space(20)]
    [Header("-----TargetSetPhase-----")]
    [Space]
    public GameObject Target;
    public bool referenceSheredTarget;
    /// <summary>
    /// �^�[�Q�b�g�̌��������t�F�[�Y�ŌĂ΂�鏈��
    /// �ݒ肵�Ȃ��ꍇEnemyCore.DefaultTargetSet()���Ă΂��
    /// </summary>
    [SerializeField, Tooltip("�^�[�Q�b�g�̌��������t�F�[�Y�ŌĂ΂�鏈���@�ݒ肵�Ȃ��ꍇEnemyCore.DefaultTargetSet()���Ă΂��")] 
    private UnityEvent TargetSetPhaseFunction;

    [Space]
    [Header("-----TargetFindPhase-----")]
    [Space]
    public bool targetFound;            //�O���Q�Ɨp
    private bool _targetFound;
    public Vector3 lastFoundPosition;   //�O���Q�Ɨp
    private Vector3 _lastFoundPosition;
    public float targetDist;            //�O���Q�Ɨp
    private float _targetDist;          
    [SerializeField] private float findRange = 200;
    public bool targetShare = false;
    [SerializeField, Tooltip("�^�[�Q�b�g���ڎ��o���Ă��邩�𔻒f���鏈���@�ݒ肵�Ȃ��ꍇEnemyCore.DefaultTargetFind()���Ă΂��")] 
    private UnityEvent TargetFindPhaseFunction;

    [Space]
    [Header("-----MovePhase-----")]
    [Space]
    [SerializeField] private NavMeshAgent navAgent;
    [SerializeField, Tooltip("�����ɉ����Ď��s���鏈��(���s������MovePhaseFunctionsRange�Ŏw��B�z�񒷂�����Ȃ��ꍇidx=0�̏�������Ɏ��s)�@�z�񂪋�̏ꍇ�f�t�H���g�̍s���p�^�[�����ݒ肳���")]
    private UnityEvent[] MovePhaseFunctions;
    [SerializeField, Tooltip("�C���f�b�N�X�̑Ή�����MovePhaseFunction�̎��s����(targetDist���l�ȉ��̏ꍇ�Ή����鏈�������s�B-1�ŋ������B�l�̏����������D�悳���)")]
    private float[] MovePhaseFunctionsRange;
    private int[] MoveFuncOrder;
    [SerializeField, Tooltip("�ړ����s��Ȃ����̑ҋ@�����@�f�t�H���g�̈ړ������s�s�\�������ꍇ�A�t�H�[���o�b�N�I�ɌĂ΂�鎖������")]
    private UnityEvent MoveStayFunctions;

    [Space]
    [Header("-----AlignPhase-----")]
    [Space]
    [SerializeField] private GroundedSensor gs;
    [Tooltip("�p����n�`�ɉ��킹�邩�@gs�y��navAgant���K�v")]public bool grounding;
    [Tooltip("�n�`�ɉ��킹��I�u�W�F�N�g�i�q���܂ށj")] public GameObject[] groundingObj;
    [Tooltip("�����Ə��iy����]�j���s���I�u�W�F�N�g�i�q���܂ށj�i�Đ��r���Œǉ����Ȃ����Ɓj")] public GameObject[] horizontalAlignObj;
    public float leftAlignLimit = 180;
    public float rightAlignLimit = 180;
    private float[] hInitY;
    [Tooltip("�����Ə��ix����]�j���s���I�u�W�F�N�g�i�q���܂ށj�i�Đ��r���Œǉ����Ȃ����Ɓj")] public GameObject[] elevasionAlignObj;
    public float elevasionLimit = 30;
    public float depressionLimit = 30;
    private float[] eInitX;
    private Quaternion[] groundingInitRot;
    private Quaternion[] hInitAli;
    private Quaternion[] eInitAli;
    public float alignmentSpeed = 100;
    [SerializeField, Tooltip("�p����n�`�ɉ��킹����A�����Ə��A�{�p�Ə����s�������B")]
    private UnityEvent AlignPhaseFunction;

    [Space]
    [Header("-----AttackPhase-----")]
    [Space]
    [SerializeField] private UnityEvent AttackPhaseFunction;
    private Weapon defWep;

    [Space]
    [Header("-----Other-----")]
    [Space]

    [SerializeField, Tooltip("�_���[�W���󂯂����̏��� (�����Fint damage, Vector3 hitPosition, GameObject source, string damageType)")]
    private UnityEvent<int,Vector3,GameObject,string> DamageFunction;
    [SerializeField, Tooltip("�_���[�W���󂯂�armorPoint��0�ɂȂ����Ƃ��̏����ienemyCore.InvokeDestroy()�ő��X�N���v�g������s�\�j")]
    private UnityEvent DestroyFunction;


    // Start is called before the first frame update
    void Start()
    {
        defWep = transform.AddComponent<Weapon.Conc>();

        //MovePhase�̎��s��������
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

        //Align�n������
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
            Debug.Log("[EnemyCore.DefaultAlign] > navMesh��null�ł�");
        }
        


        

    }

    // Update is called once per frame
    void Update()
    {
        if (alive) {

            //�^�[�Q�b�g�̐ݒ�
            if (TargetSetPhaseFunction.GetPersistentEventCount() >= 1) { TargetSetPhaseFunction?.Invoke(); } else if (useDefaultFuncForEmptyPhase)
                DefaultTargetSet();

            //�^�[�Q�b�g�̑{��
            if (TargetFindPhaseFunction.GetPersistentEventCount() >= 1) { TargetFindPhaseFunction?.Invoke(); } else if (useDefaultFuncForEmptyPhase)
                DefaultTargetFind();

            //�ړ�
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

            //�Ə�
            if (AlignPhaseFunction.GetPersistentEventCount() >= 1) { AlignPhaseFunction.Invoke(); } else if (useDefaultFuncForEmptyPhase)
                DefaultAlign();

            //�U��
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
    /// EnemyCore�̃f�t�H���g�̃^�[�Q�b�g���菈��
    /// referenceSharedTarget��true�̏ꍇEnemy.SheredTarget���^�[�Q�b�g�ɐݒ肷��
    /// </summary>
    public void DefaultTargetSet()
    {
        if (referenceSheredTarget)
        {
            Target = Enemy.sharedTarget;
        }
    }

    /// <summary>
    /// �C�ӂ̍��W�܂Ŏ������ʂ邩��Ԃ�
    /// </summary>
    /// <param name="point">���W�i���[���h��ԁj</param>
    /// <param name="ignoreObject">�������Ղ�Ȃ��I�u�W�F�N�g</param>
    /// <returns></returns>
    public bool VisibleThisToPoint(Vector3 targetPoint, float range, Transform[] ignoreObject) {
        Vector3 posDiff = targetPoint - transform.position;
        if (posDiff.magnitude > range) {
            return false;
        }
        Ray ray = new Ray(transform.position, posDiff);
        RaycastHit[] results = new RaycastHit[20];
        int len = Physics.RaycastNonAlloc(ray, results, targetDist);
        //���U���g�Ɏ��E���ǂ��������邩���ׂ�
        bool blocked = false;
        for (int i = 0; i < len; i++) {
            RaycastHit res = results[i];

            //���U���g��ignoreList�ɓ����邩
            bool isIgnoreObj = false;
            foreach (Transform io in ignoreObject) {
                if (io.Equals(res.transform)) {
                    isIgnoreObj = true; break;
                }
            }
            if (isIgnoreObj) continue;

            //�e��k����ignoreOnject�ɓ����邩
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
    /// EnemyCore�̃f�t�H���g�̃^�[�Q�b�g�F������
    /// �^�[�Q�b�g�̈ʒu�܂Ń��C���΂��A�r���Ɏ��E���Ղ镨���������𒲂�TargetFound�ɔ��f����
    /// (���g�ƃ^�[�Q�b�g�Ԃ�20�ȏ�̃R���C�_�[�����݂���ꍇ����ɓ��삵�Ȃ��\��������)
    /// </summary>
    public void DefaultTargetFind()
    {
        if (Target == null) {
            targetFound = false;
            _targetFound = false;
            return;
        }
        //�v���C���[�̕����Ƀ��C���΂�
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
        //���U���g�Ɏ��E���ǂ��������邩���ׂ�
        bool blocked = false;

        for(int i = 0; i < len; i++)
        {
            RaycastHit res = results[i];
            
            //���U���g������or�^�[�Q�b�g�̏ꍇ�������Ď��̃��U���g��
            if (res.transform == transform�@|| res.transform == Target.transform) continue;

            //�e��k���Ď���or�^�[�Q�b�g�ɓ����邩
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
        //���ʂ��i�[
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

        //���J�ppublic�ϐ��̍X�V
        targetFound = _targetFound;
        targetDist = _targetDist;

    }


    /// <summary>
    /// EnemyCore�̃f�t�H���g�̈ړ������̓��̐ڋߏ���
    /// �f�t�H���g�ł�targetDist��80�ȏ�̎��Ă΂��
    /// </summary>
    public void DefaultApproachMove()
    {
        if(navAgent == null)
        {
            Debug.LogError("[EnemyCore.DefaultApproachMove] > navAgent���Z�b�g����Ă��܂���@�f�t�H���g�ڋߏ����𗘗p����ꍇ��navAgent���Z�b�g���Ă�������");
            DefaultStayMove();
            return;
        }
        if(Target == null)
        {
            Debug.LogError("[EnemyCore.DefaultApproachMove] > Target��null�ł�");
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
    /// EnemyCore�̃f�t�H���g�̈ړ������̓��̐퓬�@������
    /// �f�t�H���g�ł�targetDist��30�ȏ�80�����̎��Ă΂��
    /// </summary>
    public void DefaultBattleMove()
    {
        if(navAgent == null)
        {
            Debug.LogError("[EnemyCore.DefaultBattleMove] > navAgent���Z�b�g����Ă��܂���@�f�t�H���g�퓬�@�������𗘗p����ꍇ��navAgent���Z�b�g���Ă�������");
            DefaultStayMove();
            return;
        }
        if (Target == null)
        {
            Debug.LogError("[EnemyCore.DefaultBattleMove] > Target��null�ł�");
            DefaultStayMove();
            return;
        }

        //�E���or����������
        //�������猩�ă^�[�Q�b�g�����E�ǂ���ɂ��邩�擾
        Vector3 tPosOnLocSpc = transform.InverseTransformPoint(Target.transform.position);

       
        //�ړ�����������
        if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid)
        {
            if(tPosOnLocSpc.x > 0)
                navAgent.destination = transform.position + (Vector3.Cross(Target.transform.position - transform.position, Vector3.up).normalized);
            else
                navAgent.destination = transform.position + (-Vector3.Cross(Target.transform.position - transform.position, Vector3.up).normalized);

        }
    }


    /// <summary>
    /// EnemyCore�̃f�t�H���g�̈ړ������̓��̌�ޏ���
    /// �f�t�H���g�ł�targetDist��30�����̎��Ă΂��
    /// </summary>
    public void DefaultRetreatMove()
    {
        if(navAgent == null) {
            Debug.LogError("[EnemyCore.DefaultRetreatMove] > navAgent���Z�b�g����Ă��܂���@�f�t�H���g��ޏ����𗘗p����ꍇ��navAgent���Z�b�g���Ă�������");
            DefaultStayMove();
            return;
        }
        if (Target == null)
        {
            Debug.LogError("[EnemyCore.DefaultRetreatMove] > Target��null�ł�");
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
    /// EnemyCore�̃f�t�H���g�̈ړ������̂����̑ҋ@����
    /// ���̃f�t�H���g�ړ����������s�s�\�i�G���[���j�Ȏ��ɂ��Ă΂��
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
            Debug.LogError("[EnemyCore.DefaultAlign] > Target��null�ł�");
            return;
        }

        //grounding
        if (grounding)
        {
            if (gs == null)
            {
                Debug.LogWarning("[EnemyCore.DefaultAlign] > gs��null�ł� grounding�͎��s�ł��܂���");
            }
            else
            {
                Vector3 gNormal;
                if(navAgent == null)
                {
                    Debug.LogWarning("[EnemyCore.DefaultAlign] > navMesh��null�ł� grounding�͎��s�ł��܂���");
                }
                if(gs.isGrounded(out gNormal))
                {
                    //��������������
                    


                    Vector3 oldForward = -navAgent.transform.forward;
                    Vector3 oldUp = navAgent.transform.up;
                    Vector3 newRight = Vector3.Cross(oldForward, gNormal).normalized;
                    Vector3 newForward = Vector3.Cross(newRight, gNormal).normalized;

                    //navAgent.transform�Ƃ̃M���b�v
                    Debug.DrawRay(transform.position, navAgent.transform.up, Color.gray);
                    Debug.DrawRay(transform.position, navAgent.transform.forward, Color.gray);
                    Debug.DrawRay(transform.position, navAgent.transform.right, Color.gray);

                    Debug.DrawRay(transform.position, gNormal, Color.green);
                    Debug.DrawRay(transform.position, newRight, Color.red);
                    Debug.DrawRay(transform.position, newForward, Color.blue);

                    //��]�𐶐�
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

                        //�v�Z�̈׈�U������]��������
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

                        //�v�Z�̈׈�U������]��������
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
    /// �f�t�H���g�̔�_���[�W�����@armorPoint��0�ɂȂ�����InvokeDestroy()�����s����
    /// </summary>
    /// <param name="damage">�_���[�W�̒l</param>
    /// <param name="hitPosition">�U���̓��������ꏊ</param>
    /// <param name="source">�U�����s�����I�u�W�F�N�g</param>
    /// <param name="damageType">�_���[�W�̃^�C�v</param>
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
    /// �o�^����Ă���Destroy���������s �������o�^����Ă��炸����useDefaultFuncForEmptyPhase��true�̏ꍇDefaultDestroy�����s
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
