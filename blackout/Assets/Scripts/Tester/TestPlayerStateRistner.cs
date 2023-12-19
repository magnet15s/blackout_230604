using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class TestPlayerStateRistner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] PlayerController pc;
    public bool active = true;
    void Start()
    {
        //各イベント発火時に実行させるコールバック関数を登録
        pc.OnJumped +=  oj;
        pc.OnJumpChargeStarted += ojcs;
        pc.OnEvasionMoved += oem;
        pc.OnDashed += od;
        pc.OnDashCanceled += odc;
        pc.OnTouchDowned += otd;
        pc.OnAired += oa;
    }

    // Update is called once per frame
    void Update()
    {
        //各ステートの取得
        //moving : 0~1  dashing : boolean  aligning : 0~1  airing : boolean
        if(active)Debug.Log($"moving:{pc.moving} dashing:{pc.dashing} aligning:{pc.aligning} airing:{pc.airing} ");
    }

    void ojcs() {
        Debug.Log("OnJumpCharge");
    }

    void oj() {
        Debug.Log("OnJump");
    }
    void oem() {
        Debug.Log("OnEvaseionMove");
    }
    void od() {
        Debug.Log("OnDash");
    }
    void odc() {
        Debug.Log("OnDashCharge");
    }

    void otd() {
        Debug.Log("OnTouchDown");
    }
    void oa() {
        Debug.Log("OnAir");   
    }
    /*
        public PlayerStateHandler OnJumpChargeStarted;
        public PlayerStateHandler OnJumped;
        public PlayerStateHandler OnEvasionMoved;
        public PlayerStateHandler OnDashed;
        public PlayerStateHandler OnDashCanceled;
        public PlayerStateHandler OnTouchDowned;
        public PlayerStateHandler OnAired;*/
}
