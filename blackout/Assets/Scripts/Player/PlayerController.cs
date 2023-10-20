using Cinemachine;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        touchdown = 9
    } 

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
    [SerializeField] private Transform pilotEyePoint;
    [SerializeField] private PlayerCameraCood pcc;
    [Space]
    public List<Weapon> weapons;
    [Space]
    //移動計算用
    private float gravity = 9.8f;
    private bool inAir = false;
    [SerializeField]private float inAirCnt = 0;
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

    [SerializeField] private Weapon selectWep;
    [SerializeField] private int selWepIdx;
    private List<bool> WepActReqBools = new();

    //入力コンテキスト
    private Vector3 moveAngleContext;
    private float moveMagnContext;
    private bool dashContext = false;
    private bool dashItrContext = false;
    private float dashItrCnt = 0;
    private bool jumpContext = false;
    private float jumpChargeCnt = 0;
    private bool evasionMoveContext = false;
    private bool fireContext = false;
    private Vector2 viewPoint;
    private bool focusContext = false;
    private float focusMagn;

    private float levelAiming;
    private float verticalAiming;


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
    [SerializeField, Tooltip("ジャンプのおおよその上昇時間")] private float jumpTime = 2;
    [SerializeField] private float jumpChargeTime = 0.3f;
    
    [Space]
    [SerializeField] private float turningSpeed = 200;
    [SerializeField] private float dashTurningFactor = 0.4f;
    [SerializeField] private Vector2 viewRotetionFactor = new Vector2(10, 10);
    [SerializeField] private float zoomInViewRotFactor = 0.5f;
    [SerializeField] private float maxFocusMagn = 2;
    [Space]
    public GameObject pilotCamera;
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
    public void ThrowHitResponse()
    {
        hitResponse.color = new Color(1, 1, 1, 1);
    }

    bool WeaponUser.RequestWepAction()
    {
        return true;
    }
    public event EventHandler WepActionCancel;
    void OnWepActionCancel(EventArgs e) {
        WepActionCancel.Invoke(this, e);
    }

    private struct MoveORReq {
        public Vector3 Movement;
        public float time;
        public Vector3 weight;
        public bool gOnly;
        public bool running;
        public MoveORReq(Vector3 m, float t, Vector3 w, bool g, bool r) {
            Movement = m;
            time = t;
            weight = w;
            gOnly = g;
            running = r;
        }
    }
    private List<MoveORReq> MoveORReqs = new List<MoveORReq>();
    void WeaponUser.ReqMoveOverrideForWepAct(UnityEngine.Vector3 localMovement, float time, UnityEngine.Vector3 overrideWeight, bool groundedOnly) {
        MoveORReqs.Add(new MoveORReq(localMovement, time, overrideWeight, groundedOnly, false));
    }


    public void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType)
    {
        Debug.Log("Damage!! " + Time.frameCount);
        armorPoint -= damage;
        GameObject dfx;
        (dfx = Instantiate(damageFX, hitPosition, Quaternion.identity)).transform.LookAt(hitPosition + (hitPosition - transform.position));
        dfx.transform.localScale = new Vector3(3, 3, 3);


        statusView.gameObject.GetComponent<Animator>().SetFloat("Armor", (float)armorPoint / (float)maxArmorPoint);
        statusView.gameObject.GetComponent<Animator>().SetTrigger("Damage");

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


    public void OnDash(InputAction.CallbackContext context)
    {
        dashItrContext = context.performed;
        
        if (context.canceled) {
            dashContext = false;
            dashItrCnt = 0;
            if (dash) DashCancel();
        }
        
    }

    public void OnJump(InputAction.CallbackContext context) {//緊急回避もここで判定
        if (context.started) {
            jumpContext = true;
            
            if (GetPlayerActSt() == PlayerActionState.jumpcharge) {//ジャンプチャージ中にもう一度ジャンプを押すと緊急回避に派生
                jumpChargeCnt = 0;
                jumpContext = false;
                evasionMoveContext = true;
                return;
            }

        }

        
    }


    public void OnLook(InputAction.CallbackContext context)
    {
        viewPoint += context.ReadValue<Vector2>() / 3 * viewRotetionFactor * (focusContext ? zoomInViewRotFactor : 1);
        if (viewPoint.y > 80) viewPoint.y = 80;
        else if (viewPoint.y < -80) viewPoint.y = -80;

        //視点移動反映
        
    }


    //武器関連
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.started) fireContext = true; 
        if(context.canceled)fireContext = false;
    }

    public void OnFocus(InputAction.CallbackContext context)
    {
        if(context.performed) focusContext = true;
        else if(context.canceled) focusContext = false;
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
            if (selectWep.HUDWeaponImage == null)
                weaponImage.material = noDataWeaponImage;
            else
                weaponImage.material = selectWep.HUDWeaponImage;
        }
    }
    public void WeaponsListDown(InputAction.CallbackContext context) {
        if (context.started && selWepIdx+1 < weapons.Count) {
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

        

    }
    void Start()
    {
        if (anim == null) anim = GetComponent<Animator>();
        if (cc == null) cc = GetComponent<CharacterController>();
        if (gs == null) gs = GetComponent<GroundedSensor>();

        //damageFX
        damageFX = (GameObject)Resources.Load("BO_WeaponSystem/Particles/vulletHit");



        //ArmorPoint
        if (armorPoint > maxArmorPoint) armorPoint = maxArmorPoint;

        //weapons
        if (weapons.Count == 0)
        {
            weapons.Add(new EmptyWeaponSlot());
        }
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i] == null)
            {
                weapons[i] = new EmptyWeaponSlot();
            }
            weapons[i].setSender(this);
        }
        selectWep = weapons[0];
        selWepIdx = 0;
        selectWep.Ready();

        if (selectWep.HUDWeaponImage == null)
        {
            weaponImage.material = noDataWeaponImage;
            Debug.Log(selectWep);
        }
        else
        {
            weaponImage.material = selectWep.HUDWeaponImage;

        }
        
        hitResponse.color = new Color(1, 1, 1, 0);

        

    }

    // Update is called once per frame
    void Update()
    {
        //MoveOverrideReqestsの処理

        if(MoveORReqs.Count >= 1) {
            List<MoveORReq> remList = new();
            MoveORReqs.ForEach(item => {
                item.time -= Time.deltaTime;
                if (item.time <= 0) {
                    remList.Add(item);
                    return;
                }
                if (item.Equals(MoveORReqs.First())) {
                    item.running = true;
                } else {
                    remList.Add(item);
                    return;
                }
            });
            if(remList.Count > 0) {
                remList.ForEach(item => {
                    MoveORReqs.Remove(item);
                });
                remList.Clear();
            }
        }


        //弾ヒット時のレティクル
        if(hitResponse.color.r >= 0) {
            hitResponse.color = new Color(
                hitResponse.color.r - Time.deltaTime / 0.3f,
                hitResponse.color.g - Time.deltaTime / 0.3f,
                hitResponse.color.b - Time.deltaTime / 0.3f,
                1);
            if (hitResponse.color.r < 0) hitResponse.color = new Color(0, 0, 0, 1);
        }

        //チャージ系入力加算
        //ダッシュチャージ
        if (dashItrContext && dashItrCnt < dashInteractTime) dashItrCnt += Time.deltaTime;
        if (dashItrCnt >= dashInteractTime) {
            dashContext = true;
        }
        //ジャンプチャージ
        bool jumpCharge = false;
        if (jumpContext)
        {
            PlayerActionState pas = GetPlayerActSt();
            switch (pas)
            {
                //ジャンプしない、ジャンプをキャンセルするアクション
                case PlayerActionState.falling:
                case PlayerActionState.jump:
                case PlayerActionState.touchdown:
                case PlayerActionState.evasionmove:
                    jumpChargeCnt = 0;
                    jumpContext = false;
                    jump = false;
                    break;

                //ジャンプチャージを開始、継続するアクション
                case PlayerActionState.move:
                case PlayerActionState.dash:
                case PlayerActionState.dashcancel:
                case PlayerActionState.dashcharge:
                case PlayerActionState.idle:
                case PlayerActionState.jumpcharge:
                    jumpCharge = true;
                    break;
                default :
                    Debug.LogWarning("[PlayerController] > ジャンプ予備動作中にこのアクションが行われることを想定していません。\n" +
                        "ジャンプチャージ処理を編集してください　(アクション:" + GetPlayerActSt() + ")");
                    break;
            }
        }
        if (jumpCharge) {
            jumpChargeCnt += Time.deltaTime;
            if (jumpChargeCnt > jumpChargeTime) {
                jump = true;
                jumpChargeCnt = 0;
                jumpContext = false;
            }
        }
        

        //実移動量計算
        lastActualMovement = this.transform.position - oldPosition;
        lastActualMovement /= lastTimeDelta;
        //Debug.Log($"{lastMovement}  {lastActualMovement}");

        //視点移動計算 

        float laRotVol = Mathf.Abs(viewPoint.x - levelAiming) < 0.001 ? 0f : ((viewPoint.x - levelAiming));
        if (Mathf.Abs(laRotVol) > turningSpeed * Time.deltaTime) laRotVol = turningSpeed * Time.deltaTime * Mathf.Sign(laRotVol);
        levelAiming += laRotVol;

        float vaRotVol = Mathf.Abs(viewPoint.y - verticalAiming) < 0.001 ? 0f : ((viewPoint.y - verticalAiming));
        if (Mathf.Abs(vaRotVol) > turningSpeed * Time.deltaTime) vaRotVol = turningSpeed * Time.deltaTime * Mathf.Sign(vaRotVol);
        verticalAiming += vaRotVol;


        transform.eulerAngles = new Vector3(0, levelAiming, transform.eulerAngles.z);
        sightOrigin.transform.localEulerAngles = new Vector3(verticalAiming, 0, 0);

        //camera
        pilotCamera.transform.eulerAngles = new Vector3(viewPoint.y, viewPoint.x, pilotEyePoint.transform.eulerAngles.z);
        


        //----------------------------
        //          移動系
        //----------------------------

        //ダッシュ入力時判定
        //ダッシュクール中などダッシュが入力できる状態か判定
        if (dashContext && moveMagnContext > 0 && dashCTcnt == 0 && !dash)
        {
            dash = true;
            turningSpeed *= dashTurningFactor;
        }


        //移動入力計算
        jumped = false;
        if (gs.isGrounded(out groundNormal) && !wallBound && !jump) //接地判定
        {
            
            if (dashCTcnt == 0) {
                if (!dash)//非ダッシュ時
                {
                    Vector3 lm = worldVec2localVec(lastMovement);
                    lm.y = 0;
                    Vector3 dlm = lm.normalized * Mathf.Max(lm.magnitude - lm.magnitude * Time.deltaTime * brake, 0);
                    if (moveAngleContext.magnitude * speed < dlm.magnitude)
                    {
                        movement = dlm;
                    }
                    else movement = moveAngleContext * speed;
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
                    //inertiaAngle = Vector3.zero;
                }
                movement = inertiaAngle * (speed * 0.7f + (dashSpeed - speed) * (dashCTcnt != 0 ? dashCTcnt : 0.01f / dashCoolTime));
            }
            if (inAir) {//着地の瞬間
                if(Vector3.Angle(groundNormal, Vector3.up) > cc.slopeLimit) {//groundSensorが着地判定を出したが、坂の角度がslopeLimitを越えていた場合
                    //ここではinAirはfalseにしない。　if(wallBound) 部で跳ね返りを計算しながら滑り落ちていく
                    wallBoundVector = Vector3.Reflect(lastMovement, groundNormal);
                    wallBound = true;
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
            
            movement.y = -30; //接地時重力


        } else {//空中に居る場合
            if (!inAir) {
                
                if (jump) {
                    gs.Sleep(jumpTime, false);
                    jump = false;
                    jumped = true;  //GetPlayerActSt()を使用して他コンポーネントからジャンプを検知する時の為
                                    //1フレーム猶予を設ける(jumpedは次のフレームでfalseになる)
                    inAirCnt = -jumpTime;
                }
                else inAirCnt = 0;
                inAir = true;
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
                if(lastActualMovement.y < 0) {
                    gs.WakeUp();
                }
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

        //sightOriginの位置をpilotCameraに合わせる
        pilotCamera.transform.position = pilotEyePoint.position;
        sightOrigin.transform.position = pilotCamera.transform.position;

        //実移動量計算用
        lastTimeDelta = Time.deltaTime;

        //--------------------
        //  ↑移動　↓射撃
        //--------------------

        if (fireContext) selectWep.MainAction();
        if (focusContext)selectWep.SubAction();
        //pcc.zoom = focusContext;

        //--------------------
        //　↑射撃　↓その他
        //--------------------
        //Debug.Log($"{movement}");
    }

    //その他関数
    public void DashCancel()
    {
        dash = false;
        dashCTcnt = dashCoolTime;
        turningSpeed /= dashTurningFactor;
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

    /// <summary>
    /// テストの為初期に作ったアクション取得用メソッド。非推奨。GetPlayerActSt()を使う事
    /// </summary>
    /// <returns></returns>
    public string getActState()
    {
        if (inAir)          return "in the air";
        
        if (dashCTcnt > 0)  return "dash cancel";
        if (dash)           return "dash";
        if (dashItrContext) return "dash interact";
        if (moveMagnContext > 0) return "move";
        return "idle";
    }
    public PlayerActionState GetPlayerActSt() {

        PlayerActionState pas =
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

    
    
}
