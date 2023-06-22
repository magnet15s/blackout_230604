using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniTank : Enemy {
    public Animator anim;


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
        
    }
}
