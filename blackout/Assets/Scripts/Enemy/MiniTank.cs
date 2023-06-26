using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MiniTank : Enemy {
    public Animator anim;
    public GroundedSensor gg;
    public NavMeshAgent navAgent;
    public GameObject Target;
    public GameObject gunTurretBone;
    public GameObject gunBone;

    public float sensorRange = 300;
    public bool discoveredTargetShare = true;



    // Start is called before the first frame update
    void Start()
    {
        modelName = "MiniTank";
        if(maxArmorPoint == 0)maxArmorPoint = 200;
        if(armorPoint == 0)armorPoint = 200;

        if (Target == null && Enemy.sharedTarget != null) Target = Enemy.sharedTarget;
    }
    public override void Damage(int damage, Vector3 hitPosition, GameObject source, string damageType) {
        armorPoint -=  damage;
        if(armorPoint <= 0) {
            anim.SetBool("Destroy", true);
        }
    }
    // Update is called once per frame
    void Update()
    {

        if(armorPoint > 0)
        {
            Ray ray = new Ray(transform.position + (Target.transform.position - transform.position).normalized * 2, Target.transform.position - transform.position);
            RaycastHit result;
            Physics.Raycast(ray, out result, sensorRange);
            if (result.transform != null) 
            Debug.Log(result.transform.gameObject.Equals(Target) + " " + result.transform.gameObject);


            if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid && (result.transform != null && result.transform.Equals(Target.transform)))
            {
                if(discoveredTargetShare) Enemy.sharedTargetPosition = result.transform.position;
                navAgent.destination = Target.transform.position;
            }else if(Enemy.sharedTargetPosition != null){
                navAgent.destination = (Vector3)Enemy.sharedTargetPosition;
            }
        }
        else
        {
            if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid)
            {
                navAgent.destination = transform.position;
            }
        }
        
        ///à⁄ìÆ
        
        //navMeshAgentÇÃëÄçÏ
    }
}
