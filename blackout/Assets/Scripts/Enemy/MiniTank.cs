using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MiniTank : Enemy {
    public Animator anim;
    public GroundedSensor gg;
    public NavMeshAgent navAgent;
    public GameObject Target;


    // Start is called before the first frame update
    void Start()
    {
        modelName = "MiniTank";
        maxArmorPoint = 200;
        armorPoint = 200;
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
            if (navAgent.pathStatus != NavMeshPathStatus.PathInvalid)
            {

                navAgent.destination = Target.transform.position;
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
