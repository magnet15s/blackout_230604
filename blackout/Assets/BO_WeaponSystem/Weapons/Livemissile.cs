using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.SceneManagement;
using UnityEngine;

public class Livemissile : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Transform target;
    [SerializeField, Min(0)]
    float time = 0.5f;
    [SerializeField]
    float lifeTime = 2;
    [SerializeField]
    bool limitAcceleration = true;
    [SerializeField, Min(0)]
    float maxAcceleration = 1;
    [SerializeField]
    Vector3 minInitVelocity;
    [SerializeField]
    Vector3 maxInitVelocity;
    public string weapon_name;

    Vector3 position;
    Vector3 velocity;
    Vector3 acceleration;
    Transform thisTransform;


    
    public Weapon shooter;
    public int damage;
    public float generatedTime;
    public float age;
    //public LineRenderer line;
    //public Vector3[] linePosArray;

    public TrailRenderer tr;
    public CapsuleCollider cc;

    public ParticleSystem ps1;
    public ParticleSystem ps2;
    public ParticleSystem ps3;
    public GameObject ps4;

    public static bool GET_PREFAB = false;
    public static GameObject PR_LIVEBULLET;

    public static bool psDestroyed = false;

    private Vector3 np = Vector3.zero;

    private bool hit = false;
    private bool destroyReady = false;

    
    public Transform Target {
        set {
            target = value;
        }
        get {
            return target;
        }
    }
    /*public static Livemissile BulletInstantiate(
        Weapon _shooter,
        Vector3 _initialPosition,
        Vector3 _initialVelocity, 
        int _damage 
    ){


        if (!GET_PREFAB) {
            PR_LIVEBULLET = ((GameObject)Resources.Load("BO_WeaponSystem/Prefabs/PrLiveBullet")) ?? new GameObject { name = "Bullet Load Error" };
            GET_PREFAB = true;
        }

        GameObject bullet = Instantiate(PR_LIVEBULLET);

        bullet.name += _shooter.sender.ToString() + " " + Time.frameCount.ToString();
        bullet.transform.position = _initialPosition;
        bullet.transform.LookAt(_initialPosition + _initialVelocity);

        Livemissile bulletLB = bullet.GetComponent<Livemissile>();

        bulletLB.initialPosition = _initialPosition;
        bulletLB.initialVelocity = _initialVelocity;
        bulletLB.velocity = _initialVelocity;
        bulletLB.shooter = _shooter;
        bulletLB.damage = _damage;
        bulletLB.generatedTime = Time.frameCount;

        
        bulletLB.tr = bullet.GetComponent<TrailRenderer>();

        return bulletLB;
    }*/


    /*public static Vector3 BullisticCalc(Vector3 velocity, float time) { //後回し
        Vector3 aveVelocity = Vector3.zero;
        for(int i = 0; i < 10; i++) {
            //Xの平均velocityを求める
        }
        for(int i = 0; i < 10; i++) {
            //Zの平均velocityを求める
        }
        //Yをシミュレートする

        //平均*time
        return new Vector3();
    }*/

    

    void Start()
    {
        thisTransform = transform;
        position = thisTransform.position;
        velocity = new Vector3(UnityEngine.Random.Range(minInitVelocity.x, maxInitVelocity.x), UnityEngine.Random.Range(minInitVelocity.y, maxInitVelocity.y), UnityEngine.Random.Range(minInitVelocity.z, maxInitVelocity.z));
        StartCoroutine(nameof(Timer));
    }

    // Update is called once per frame
    void Update() {

        if (target == null) {
            return;
        }
        acceleration = 2f / (time * time) * (target.position - position - time * velocity);

        if (limitAcceleration && acceleration.sqrMagnitude > maxAcceleration * maxAcceleration) {
            acceleration = acceleration.normalized * maxAcceleration;
        }

        time -= Time.deltaTime;

        if (time < 0f) {
            return;
        }

        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        thisTransform.position = position;
        thisTransform.rotation = Quaternion.LookRotation(velocity);
        

        Vector3 Movement = velocity * Time.deltaTime;

        if (destroyReady) {
            Destroy(this.gameObject);
            
        }

        if (hit) {
            destroyReady = true;
        }

        age += Time.deltaTime;
        cc.height = Movement.magnitude;
        cc.center = new Vector3(0, 0, Movement.magnitude / 2f);
        /*if (transform.position.y < -10)
        {
            Destroy(this.gameObject);
        }*/
        if (age > 2 && !psDestroyed)
        {
            Destroy(ps1.gameObject);
            Destroy(ps2.gameObject);
            
            psDestroyed = true;
        }
        if (age > 2 && ps3 != null) {
            Destroy(ps3.gameObject);
            
            ps3 = null;
        }
        if(age > 5)
        {
            
            Destroy(this.gameObject);
        }
    }
    IEnumerator Timer() {
        yield return new WaitForSeconds(lifeTime);
        
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other) {
        if (age > 0.15) {
            hit = true;
            GameObject EMP = Instantiate(ps4);
            EMP.transform.position = this.transform.position;
            DamageReceiver dr;
            Debug.Log("hit!");

            dr = other.GetComponent<DamageReceiver>();
            Debug.Log("dr:" + dr);
            Vector3 hitPos = other.ClosestPointOnBounds(transform.position);
            dr.Damage(damage, hitPos, gameObject, "Livemissile");
            shooter.sender.ThrowHitResponse(this.gameObject, other.gameObject);
        }
        if (!other.CompareTag("IgnoreCollision")) {
            return;
        }
        if (!hit) {


            
            if (other.gameObject.tag != "Player") {

            }


        }
    }

}
