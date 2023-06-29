using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

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

    public static bool GET_PREFAB = false;
    public static GameObject PR_LIVEBULLET;

    public static bool psDestroyed = false;

    private Vector3 np = Vector3.zero;

    private bool hit = false;
    private bool destroyReady = false;

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

    public void OnTriggerEnter(Collider other) {
        if (!hit) {
            Vector3 hitPos = other.ClosestPointOnBounds(transform.position);
            hit = true;
            DamageReceiver dr;
            if ((dr = other.GetComponent<DamageReceiver>()) != null) {
                dr.Damage(damage, hitPos, shooter.gameObject, "LiveBullet");
            }
        }
       

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate() {

        transform.position = np == Vector3.zero ? transform.position : np;
        
        if (age < 1) {
            ps1.gameObject.transform.position = initialPosition;
            ps2.gameObject.transform.position = initialPosition;
        }
        if(age < 2) {
            ps3.gameObject.transform.position = initialPosition;
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
}
