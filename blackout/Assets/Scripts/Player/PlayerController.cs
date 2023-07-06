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
    //計算用
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

    //入力コンテキスト
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

    //------------継承-------------
    public Animator getAnim() {
        return anim;
    }
    public string getWepUseAnimLayer() {
        return "right_arm";
    }
    public GameObject getAimingObj() {
        return sightOrigin;
    }
    

    //------------入力受け取り-----------
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

        //視点移動反映
        
    }

    //武器関連
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
    //----------------Updateなど----------------

    void Awake()
    {
        //敵設定
        if(setEnemiesShareTarget)Enemy.sharedTarget = this.gameObject;

        //視点基点決定
        levelAiming = transform.eulerAngles.y;
        verticalAiming = transform.eulerAngles.x;

        viewPoint = new Vector2(transform.eulerAngles.y, transform.eulerAngles.x);

        //アニメーター
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
        //実移動量計算
        lastActualMovement = this.transform.position - oldPosition;
        lastActualMovement /= lastTimeDelta;
        //Debug.Log($"{lastMovement}  {lastActualMovement}");
        
        //視点移動計算

        float laRotVol = Mathf.Abs(viewPoint.x - levelAiming) < 0.01 ? 0f : ((viewPoint.x - levelAiming)*10) * Time.deltaTime;
        if (Mathf.Abs(laRotVol) > turningSpeed * Time.deltaTime) laRotVol = turningSpeed * Time.deltaTime * Mathf.Sign(laRotVol);
        levelAiming += laRotVol;

        float vaRotVol = Mathf.Abs(viewPoint.y - verticalAiming) < 0.01 ? 0f : ((viewPoint.y - verticalAiming) * 10) * Time.deltaTime;
        if (Mathf.Abs(vaRotVol) > turningSpeed * Time.deltaTime) vaRotVol = turningSpeed * Time.deltaTime * Mathf.Sign(vaRotVol);
        verticalAiming += vaRotVol;


        transform.eulerAngles = new Vector3(0, levelAiming, transform.eulerAngles.z);
        sightOrigin.transform.localEulerAngles = new Vector3(verticalAiming, 0, 0);

        //testcam用
        testcam.transform.eulerAngles = new Vector3(viewPoint.y, viewPoint.x, testcam.transform.eulerAngles.z);



        //----------------------------
        //          移動系
        //----------------------------

        //ダッシュ入力時判定
        //ダッシュクール中などダッシュが入力できる状態か判定
        if (dashContext && moveMagnContext > 0 && dashCTcnt == 0 && !dash)
        {
            dash = true;
            turningSpeed *= dashTurningFacter;
        }


        //移動入力計算

        if (gs.isGrounded(out groundNormal) && !wallBound) //接地判定
        {
            
            if (dashCTcnt == 0) {
                if (!dash)//非ダッシュ時
                {
                    movement = moveAngleContext * speed;
                } else//ダッシュ時→
                  {
                    if (dashAngle == Vector3.zero)   //ダッシュ開始フレームの場合
                    {
                        movement = moveAngleContext.normalized * dashSpeed;
                        dashAngle = movement.normalized;
                    } else                            //ダッシュ中
                      {
                        if (Vector3.Angle(moveAngleContext, dashAngle) > 100) {
                            DashCancel();    //ダッシュ方向から100度以上の入力転換（ダッシュキャンセル）
                        } else {
                            movement = Vector3.Normalize(dashAngle + moveAngleContext * Time.deltaTime * 2) * dashSpeed;
                            dashAngle = movement.normalized;
                        }
                    }
                }
            }
            if (dashCTcnt > 0)    //ダッシュ後減速
            {
                dashCTcnt -= Time.deltaTime;
                if (dashCTcnt <= 0) {
                    dashCTcnt = 0;
                    inertiaAngle = Vector3.zero;
                }
                movement = inertiaAngle * (speed * 0.7f + (dashSpeed - speed) * (dashCTcnt != 0 ? dashCTcnt : 0.01f / dashCoolTime));
            }
            if (inAir) {//着地の瞬間
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


            movement.y = -30;//接地時重力
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
                //Debug.Log("はねかえり");
            }
            else if (lastMovement.magnitude * 0.99 > lastActualMovement.magnitude) {
                Vector2 horLMDiff = new Vector2(lastMovement.x, lastMovement.z) - new Vector2(lastActualMovement.x, lastActualMovement.z);
                if (horLMDiff.magnitude * 0.1f < horLMDiff.normalized.magnitude) horLMDiff.Normalize();
                else horLMDiff *= 0.1f;
                lastMovement.x = lastActualMovement.x - (horLMDiff.x);
                lastMovement.z = lastActualMovement.z - (horLMDiff.y);
                
                //Debug.Log("つっかえ " + lastActualMovement);
            } else {
                //Debug.Log("Notつっかえ");
            }
            movement = worldVec2localVec(lastMovement);

            

            
            
            
        }

        //アニメーター用
        //ダッシュキャンセル
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
        //モーション管理
        MotionChange();

        //移動反映
        if(!inAir && touchDownCnt <= 0)lastMovement = localVec2worldVec(movement);
        movement *= Time.deltaTime;
        movement = localVec2worldVec(movement);
        oldPosition = transform.position;
        cc.Move(movement);

        //testcam用
        sightOrigin.transform.position = testcam.transform.position;

        //実移動量計算用
        lastTimeDelta = Time.deltaTime;

        //--------------------
        //  ↑移動　↓射撃
        //--------------------

        if (fireContext) selectWep.MainAction();
    }

    //その他関数
    public void DashCancel()
    {
        dash = false;
        dashCTcnt = dashCoolTime;
        turningSpeed /= dashTurningFacter;
        inertiaAngle = dashAngle;
        dashAngle = Vector3.zero;
    }
    /// <summary>
    /// ローカル空間ベクトルをワールド空間ベクトルに変換
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private Vector3 localVec2worldVec(Vector3 input)
    {
        //memo: 横移動時にz軸が、前後移動時にx軸が反転していたので無理矢理解消
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
