using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class LiveBullet : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 initialPosition;
    public Vector3 initialVelocity;
    public Vector3 velocity;
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

    [SerializeField] protected GameObject HitInitPref;

    public static bool GET_PREFAB = false;
    public static GameObject PR_LIVEBULLET;

    public static bool psDestroyed = false;

    protected Vector3 np = Vector3.zero;

    protected bool hit = false;
    protected bool destroyReady = false;

    public static LiveBullet BulletInstantiate(
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

        LiveBullet bulletLB = bullet.GetComponent<LiveBullet>();

        bulletLB.initialPosition = _initialPosition;
        bulletLB.initialVelocity = _initialVelocity;
        bulletLB.velocity = _initialVelocity;
        bulletLB.shooter = _shooter;
        bulletLB.damage = _damage;
        bulletLB.generatedTime = Time.frameCount;

        
        bulletLB.tr = bullet.GetComponent<TrailRenderer>();

        return bulletLB;
    }

    public static LiveBullet BulletInstantiate(
        Weapon sender,
        Transform initialPosition,
        Vector3 initialVelocity,
        int damage
    )
    {
        Debug.Log(initialPosition);
        LiveBullet lb = LiveBullet.BulletInstantiate(sender, initialPosition.position, initialVelocity,damage);
        lb.ps1.GetComponent<Transform>().SetParent(initialPosition);
        lb.ps2.GetComponent<Transform>().SetParent(initialPosition);
        lb.ps3.GetComponent<Transform>().SetParent(initialPosition);
        return lb;

    }
    

    public static Vector3 BullisticCalc(Vector3 velocity, float time) { //後回し
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
    }

    public void OnTriggerEnter(Collider other) {
        if (other.CompareTag("IgnoreCollision"))
        {
            return;
        }
        if (!hit) {

            RaycastHit[] results = new RaycastHit[10];
            int lMask = 1 << 6 | 1; //Layer6 : PlayerBody, Layer1 : Dafault
            int length = Physics.RaycastNonAlloc(new Ray(transform.position, transform.forward), results, (np - transform.position).magnitude, lMask);

            float minDist = (np - transform.position).magnitude + 100;
            int colIdx = -1;
            for(int i = 0; i < length; i++)
            {
                if (results[i].collider == cc) continue;
                if (minDist > (results[i].transform.position - transform.position).magnitude)
                {
                    minDist = results[i].transform.position.magnitude;
                    colIdx = i;
                }

            }
            if (colIdx != -1) hit = true;
            else return;

            DamageReceiver dr;
            if ((dr = other.GetComponent<DamageReceiver>()) != null) {
                Vector3 hitPos = results[colIdx].point;
                dr.Damage(damage, hitPos, shooter?.gameObject, "LiveBullet");
                shooter.sender.ThrowHitResponse(this.gameObject, other.gameObject );
            }

            if(HitInitPref != null)Instantiate(HitInitPref, results[colIdx].point, transform.rotation, null);
            else Instantiate(new GameObject(), results[colIdx].point, transform.rotation, null);

        }
       

    }

    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate() {

        transform.position = np == Vector3.zero ? transform.position : np;
        
        if (age < 1) {
            //ps1.gameObject.transform.position = initialPosition;
            //ps2.gameObject.transform.position = initialPosition;
            ps1.transform.localScale = Vector3.one;
            ps2.transform.localScale=Vector3.one;
        }

        if(age < 2) {
            //ps3.gameObject.transform.position = initialPosition;
            ps3.transform.localScale = Vector3.one;
        }

        if (destroyReady) {
            Destroy(this.gameObject);
        }

        if (hit) {
            destroyReady = true;
        }

        age += Time.deltaTime;

        velocity.y -= 9.8f * Time.deltaTime;
        velocity.y -= velocity.y * 0.4f * Time.deltaTime;
        velocity.x -= velocity.x * 0.4f * Time.deltaTime;
        velocity.z -= velocity.z * 0.4f * Time.deltaTime;
        Vector3 Movement = velocity * Time.deltaTime;
        Vector3 op = transform.position;
        //Debug.Log($" {velocity}  {Movement}");
        np = op + Movement;
        cc.height = Movement.magnitude;
        cc.center = new Vector3(0, 0, Movement.magnitude / 2f);


        if (transform.position.y < -10)
        {
            Destroy(this.gameObject);
        }
        if(age > 1 && !psDestroyed)
        {
            Destroy(ps1.gameObject);
            Destroy(ps2.gameObject);
            psDestroyed= true;
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

    private void OnDestroy()
    {
        if (ps1.gameObject) Destroy(ps1.gameObject);
        if (ps2.gameObject) Destroy(ps2.gameObject);
        if (ps2.gameObject) Destroy(ps3.gameObject);
    }
}
