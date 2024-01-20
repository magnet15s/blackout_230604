using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
//using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, WeaponUser, DamageReceiver {

    public enum PlayerActionState {
        idle = 0,
        move = 1,
        dash = 2,
        dashcancel = 3,
        dashcharge = 4,
        falling = 5,
        jump = 6,
        jumpcharge = 7,
        evasionmove = 8,
        touchdown = 9,
        weaponmove = 10,
        weaponairmove = 11,
    }
    [SerializeField] private AudioSource audio;
    [SerializeField] private AudioSource audio2;
    [SerializeField] private AudioSource audio3;
    [SerializeField] private AudioSource audio4;
    public GameObject EMPPrefab;
    public GameObject SmokePrefab;
    [SerializeField] private Animator anim;
    [SerializeField] private CharacterController cc;
    [SerializeField] private GroundedSensor gs;
    [Space]
    [SerializeField] private Image weaponImage;
    [SerializeField] private Material noDataWeaponImage;
    [SerializeField] private Image hitResponse;
    [SerializeField] private Image statusView;
    [Space]
    [SerializeField] private GameObject damageFX;

    [Space]
    [SerializeField] private Transform cockpit;
    [SerializeField] private Transform cockpitParent;
    [SerializeField] private float cockpitVerticalAlignOffset;
    [SerializeField] private float cockpitVerticalAlignFactor;
    [SerializeField] private float cockpitVAlignFactorOnParentRot;
    [SerializeField] private Transform pilotEyePoint;
    [SerializeField] private PlayerCameraCood pcc;
    [Space]
    public List<Weapon> weapons;

    [Space]
    //�ړ��v�Z�p
    [SerializeField]private float gravity = 9.8f;
    private bool inAir = false;
    [SerializeField] private float inAirCnt = 0;
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
    private bool jump = false;
    private bool jumped = false;
    private bool wepMoving = false;

    //���퐧��
    [SerializeField] public Weapon selectWep { get; private set; }
    [SerializeField] private int selWepIdx;
    private List<bool> WepActReqBools = new();
    private List<WeaponConnectionToBone> wepConnectors = new();
    [SerializeField] private WeaponConnectionToBone wepCRoot, wepCRightForearm, WepCLeftForearm;

    //���̓R���e�L�X�g
    private Vector3 moveAngleContext;
    private float moveMagnContext;
    private bool dashContext = false;
    private bool dashItrContext = false;
    private float dashItrCnt = 0;
    private bool jumpContext = false;
    private bool jButtonContext = false;
    private float jumpChargeCnt = 0;
    private bool evasionMoveContext = false;
    private Vector3 evasionMoveAngle;
    private float evasionMoveTime = 0;
    private bool fireContext = false;
    private Vector2 viewPoint;
    private bool focusContext = false;
    private float focusMagn;

    private float levelAiming;
    private float verticalAiming;
    private float initCockpitLAim;
    private float initCockpitVAim;
    private float initCockpitParentVAim;

    

    private Vector3 moveDirForAnim;
    [Space]
    [SerializeField] private int maxArmorPoint = 500;
    [SerializeField] private int armorPoint = 500;
    [Space]
    [SerializeField] private float speed = 10;
    [SerializeField] private float brake = 2;
    [SerializeField] private float dashSpeed = 22;
    [SerializeField] private float dashInteractTime = 0.5f;
    [SerializeField] private float dashCoolTime = 1;
    [SerializeField] private float touchDownTime = 0.8f;
    [Space]
    [SerializeField, Tooltip("�W�����v�̂����悻�̏㏸����")] private float jumpTime = 2;
    [SerializeField] private float jumpChargeTime = 0.3f;
    [SerializeField] private float fallSpeedFactor = 1;
    [SerializeField] private float airAxelFallSpeed = 0.5f;
    [SerializeField] private float airAcceleration = 10;
    [SerializeField] private float maxAirAxelSpeed = 30;
    [Space]
    [SerializeField] private float evasionMoveAllTime = 0.7f;
    [SerializeField] private float evasionMoveSpeed = 30;
    [Space]
    [SerializeField] private float turningSpeed = 200;
    [SerializeField] private float slowTurningZoneSize = 0.1f;
    [SerializeField] private float dashTurningFactor = 0.4f;
    [SerializeField] private Vector2 viewRotetionFactor = new Vector2(10, 10);
    [SerializeField] private float zoomInViewRotFactor = 0.5f;
    [SerializeField] private float maxFocusMagn = 2;
    [Space]
    public GameObject pilotCamera;
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
    public void ThrowHitResponse() {
        hitResponse.color = new Color(1, 1, 1, 1);
    }


    public event EventHandler WepActionCancel;
    void OnWepActionCancel(EventArgs e) {
        WepActionCancel.Invoke(this, e);
    }

    private WeaponUser.MoveOverrideForWepAct wepMove = null;
    private float wepMoveLiveTime = 0;
    bool WeaponUser.SetWepMove(WeaponUser.MoveOverrideForWepAct wepMove, float overallTime) {
        if (this.wepMove == null) {
            this.wepMove = null;
            this.wepMove = wepMove;
            wepMoveLiveTime = overallTime;
            return true;
        } else return false;
    }
    void WeaponUser.removeWepMove(WeaponUser.MoveOverrideForWepAct wepMove) {
        if (this.wepMove == wepMove) { Debug.LogWarning("wepmove hit"); } else { Debug.LogWarning("wepmove not found"); }
        this.wepMove -= wepMove;

    }



    public void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType) {
        Debug.Log("Damage!! " + Time.frameCount);
        armorPoint -= damage;
        GameObject dfx;
        (dfx = Instantiate(damageFX, hitPosition, Quaternion.identity)).transform.LookAt(hitPosition + (hitPosition - transform.position));
        dfx.transform.localScale = new Vector3(3, 3, 3);


        statusView.gameObject.GetComponent<Animator>().SetFloat("Armor", (float)armorPoint / (float)maxArmorPoint);
        statusView.gameObject.GetComponent<Animator>().SetTrigger("Damage");

    }


    //------------���͎󂯎��-----------
    public void OnMove(InputAction.CallbackContext context) {
        moveAngleContext = context.ReadValue<Vector2>();
        moveAngleContext.z = moveAngleContext.y;
        moveAngleContext.y = 0;

        moveMagnContext = moveAngleContext.magnitude;
        if (moveMagnContext > 1) moveMagnContext = 1;

        moveAngleContext.Normalize();

    }


    public void OnDash(InputAction.CallbackContext context) {
        dashItrContext = context.performed;

        if (context.canceled) {
            dashContext = false;
            dashItrCnt = 0;
            if (dash) DashCancel();
        }

    }

    public void OnJump(InputAction.CallbackContext context) {//�ً}����������Ŕ���
        if (context.started) {
            jumpContext = true;
            jButtonContext = true;
            if (GetPlayerActSt() == PlayerActionState.jumpcharge) {//�W�����v�`���[�W���ɂ�����x�W�����v�������Ƌً}����ɔh��
                jumpChargeCnt = 0;
                jumpContext = false;
                evasionMoveContext = true;
                return;
            }
        }
        if (context.canceled) jButtonContext = false;

    }


    public void OnLook(InputAction.CallbackContext context) {
        viewPoint += context.ReadValue<Vector2>() / 3 * viewRotetionFactor * (focusContext ? zoomInViewRotFactor : 1);
        if (viewPoint.y > 80) viewPoint.y = 80;
        else if (viewPoint.y < -80) viewPoint.y = -80;

        //���_�ړ����f

    }


    //����֘A
    public void OnFire(InputAction.CallbackContext context) {
        if (context.started) fireContext = true;
        if (context.canceled) fireContext = false;
    }

    public void OnFocus(InputAction.CallbackContext context) {
        if (context.performed) focusContext = true;
        else if (context.canceled) focusContext = false;
    }

    public void OnReload(InputAction.CallbackContext context) {
        if (context.performed) {
            selectWep.Reload();

        }
    }
    public void WeaponsListUp(InputAction.CallbackContext context) {
        if (context.started && selWepIdx > 0) {
            selectWep.PutAway();
            selWepIdx--;
            selectWep = weapons[selWepIdx];
            selectWep.Ready();
            if (selectWep.HUDWeaponImage == null)
                weaponImage.material = noDataWeaponImage;
            else
                weaponImage.material = selectWep.HUDWeaponImage;
        }
    }
    public void WeaponsListDown(InputAction.CallbackContext context) {
        if (context.started && selWepIdx + 1 < weapons.Count) {
            selectWep.PutAway();
            selWepIdx++;
            selectWep = weapons[selWepIdx];
            selectWep.Ready();
            if (selectWep.HUDWeaponImage == null)
                weaponImage.material = noDataWeaponImage;
            else
                weaponImage.material = selectWep.HUDWeaponImage;
        }
    }
    //----------------Update�Ȃ�----------------

    void Awake() {
        //�G�ݒ�
        if (setEnemiesShareTarget) Enemy.sharedTarget = this.gameObject;

        //���_��_����
        levelAiming = transform.eulerAngles.y;
        verticalAiming = transform.eulerAngles.x;
        initCockpitLAim = cockpit.eulerAngles.y;
        initCockpitVAim = cockpit.eulerAngles.x;
        initCockpitParentVAim = cockpitParent.eulerAngles.x;

        viewPoint = new Vector2(transform.eulerAngles.y, transform.eulerAngles.x);

        //�A�j���[�^�[
        moveDirForAnim = Vector2.zero;



    }
    void Start() {
        if (anim == null) anim = GetComponent<Animator>();
        if (cc == null) cc = GetComponent<CharacterController>();
        if (gs == null) gs = GetComponent<GroundedSensor>();

        //damageFX
        damageFX = (GameObject)Resources.Load("BO_WeaponSystem/Particles/vulletHit");



        //ArmorPoint
        if (armorPoint > maxArmorPoint) armorPoint = maxArmorPoint;

        //weapons
        if (weapons.Count == 0) {
            weapons.Add(new EmptyWeaponSlot());
        }
        for (int i = 0; i < weapons.Count; i++) {
            if (weapons[i] == null) {
                weapons[i] = new EmptyWeaponSlot();
            }
            weapons[i].setSender(this);
        }
        selectWep = weapons[0];
        selWepIdx = 0;
        selectWep.Ready();

        if (selectWep.HUDWeaponImage == null) {
            weaponImage.material = noDataWeaponImage;
            Debug.Log(selectWep);
        } else {
            weaponImage.material = selectWep.HUDWeaponImage;

        }

        hitResponse.color = new Color(1, 1, 1, 0);



    }

    // Update is called once per frame
    void Update() {
        audio.pitch = this.moving;
        audio2.pitch = this.aligning;
        /*if (this.dashing==false) {
            audio3.mute=true;
        }
        else {
            audio3.mute=false;
        }
        if (this.jumpCharging) {
            audio4.mute = true;
        }
        else {
            audio4.mute = false;
        }*/
        if (Input.GetKey(KeyCode.Alpha1)) {
            GameObject EMP=Instantiate(EMPPrefab);
            EMP.transform.position = this.transform.position;
        }
        if (Input.GetKey(KeyCode.Alpha2)) {
            GameObject SMOKE = Instantiate(SmokePrefab);
            SMOKE.transform.position = this.transform.position;
        }
        if ((wepMoveLiveTime -= Time.deltaTime) < 0) {
            wepMoveLiveTime = 0;
            wepMove = null;
        }


        //�e�q�b�g���̃��e�B�N��
        if (hitResponse.color.r >= 0) {
            hitResponse.color = new Color(
                hitResponse.color.r - Time.deltaTime / 0.3f,
                hitResponse.color.g - Time.deltaTime / 0.3f,
                hitResponse.color.b - Time.deltaTime / 0.3f,
                1);
            if (hitResponse.color.r < 0) hitResponse.color = new Color(0, 0, 0, 1);
        }

        //�`���[�W�n���͉��Z
        //�_�b�V���`���[�W
        if (dashItrContext && dashItrCnt < dashInteractTime) dashItrCnt += Time.deltaTime;
        if (dashItrCnt >= dashInteractTime) {
            dashContext = true;
        }
        //�W�����v�`���[�W
        bool jumpCharge = false;
        if (jumpContext) {
            PlayerActionState pas = GetPlayerActSt();
            switch (pas) {
                //�W�����v���Ȃ��A�W�����v���L�����Z������A�N�V����
                case PlayerActionState.falling:
                case PlayerActionState.jump:
                case PlayerActionState.touchdown:
                case PlayerActionState.evasionmove:
                case PlayerActionState.weaponmove:
                case PlayerActionState.weaponairmove:
                    jumpChargeCnt = 0;
                    jumpContext = false;
                    jump = false;
                    break;

                //�W�����v�`���[�W���J�n�A�p������A�N�V����
                case PlayerActionState.move:
                case PlayerActionState.dash:
                case PlayerActionState.dashcancel:
                case PlayerActionState.dashcharge:
                case PlayerActionState.idle:
                case PlayerActionState.jumpcharge:
                    jumpCharge = true;
                    break;
                default:
                    Debug.LogWarning("[PlayerController] > �W�����v�\�����쒆�ɂ��̃A�N�V�������s���邱�Ƃ�z�肵�Ă��܂���B\n" +
                        "�W�����v�`���[�W������ҏW���Ă��������@(�A�N�V����:" + GetPlayerActSt() + ")");
                    break;
            }
        }
        if (jumpCharge) {
            if (jumpChargeCnt <= 0) OnJumpChargeStarted?.Invoke();
            jumpChargeCnt += Time.deltaTime;
            if (jumpChargeCnt > jumpChargeTime) {
                jump = true;
                jumpChargeCnt = 0;
                jumpContext = false;
                //�O���Q�ƃC�x���g
                OnJumped?.Invoke();
            }
        }


        //���ړ��ʌv�Z
        lastActualMovement = this.transform.position - oldPosition;
        lastActualMovement /= lastTimeDelta;
        //Debug.Log($"{lastMovement}  {lastActualMovement}");

        //���_�ړ��v�Z 

        float laRotVol = Mathf.Abs(viewPoint.x - levelAiming) < 0.001 ? 0f : ((viewPoint.x - levelAiming));
        if (Mathf.Abs(laRotVol) > turningSpeed * Time.deltaTime) laRotVol = turningSpeed * Time.deltaTime * Mathf.Sign(laRotVol);
        levelAiming += laRotVol;


        float vaRotVol = Mathf.Abs(viewPoint.y - verticalAiming) < 0.001 ? 0f : ((viewPoint.y - verticalAiming));
        if (Mathf.Abs(vaRotVol) > turningSpeed * Time.deltaTime) vaRotVol = turningSpeed * Time.deltaTime * Mathf.Sign(vaRotVol);
        verticalAiming += vaRotVol;
        

        aligning = Mathf.Min(Mathf.Max(Mathf.Abs(laRotVol) / (turningSpeed * Time.deltaTime), Mathf.Abs(vaRotVol) / (turningSpeed * Time.deltaTime)), 1);


        transform.eulerAngles = new Vector3(0, levelAiming, transform.eulerAngles.z);
        sightOrigin.transform.localEulerAngles = new Vector3(verticalAiming, 0, 0);
        cockpit.rotation = Quaternion.Euler(
            (-verticalAiming * cockpitVerticalAlignFactor) + (initCockpitParentVAim - cockpitParent.eulerAngles.x)*cockpitVAlignFactorOnParentRot + cockpitVerticalAlignOffset, 
            cockpitParent.rotation.eulerAngles.y, 
            cockpitParent.rotation.eulerAngles.z
        );

        
        //camera
        pilotCamera.transform.eulerAngles = new Vector3(viewPoint.y, viewPoint.x, pilotEyePoint.transform.eulerAngles.z);



        //----------------------------
        //          �ړ��n
        //----------------------------

        //�_�b�V�����͎�����
        //�_�b�V���N�[�����Ȃǃ_�b�V�������͂ł����Ԃ�����
        if (dashContext && moveMagnContext > 0 && dashCTcnt == 0 && !dash) {
            dash = true;
            turningSpeed *= dashTurningFactor;
        }


        //�ړ����͌v�Z
        jumped = false;

        //�J�E���g�A�b�v���_�E��
        evasionMoveTime -= Time.deltaTime;
        if (evasionMoveTime < 0)evasionMoveTime = 0;
        

        if (gs.isGrounded(out groundNormal) && !wallBound && !jump) //�ڒn����
        {
            if (evasionMoveContext && evasionMoveTime <= 0) {
                evasionMoveTime = evasionMoveAllTime;
                evasionMoveAngle = moveAngleContext.normalized;
                evasionMoveContext = false;
                OnEvasionMoved?.Invoke();
            }
            if(evasionMoveTime > 0) {
                movement = evasionMoveAngle * Mathf.Max(evasionMoveSpeed * (float)(evasionMoveTime / evasionMoveAllTime),evasionMoveSpeed * 0.1f);
            }else
            //�_�b�V���N�[���^�C�����łȂ����
            if (dashCTcnt == 0) {

                //��_�b�V����
                if (!dash) {

                    //�ŏ���lastMovement�Ɏ��R�����ʂ��|����
                    Vector3 lm = worldVec2localVec(lastMovement);
                    lm.y = 0;
                    Vector3 dlm = lm.normalized * Mathf.Max(lm.magnitude - lm.magnitude * Time.deltaTime * brake, 0);

                    //���R������̒l������movement�̂ق����傫����΂�������̂�
                    if (moveAngleContext.magnitude * speed < dlm.magnitude) {
                        movement = dlm;
                    } else movement = moveAngleContext * speed;
                }

                //�_�b�V����
                else {

                    //�_�b�V���J�n�t���[���̏ꍇ
                    if (dashAngle == Vector3.zero) {
                        //�_�b�V���J�n���̕�����ۑ�
                        movement = moveAngleContext.normalized * dashSpeed;
                        dashAngle = movement.normalized;
                        //�O���Q�ƃC�x���g
                        OnDashed?.Invoke();
                    }
                    //�_�b�V����
                    else {
                        if (Vector3.Angle(moveAngleContext, dashAngle) > 100) {
                            DashCancel();    //(�_�b�V����������100�x�ȏ�̓��͓]���Ń_�b�V���L�����Z���j
                        } else {
                            //�_�b�V���J�n�t���[���ȊO�͑O�t���[���̕����Ɋ�Â��ĕ���������
                            movement = Vector3.Normalize(dashAngle + moveAngleContext * Time.deltaTime * 2) * dashSpeed;
                            dashAngle = movement.normalized;
                            
                        }
                    }
                }
            }
            //�_�b�V���㌸��
            if (dashCTcnt > 0) {
                dashCTcnt -= Time.deltaTime;
                if (dashCTcnt <= 0) {
                    dashCTcnt = 0;
                }
                if(evasionMoveTime <= 0)
                    movement = inertiaAngle * (speed * 0.7f + (dashSpeed - speed) * (dashCTcnt != 0 ? dashCTcnt : 0.01f / dashCoolTime));
                
            }

            //���n�̏u��
            if (inAir) {

                //groundSensor�����n������o�������A��̊p�x��slopeLimit���z���Ă����ꍇ
                if (Vector3.Angle(groundNormal, Vector3.up) > cc.slopeLimit) {
                    //�����ł�inAir��false�ɂ��Ȃ��B�@if(wallBound) ���Œ��˕Ԃ���v�Z���Ȃ��犊�藎���Ă���
                    wallBoundVector = Vector3.Reflect(lastMovement, groundNormal);
                    wallBound = true;
                }
                //���n�\�Ȓn�ʂ������ꍇ
                else {
                    inAir = false;
                    touchDownCnt = touchDownTime;
                    //�O���Q�ƃC�x���g
                    OnTouchDowned?.Invoke();
                }
            }
            if (touchDownCnt > 0) {//���n��d��
                touchDownCnt -= Time.deltaTime;
                if (touchDownCnt < 0) {
                    touchDownCnt = 0;
                }
                movement = worldVec2localVec(lastMovement) * 0.5f;
            }

            movement.y = -30; //�ڒn���d��

            //wepMove����
            //wepMove�ɉ����֐��������Ă�����
            if (wepMove != null) {
                //wepMove����������ꍇlastMovement��wepMove�����O�Ɋm�肳����
                wepMoving = true;
                if (!inAir && touchDownCnt <= 0) lastMovement = localVec2worldVec(movement);
                movement = wepMove(movement, true, transform);
            } else wepMoving = false;


            //�O���Q�ƒl���� 
            if (inAir || evasionMoveTime > 0) {
                moving = 0;
            }else moving = MathF.Min(new Vector2(movement.x, movement.z).magnitude / speed, 1);

        } else {//�󒆂ɋ���ꍇ
            if (!inAir) {

                if (jump) {
                    gs.Sleep(jumpTime, false);
                    jump = false;
                    jumped = true;  //GetPlayerActSt()���g�p���đ��R���|�[�l���g����W�����v�����m���鎞�̈�
                                    //1�t���[���P�\��݂���(jumped�͎��̃t���[����false�ɂȂ�)
                    inAirCnt = -jumpTime;
                } else inAirCnt = 0;
                inAir = true;
                OnAired?.Invoke();
                lastMovement.y = 0;
            } else {

                if (inAirCnt < 4) inAirCnt += Time.deltaTime * fallSpeedFactor;
            
            }
            airAxeling = false;

            if (wallBound) {
                wallBound = false;
                lastMovement.x = wallBoundVector.x;
                lastMovement.z = wallBoundVector.z;
                //Debug.Log("�͂˂�����");
            } else if (lastMovement.magnitude * 0.99 > lastActualMovement.magnitude) {
                Vector2 horLMDiff = new Vector2(lastMovement.x, lastMovement.z) - new Vector2(lastActualMovement.x, lastActualMovement.z);
                if (horLMDiff.magnitude * 0.1f < horLMDiff.normalized.magnitude) horLMDiff.Normalize();
                else horLMDiff *= 0.1f;
                lastMovement.x = lastActualMovement.x - (horLMDiff.x);
                lastMovement.z = lastActualMovement.z - (horLMDiff.y);
                if (lastActualMovement.y <= 0) {
                    gs.WakeUp();
                }
            } else {
                //�󒆂ŃW�����v�{�^���������Ă���Ԃ̓G�A�A�N�Z���i�ӂ�ӂ�~���j�ɕω�
                if (jButtonContext ) {
                    airAxeling = true;
                    OnAirAxeled?.Invoke();
                    //�������x������
                    if(inAirCnt > airAxelFallSpeed ) inAirCnt = airAxelFallSpeed;
                    //�󒆈ړ����͂̔��f
                    //���͕���*�󒆍ő呬�x��ڕW�̃x�N�g���Ƃ��A���݃x�N�g��(lastMovement)�Ƃ̍����󒆉����x���l�߂Ă���
                    lastMovement.y = 0;
                    lastMovement = worldVec2localVec(lastMovement);
                    Vector3 targetMovement = moveAngleContext.normalized * maxAirAxelSpeed;
                    //���݃x�N�g���̑傫�����󒆍ő呬�x���z���Ă���ꍇ�A
                    //���݃x�N�g���ƖڕW�x�N�g���̊p�x���̏������ilastMovement�EtargetMovement�j�ɉ����ĖڕW�x�N�g���̑傫����傫������
                    float overMagn = lastMovement.magnitude - maxAirAxelSpeed;
                    if (overMagn > 0) targetMovement += targetMovement.normalized * (overMagn * Mathf.Max(0, Vector3.Dot(lastMovement.normalized, targetMovement.normalized)));

                    //Debug.LogWarning($"{targetMovement}  {lastMovement}  {(targetMovement - lastMovement).normalized}");
                    lastMovement += (targetMovement - lastMovement).normalized * airAcceleration * moveAngleContext.magnitude; //�l�߂�Ƃ���
                    lastMovement = localVec2worldVec(lastMovement);
                }
                //�������x�v�Z
                lastMovement.y = fallCalc(inAirCnt);
            }
            //���󒆍ō������_�b�V���X�s�[�h�ȏ�ɂ����Ȃ��悤�ɂ����i�󒆈ړ������ɂ��R�����g�A�E�g�j
            /*Vector2 lmVec;
            if (( lmVec = new Vector2(lastMovement.x, lastMovement.z) ).magnitude > dashSpeed)
            {
                lastMovement = lastMovement.normalized * dashSpeed;
            }*/
            movement = worldVec2localVec(lastMovement);

            if (wepMove != null) {
                wepMoving = true;
                if (!inAir && touchDownCnt <= 0) lastMovement = localVec2worldVec(movement);
                wepMove(movement, false, transform);
            } else wepMoving = false;

            //�O���Q�ƒl����
            moving = 0;
        }




        //�A�j���[�^�[�p
        //�_�b�V���L�����Z��
        Vector3 mdfaDiff = ((movement * (dashCTcnt != 0 ? -1 : 1)) / dashSpeed - moveDirForAnim);
        if (mdfaDiff.magnitude > 0.08f) {
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
        if (!inAir && touchDownCnt <= 0 && !wepMoving) lastMovement = localVec2worldVec(movement);
        movement *= Time.deltaTime;
        movement = localVec2worldVec(movement);
        oldPosition = transform.position;
        cc.Move(movement);

        //sightOrigin�̈ʒu��pilotCamera�ɍ��킹��
        pilotCamera.transform.position = pilotEyePoint.position;
        sightOrigin.transform.position = pilotCamera.transform.position;
        //cockpit.position = cockpitParent.position;

        //���ړ��ʌv�Z�p
        lastTimeDelta = Time.deltaTime;

        //--------------------
        //  ���ړ��@���ˌ�
        //--------------------

        if (fireContext) selectWep.MainAction();
        if (focusContext) selectWep.SubAction();
        //pcc.zoom = focusContext;

        //--------------------
        //�@���ˌ��@�����̑�
        //--------------------
        //Debug.Log($"{movement}");
    }

    //���̑��֐�
    public void DashCancel() {
        dash = false;
        dashCTcnt = dashCoolTime;
        turningSpeed /= dashTurningFactor;
        inertiaAngle = dashAngle;
        dashAngle = Vector3.zero;
        //�O���Q�ƃC�x���g
        OnDashCanceled?.Invoke();
    }

    private float fallCalc(float fallDuration) {

        return -(gravity * fallDuration);
    }

    /// <summary>
    /// ���[�J����ԃx�N�g�������[���h��ԃx�N�g���ɕϊ�
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private Vector3 localVec2worldVec(Vector3 input) {
        //memo: ���ړ�����z�����A�O��ړ�����x�������]���Ă����̂Ŗ��������
        return new Vector3(
            Vector3.Dot(new Vector3(transform.right.x, transform.right.y, -transform.right.z), input),
            Vector3.Dot(transform.up, input),
            Vector3.Dot(new Vector3(-transform.forward.x, transform.forward.y, transform.forward.z), input)
        );
    }
    private Vector3 worldVec2localVec(Vector3 input) {
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
        } else if (touchDownCnt > 0) {
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

    

    /// <summary>
    /// �e�X�g�̈׏����ɍ�����A�N�V�����擾�p���\�b�h�B�񐄏��BGetPlayerActSt()���g����
    /// </summary>
    /// <returns></returns>
    public string getActState() {
        if (inAir) return "in the air";

        if (dashCTcnt > 0) return "dash cancel";
        if (dash) return "dash";
        if (dashItrContext) return "dash interact";
        if (moveMagnContext > 0) return "move";
        return "idle";
    }
    public PlayerActionState GetPlayerActSt() {

        PlayerActionState pas =
            inAir && wepMoving ? PlayerActionState.weaponairmove : 
            wepMoving ? PlayerActionState.weaponmove :

            inAir && lastMovement.y <= 0 ? PlayerActionState.jump :
            inAir ? PlayerActionState.falling :
            touchDownCnt > 0 ? PlayerActionState.touchdown :
            jumpChargeCnt > 0 ? PlayerActionState.jumpcharge :
            jump || jumped ? PlayerActionState.jump :

            dashCTcnt > 0 ? PlayerActionState.dashcancel :
            dash ? PlayerActionState.dash :
            dashItrContext ? PlayerActionState.dashcharge :
            moveMagnContext > 0 ? PlayerActionState.move :
            PlayerActionState.idle;

        return pas;
    }

    //���ā[�Ǝ擾

    public float moving { get; private set; } = 0;
    public bool dashing { get { return dash && dashCTcnt <= 0 && evasionMoveTime <= 0 && !wepMoving; }}
    public float aligning { get; private set; } = 0;
    public bool airing { get { return inAir; } }
    public bool jumpCharging { get { return jumpChargeCnt > 0; }}
    public bool airAxeling { get; private set; } = false;


    public delegate void PlayerStateHandler();
    public PlayerStateHandler OnJumpChargeStarted;
    public PlayerStateHandler OnJumped;
    public PlayerStateHandler OnEvasionMoved;
    public PlayerStateHandler OnDashed;
    public PlayerStateHandler OnDashCanceled;
    public PlayerStateHandler OnTouchDowned;
    public PlayerStateHandler OnAired;
    public PlayerStateHandler OnAirAxeled;


}
