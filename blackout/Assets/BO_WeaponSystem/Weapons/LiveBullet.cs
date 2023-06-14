using System;
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
    //public LineRenderer line;
    //public Vector3[] linePosArray;
    
    public static bool GET_PREFAB = false;
    public static GameObject PR_LIVEBULLET;

    public bool started = false;

    public static LiveBullet BulletInstantiate(
        Weapon _shooter,
        Vector3 _initialPosition,
        Vector3 _initialVelocity, 
        float _damage 
    ){
        if (!GET_PREFAB) {
            PR_LIVEBULLET = ((GameObject)Resources.Load("BO_WeaponSystem/Prefabs/PrLiveBullet")) ?? new GameObject { name = "Bullet Load Error" };
            GET_PREFAB = true;
        }

        GameObject bullet = Instantiate(PR_LIVEBULLET);

        bullet.name += _shooter.sender.ToString() + " " + Time.frameCount.ToString();
        bullet.transform.position = _initialPosition;

        LiveBullet bulletLB = bullet.GetComponent<LiveBullet>();

        bulletLB.initialVelocity = _initialVelocity;
        bulletLB.velocity = _initialVelocity;
        bulletLB.shooter = _shooter;
        bulletLB.damage = _damage;
        bulletLB.generatedTime = Time.frameCount;

        //LineRenderer bulletLR = bullet.GetComponent<LineRenderer>();
        //bulletLR.positionCount = 1;
        //bulletLR.SetPosition(0, _initialPosition);

        bulletLB.started = true;
        return bulletLB;
    }


    void Start()
    {
        //line = GetComponent<LineRenderer>();
        //line.GetPositions(linePosArray);
        //linePosArray = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(false/*started && linePosArray == null*/) {
            //linePosArray = new Vector3[1] {Vector3.zero};
        }
        if (true/*linePosArray != null*/) {

            velocity.y -= 9.8f * Time.deltaTime;
            velocity.y -= velocity.y * 0.4f * Time.deltaTime;
            velocity.x -= velocity.x * 0.4f * Time.deltaTime;
            velocity.z -= velocity.z * 0.4f * Time.deltaTime;
            Vector3 Movement = velocity * Time.deltaTime;
            Vector3 op = transform.position;
            Debug.Log($" {velocity}  {Movement}");
            Vector3 np = op + Movement;
            transform.position = np;
            /*
            int i;
            linePosArray[0] = Vector3.zero;
            for (i = 1; i < linePosArray.Length; i++) {
                linePosArray[i] = linePosArray[i - 1] - Movement;

                if (linePosArray[i].magnitude > 100) {
                    break;
                }

            }

            if (i == linePosArray.Length) {
                Array.Resize(ref linePosArray, linePosArray.Length + 1);
                linePosArray[^1] = linePosArray[^2] - Movement;
            } else {
                Array.Resize(ref linePosArray, i + 1);
            }
            Debug.Log(Movement);
            line.SetPositions(linePosArray);
            line.positionCount = linePosArray.Length;*/


            if (transform.position.y < -10) {
                Destroy(this.gameObject);
            }
        }
    }
}
