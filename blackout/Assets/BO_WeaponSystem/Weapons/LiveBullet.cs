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
    public LineRenderer line;
    public Vector3[] linePosArray;
    
    public static bool GET_PREFAB = false;
    public static GameObject PR_LIVEBULLET;

    public static LiveBullet BulletInstantiate(
        Weapon _shooter,
        Vector3 _initialPosition,
        Vector3 _initialVelocity, 
        float _damage 
    ){
        if (!GET_PREFAB) {
            PR_LIVEBULLET = ((GameObject)Resources.Load("/Assets/BO_WeaponSystem/Prefabs/PrLiveBullet")) ?? new GameObject { name = "Bullet Load Error" };
            GET_PREFAB = true;
        }

        GameObject bullet = Instantiate(PR_LIVEBULLET);

        bullet.name = "bullet" + _shooter.sender.ToString() + " " + Time.frameCount.ToString();
        bullet.transform.position = _initialPosition;

        LiveBullet bulletLB = bullet.GetComponent<LiveBullet>();

        bulletLB.initialVelocity = _initialVelocity;
        bulletLB.velocity = _initialVelocity;
        bulletLB.shooter = _shooter;
        bulletLB.damage = _damage;
        bulletLB.generatedTime = Time.frameCount;

        LineRenderer bulletLR = bullet.GetComponent<LineRenderer>();
        bulletLR.positionCount = 1;
        bulletLR.SetPosition(0, _initialPosition);
        return bulletLB;
    }


    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.GetPositions(linePosArray);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        velocity.y -= 9.8f * Time.deltaTime;
        velocity.x -= velocity.x * 0.9f * Time.deltaTime;
        velocity.z -= velocity.z * 0.9f * Time.deltaTime;
        Vector3 p = transform.position;

        Vector3 np = p + velocity;
        transform.position = np;
        if (new Vector3(Mathf.Abs(linePosArray[0].x - np.x), Mathf.Abs(linePosArray[0].y - np.y), Mathf.Abs(linePosArray[0].z - np.z)).magnitude > 5) {
            
        }


        if(transform.position.y < -10) {
            Destroy(this.gameObject);
        }
    }
}
