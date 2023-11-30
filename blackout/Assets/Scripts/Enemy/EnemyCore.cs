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
    /// �^�[�Q�b�g�̌��������t�F�[�Y�ŌĂ΂�鏈��
    /// �ݒ肵�Ȃ��ꍇEnemyCore.DefaultTargetSet()���Ă΂��
    /// </summary>
    [SerializeField, Tooltip("�^�[�Q�b�g�̌��������t�F�[�Y�ŌĂ΂�鏈���@�ݒ肵�Ȃ��ꍇEnemyCore.DefaultTargetSet()���Ă΂��")] 
    private UnityEvent TargetSetPhaseFunction;

    [Space]
    [Header("TargetFindPhase")]
    [Space]
    public bool targetFound;
    private bool _targetFound;
    [SerializeField] private float findRange;
    [SerializeField, Tooltip("�^�[�Q�b�g���ڎ��o���Ă��邩�𔻒f���鏈���@�ݒ肵�Ȃ��ꍇEnemyCore.DefaultTargetFind()���Ă΂��")] 
    private UnityEvent TargetFindPhaseFunction;

    [Space]
    [Header("MovePhase")]
    [Space]
    [SerializeField] private NavMeshAgent navAgent;
    public float targetDist;
    private float _targetDist;
    [SerializeField, Tooltip("�����ɉ����Ď��s���鏈��(���s������MovePhaseFunctionsRange�Ŏw��B�z�񒷂�����Ȃ��ꍇidx=0�̏�������Ɏ��s)�@�z�񂪋�̏ꍇ�f�t�H���g�̍s���p�^�[�����ݒ肳���")]
    private UnityEvent[] MovePhaseFunctions;
    [SerializeField, Tooltip("�C���f�b�N�X�̑Ή�����MovePhaseFunction�̎��s����(targetDist���l�ȉ��̏ꍇ�Ή����鏈�������s�B-1�ŋ������B�l�̏����������D�悳���)")]
    private float[] MovePhaseFunctionsRange;
    private int[] MoveFuncOrder;
    [SerializeField, Tooltip("�ړ����s��Ȃ����̑ҋ@�����@�f�t�H���g�̈ړ������s�s�\�������ꍇ�A�t�H�[���o�b�N�I�ɌĂ΂�鎖������")]
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
    /// �^�[�Q�b�g���ڎ��o���Ă��邩�𔻒f���鏈��
    /// �ݒ肵�Ȃ��ꍇEnemyCore.DefaultTargetFind()���Ă΂��
    /// </summary>
    [SerializeField] private UnityEvent AlignPhaseFunction;

    [Space]
    [Header("AttackPhase")]
    [Space]
    [SerializeField] private UnityEvent AttackPhaseFunction;

    [Space]
    [Header("Other")]
    [Space]

    [SerializeField, Tooltip("�_���[�W���󂯂����̏����@�����Ƃ���DamageEventArgs�����֐��̂ݐݒ�\")]
    private UnityEvent<DamageEventArgs> DamageFunction; 


    // Start is called before the first frame update
    void Start()
    {

        //MovePhase�̎��s��������
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

        //Align�n������
        hAliObjInitVec = new Vector3[horizontalAlignObj.Length];
        eAliObjInitVec = new Vector3[elevasionAlignObj.Length];

        Alignment = transform.forward;
        AlignTarget = transform.forward;


        

    }

    // Update is called once per frame
    void Update()
    {
        //�^�[�Q�b�g�̐ݒ�
        if (TargetSetPhaseFunction != null) { TargetSetPhaseFunction?.Invoke(); }
        else DefaultTargetSet();

        //�^�[�Q�b�g�̑{��
        if (TargetFindPhaseFunction != null) { TargetFindPhaseFunction?.Invoke(); }
        else DefaultTargetFind();

        //�ړ�
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

        //�Ə�
        if (AlignPhaseFunction != null) { AlignPhaseFunction.Invoke(); }
        else DefaultAlign();

        //�U��
        if (AttackPhaseFunction != null) { AttackPhaseFunction.Invoke(); }
        else DefaultAttack();

        
    }

    public override void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType)
    {
        if (DamageFunction != null) DamageFunction.Invoke(new DamageEventArgs(damage, hitPosition, source, damageType));
        else DefaultDamage(new DamageEventArgs(damage, hitPosition, source, damageType));
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
    /// EnemyCore�̃f�t�H���g�̃^�[�Q�b�g�F������
    /// �^�[�Q�b�g�̈ʒu�܂Ń��C���΂��A�r���Ɏ��E���Ղ镨���������𒲂�TargetFound�ɔ��f����
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
                blocked = true; break;
            }
        }
        _targetFound = !blocked;

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
            if(Target != null && targetFound) {
                Debug.Log("���������I");
                navAgent.destination = Target.transform.position;
            } else if(referenceSheredTarget && Enemy.sharedTargetPosition != null) {
                navAgent.destination = (Vector3)sharedTargetPosition;
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

    }


    /// <summary>
    /// EnemyCore�̃f�t�H���g�̈ړ������̂����̑ҋ@����
    /// ���̃f�t�H���g�ړ����������s�s�\�i�G���[���j�Ȏ��ɂ��Ă΂��
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
