using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LiveBullet : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 initialVelocity;
    public Vector3 velocity;
    public Weapon shooter;
    public float damage;
    public float generatedTime;
    public int age;

    public static LiveBullet BulletInstantiate(
        Weapon _shooter,
        Vector3 _initialPosition,
        Vector3 _initialVelocity, 
        float _damage 
    ){
        GameObject bullet = new GameObject()
        {
            name = "bullet" + _shooter.sender.ToString() + " " + Time.frameCount.ToString()
        };
        
        bullet.transform.position = _initialPosition;

        bullet.AddComponent<LiveBullet>();
        LiveBullet bulletLB = bullet.GetComponent<LiveBullet>();

        bulletLB.initialVelocity = _initialVelocity;
        bulletLB.velocity = _initialVelocity;
        bulletLB.shooter = _shooter;
        bulletLB.damage = _damage;
        bulletLB.generatedTime = Time.frameCount;

        return bulletLB;
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity.y -= 9.8f * Time.deltaTime;
        velocity.x -= velocity.x * 0.9f * Time.deltaTime;
        velocity.z -= velocity.z * 0.9f * Time.deltaTime;
        Vector3 np = transform.position;
        transform.position = new Vector3(np.x + velocity.x, np.y + velocity.y, np.z + velocity.z);
        Debug.DrawRay(np, velocity, Color.yellow, 0.1f);
        if(transform.position.y < -10) {
            Destroy(this.gameObject);
        }
    }
}
