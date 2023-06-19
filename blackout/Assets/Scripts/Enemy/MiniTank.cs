using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniTank : Enemy {
    [SerializeField] public override string modelName { get; set; } = "MiniTank";
    [SerializeField] public override int maxArmorPoint { get; set; } = 200;
    [SerializeField] public override int armorPoint { get; set; } = 200;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
