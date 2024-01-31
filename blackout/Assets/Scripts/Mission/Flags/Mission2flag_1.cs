using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission2flag_1 : MissionEventFlag {
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            OnFlagUp();
        }
    }
}
