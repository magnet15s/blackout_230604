using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class TestPlayerStateRistner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] PlayerController pc;
    public bool active = true;
    public AudioClip sound1;
    public AudioClip sound2;
    public AudioClip sound3;
    public AudioClip sound4;
    public AudioClip sound5;
    AudioSource audioSource;
    public ParticleSystem par1;
    public ParticleSystem par2;
    public ParticleSystem par3;
    public GameObject tyakuchiPrefab;
    public GameObject airPrefab;
    public ParticleSystem par4;
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
        pc.OnAirAxeled += oaa;
        audioSource = GetComponent<AudioSource>();
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
        audioSource.PlayOneShot(sound2);
        par1.gameObject.SetActive(false);
        par2.gameObject.SetActive(false);
        par3.gameObject.SetActive(false);
        par4.gameObject.SetActive(false);

    }
    void oem() {
        Debug.Log("OnEvaseionMove");
        audioSource.PlayOneShot(sound5);
    }
    void od() {
        Debug.Log("OnDash");
        //audioSource.PlayOneShot(sound4);
    }
    void odc() {
        Debug.Log("OnDashCharge");
    }

    void otd() {
        Debug.Log("OnTouchDown");
        audioSource.PlayOneShot(sound1);
        par1.gameObject.SetActive(true);
        par2.gameObject.SetActive(true);
        par3.gameObject.SetActive(true);
        par4.gameObject.SetActive(true);
        Instantiate(tyakuchiPrefab, this.transform.position, Quaternion.identity);

    }
    void oa() {
        Debug.Log("OnAir");   
    }

    void oaa() {
        Debug.Log("OnAirAxel");
        Instantiate(airPrefab, this.transform.position, Quaternion.identity);
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
