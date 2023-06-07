using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LiveBullet : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 initialVelocity = Vector3.zero;
    public Vector3 velocity = Vector3.zero ;
    public Weapon shooter = null;
    public float damage = 0;
    public float generatedTime = 0;
    public int age = 0;

    public static LiveBullet BulletInstantiate(
        Weapon shooter,
        Vector3 initialPosition,
        Vector3 initialVelocity, 
        float damage 
    ){
        GameObject bullet = new GameObject()
        {
            name = "bullet" + shooter.sender.ToString() + " " + Time.frameCount.ToString()
        };
        
        bullet.transform.position = initialPosition;

        bullet.AddComponent<LiveBullet>();
        LiveBullet bulletLB = bullet.GetComponent<LiveBullet>();

        bulletLB.initialVelocity = initialVelocity;
        bulletLB.velocity = initialVelocity;
        bulletLB.shooter = shooter;
        bulletLB.damage = damage;
        bulletLB.generatedTime = Time.frameCount;

        return bulletLB;
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float vv = velocity.y;
        vv -= 9.8f * Time.deltaTime;
        velocity.y = vv;
        Vector3 np = transform.position;
        transform.position = new Vector3(np.x + velocity.x, np.y + vv, np.z + velocity.z);
        Debug.DrawRay(np, velocity, Color.yellow, 0.1f);
        if(transform.position.y < -10) {
            Destroy(this.gameObject);
        }
    }
}
