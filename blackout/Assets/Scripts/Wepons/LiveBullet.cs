using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveBullet : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 initialVelocity = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
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
        
    }
}
