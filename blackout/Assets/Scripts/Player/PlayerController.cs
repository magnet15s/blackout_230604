using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, WeaponUser, DamageReceiver
{
    [SerializeField] private Animator anim;
    [SerializeField] private CharacterController cc;
    [SerializeField] private GroundedSensor gs;
    [Space]
    public List<Weapon> weapons;
    [Space]
    //�v�Z�p
    private float gravity = 9.8f;
    private bool inAir = false;
    private float inAirCnt = 0;
    private float touchDownCnt = 0;
    private Vector3 movement = Vector3.zero;
    private Vector3 lastMovement = Vector3.zero;
    private Vector3 dashAngle = Vector3.zero;
    private Vector3 inertiaAngle = Vector3.zero;
    private bool dash = false;
    private float dashCTcnt = 0;
    private float lastTimeDelta;
    private Vector3 oldPosition;
    private Vector3 lastActualMovement;
    private Vector3 groundNormal;
    private Vector3 wallBoundVector;
    private bool wallBound = false;

    [SerializeField] private Weapon selectWep;
    [SerializeField] private int selWepIdx;

    //���̓R���e�L�X�g
    private Vector3 moveAngleContext;
    private float moveMagnContext;
    private bool dashContext = false;
    private bool dashItrContext = false;
    private bool fireContext = false;
    private Vector2 viewPoint;

    private float levelAiming;
    private float verticalAiming;

    private Vector3 moveDirForAnim;
    [Space]
    [SerializeField] private float speed = 10;
    [SerializeField] private float dashSpeed = 22;
    [SerializeField] private float dashCoolTime = 1;
    [SerializeField] private float touchDownTime = 0.8f;
    [Space]
    [SerializeField] private float turningSpeed = 200;
    [SerializeField] private float dashTurningFacter = 0.1f;
    [SerializeField] private Vector2 viewRotetionFactor = new Vector2(10, 10);
    [Space]
    public GameObject testcam;
    public GameObject sightOrigin;
    [Space]
    [SerializeField] private bool setEnemiesShareTarget = true;

    //------------�p��-------------
    public Animator getAnim() {
        return anim;
    }
    public string getWepUseAnimLayer() {
        return "right_arm";
    }
    public GameObject getAimingObj() {
        return sightOrigin;
    }
    

    //------------���͎󂯎��-----------
    public void OnMove(InputAction.CallbackContext context)
    {
        moveAngleContext = context.ReadValue<Vector2>();
        moveAngleContext.z = moveAngleContext.y;
        moveAngleContext.y = 0;

        moveMagnContext = moveAngleContext.magnitude;
        if(moveMagnContext > 1) moveMagnContext= 1;

        moveAngleContext.Normalize();

    }

    public void OnDashItr(InputAction.CallbackContext context)
    {
        dashItrContext = context.performed;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.canceled && dash)
        {
            DashCancel();
        }
        dashContext = context.performed;
        
    }


    public void OnLook(InputAction.CallbackContext context)
    {
        viewPoint += context.ReadValue<Vector2>()/3 * viewRotetionFactor;
        if (viewPoint.y > 80) viewPoint.y = 80;
        else if (viewPoint.y < -80) viewPoint.y = -80;

        //���_�ړ����f
        
    }

    //����֘A
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started) fireContext = true; 
        else if(context.canceled)fireContext = false;
    }

    public void OnFocus(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectWep.SubAction();
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectWep.Reload();
        }
    }
    public void WeaponsListUp(InputAction.CallbackContext context) {
        if(context.started && selWepIdx > 0) {
            selectWep.PutAway();
            selWepIdx--;
            selectWep = weapons[selWepIdx];
            selectWep.Ready();
        }
    }
    public void WeaponsListDown(InputAction.CallbackContext context) {
        if (context.started && selWepIdx+1 < weapons.Count) {
            selectWep.PutAway();
            selWepIdx++;
            selectWep = weapons[selWepIdx];
            selectWep.Ready();
        }
    }
    //----------------Update�Ȃ�----------------

    void Awake()
    {
        //�G�ݒ�
        if(setEnemiesShareTarget)Enemy.sharedTarget = this.gameObject;

        //���_��_����
        levelAiming = transform.eulerAngles.y;
        verticalAiming = transform.eulerAngles.x;

        viewPoint = new Vector2(transform.eulerAngles.y, transform.eulerAngles.x);

        //�A�j���[�^�[
        moveDirForAnim = Vector2.zero;

        //weapons
        if(weapons.Count == 0) {
            weapons.Add(new EmptyWeaponSlot());
        }
        for(int i = 0; i < weapons.Count; i++) {
            if (weapons[i] == null) {
                weapons[i] = new EmptyWeaponSlot();
            }
            weapons[i].setSender(this);
        }
        selectWep = weapons[0];
        selWepIdx = 0;
        
    }
    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (cc == null) cc = GetComponent<CharacterController>();
        if (gs == null) gs = GetComponent<GroundedSensor>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //���ړ��ʌv�Z
        lastActualMovement = this.transform.position - oldPosition;
        lastActualMovement /= lastTimeDelta;
        //Debug.Log($"{lastMovement}  {lastActualMovement}");
        
        //���_�ړ��v�Z

        float laRotVol = Mathf.Abs(viewPoint.x - levelAiming) < 0.01 ? 0f : ((viewPoint.x - levelAiming)*10) * Time.deltaTime;
        if (Mathf.Abs(laRotVol) > turningSpeed * Time.deltaTime) laRotVol = turningSpeed * Time.deltaTime * Mathf.Sign(laRotVol);
        levelAiming += laRotVol;

        float vaRotVol = Mathf.Abs(viewPoint.y - verticalAiming) < 0.01 ? 0f : ((viewPoint.y - verticalAiming) * 10) * Time.deltaTime;
        if (Mathf.Abs(vaRotVol) > turningSpeed * Time.deltaTime) vaRotVol = turningSpeed * Time.deltaTime * Mathf.Sign(vaRotVol);
        verticalAiming += vaRotVol;


        transform.eulerAngles = new Vector3(0, levelAiming, transform.eulerAngles.z);
        sightOrigin.transform.localEulerAngles = new Vector3(verticalAiming, 0, 0);

        //testcam�p
        testcam.transform.eulerAngles = new Vector3(viewPoint.y, viewPoint.x, testcam.transform.eulerAngles.z);



        //----------------------------
        //          �ړ��n
        //----------------------------

        //�_�b�V�����͎�����
        //�_�b�V���N�[�����Ȃǃ_�b�V�������͂ł����Ԃ�����
        if (dashContext && moveMagnContext > 0 && dashCTcnt == 0 && !dash)
        {
            dash = true;
            turningSpeed *= dashTurningFacter;
        }


        //�ړ����͌v�Z

        if (gs.isGrounded(out groundNormal) && !wallBound) //�ڒn����
        {
            
            if (dashCTcnt == 0) {
                if (!dash)//��_�b�V����
                {
                    movement = moveAngleContext * speed;
                } else//�_�b�V������
                  {
                    if (dashAngle == Vector3.zero)   //�_�b�V���J�n�t���[���̏ꍇ
                    {
                        movement = moveAngleContext.normalized * dashSpeed;
                        dashAngle = movement.normalized;
                    } else                            //�_�b�V����
                      {
                        if (Vector3.Angle(moveAngleContext, dashAngle) > 100) {
                            DashCancel();    //�_�b�V����������100�x�ȏ�̓��͓]���i�_�b�V���L�����Z���j
                        } else {
                            movement = Vector3.Normalize(dashAngle + moveAngleContext * Time.deltaTime * 2) * dashSpeed;
                            dashAngle = movement.normalized;
                        }
                    }
                }
            }
            if (dashCTcnt > 0)    //�_�b�V���㌸��
            {
                dashCTcnt -= Time.deltaTime;
                if (dashCTcnt <= 0) {
                    dashCTcnt = 0;
                    inertiaAngle = Vector3.zero;
                }
                movement = inertiaAngle * (speed * 0.7f + (dashSpeed - speed) * (dashCTcnt != 0 ? dashCTcnt : 0.01f / dashCoolTime));
            }
            if (inAir) {//���n�̏u��
                if(Vector3.Angle(groundNormal, Vector3.up) > cc.slopeLimit) {
                    wallBoundVector = Vector3.Reflect(lastMovement, groundNormal);
                    wallBound = true;
                    //Debug.LogWarning($"ha {Vector3.Angle(groundNormal, Vector3.up)}");
                } else {
                    inAir = false;
                    touchDownCnt = touchDownTime;
                }
            }
            if (touchDownCnt > 0) {
                touchDownCnt -= Time.deltaTime;
                if (touchDownCnt < 0) {
                    touchDownCnt = 0;
                }
                movement = worldVec2localVec(lastMovement) * 0.5f;
            }


            movement.y = -30;//�ڒn���d��
        } else {
            if (!inAir) {
                inAir = true;
                inAirCnt = 0;
                lastMovement.y = 0;
            } else {
                if(inAirCnt < 1.8)inAirCnt += Time.deltaTime;
                lastMovement.y = -(gravity * (float)Math.Pow(inAirCnt, 2) * Mathf.Sign(inAirCnt));

            }
            if (wallBound) {
                wallBound = false;
                lastMovement.x = wallBoundVector.x;
                lastMovement.z = wallBoundVector.z;
                //Debug.Log("�͂˂�����");
            }
            else if (lastMovement.magnitude * 0.99 > lastActualMovement.magnitude) {
                Vector2 horLMDiff = new Vector2(lastMovement.x, lastMovement.z) - new Vector2(lastActualMovement.x, lastActualMovement.z);
                if (horLMDiff.magnitude * 0.1f < horLMDiff.normalized.magnitude) horLMDiff.Normalize();
                else horLMDiff *= 0.1f;
                lastMovement.x = lastActualMovement.x - (horLMDiff.x);
                lastMovement.z = lastActualMovement.z - (horLMDiff.y);
                
                //Debug.Log("������ " + lastActualMovement);
            } else {
                //Debug.Log("Not������");
            }
            movement = worldVec2localVec(lastMovement);

            

            
            
            
        }

        //�A�j���[�^�[�p
        //�_�b�V���L�����Z��
        Vector3 mdfaDiff = ((movement * (dashCTcnt != 0 ? -1 : 1)) / dashSpeed - moveDirForAnim);
        if (mdfaDiff.magnitude > 0.08f)
        {
            moveDirForAnim += (mdfaDiff.normalized * Time.deltaTime * 4);
        }
        if (dashCTcnt > dashCoolTime / 1.5) moveDirForAnim /= (1 + 4 * Time.deltaTime);
        if (Mathf.Abs(moveDirForAnim.x) < 0.03) moveDirForAnim.x = 0;
        if (Mathf.Abs(moveDirForAnim.z) < 0.03) moveDirForAnim.z = 0;
        anim.SetFloat("moveDirX", moveDirForAnim.x);
        anim.SetFloat("moveDirY", moveDirForAnim.z);
        //���[�V�����Ǘ�
        MotionChange();

        //�ړ����f
        if(!inAir && touchDownCnt <= 0)lastMovement = localVec2worldVec(movement);
        movement *= Time.deltaTime;
        movement = localVec2worldVec(movement);
        oldPosition = transform.position;
        cc.Move(movement);

        //testcam�p
        sightOrigin.transform.position = testcam.transform.position;

        //���ړ��ʌv�Z�p
        lastTimeDelta = Time.deltaTime;

        //--------------------
        //  ���ړ��@���ˌ�
        //--------------------

        if (fireContext) selectWep.MainAction();
    }

    //���̑��֐�
    public void DashCancel()
    {
        dash = false;
        dashCTcnt = dashCoolTime;
        turningSpeed /= dashTurningFacter;
        inertiaAngle = dashAngle;
        dashAngle = Vector3.zero;
    }
    /// <summary>
    /// ���[�J����ԃx�N�g�������[���h��ԃx�N�g���ɕϊ�
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private Vector3 localVec2worldVec(Vector3 input)
    {
        //memo: ���ړ�����z�����A�O��ړ�����x�������]���Ă����̂Ŗ��������
        return new Vector3(
            Vector3.Dot(new Vector3(transform.right.x, transform.right.y, -transform.right.z), input),
            Vector3.Dot(transform.up, input),
            Vector3.Dot(new Vector3(-transform.forward.x, transform.forward.y, transform.forward.z), input)
        );
    }
    private Vector3 worldVec2localVec(Vector3 input)
    {
        return new Vector3(
            Vector3.Dot(input, new Vector3(transform.right.x, transform.right.y, transform.right.z)),
            Vector3.Dot(input, new Vector3(transform.up.x, transform.up.y, transform.up.z)),
            Vector3.Dot(input, new Vector3(transform.forward.x, transform.forward.y, transform.forward.z))
        );
    }

    private void MotionChange() {
        MotionReset();
        if (inAir) {
            anim.SetBool("fall", true);
        }else if(touchDownCnt > 0) {
            anim.SetBool("touch_down", true);
        } else {
            anim.SetBool("move", true);
        }
    } 
    private void MotionReset() {
        anim.SetBool("move", false);
        anim.SetBool("fall", false);
        anim.SetBool("touch_down", false);
    }

    public void onLturn(InputAction.CallbackContext context)
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 1, 0);
    }

    public void onRturn(InputAction.CallbackContext context)
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y - 1, 0);
    }

    public string getActState()
    {
        if (inAir)          return "in the air";
        
        if (dashCTcnt > 0)  return "dash cancel";
        if (dash)           return "dash";
        if (dashItrContext) return "dash interact";
        if (moveMagnContext > 0) return "move";
        return "idle";
    }

    public void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType) {
        Debug.Log("Damage!! " + Time.frameCount);
    }
}
