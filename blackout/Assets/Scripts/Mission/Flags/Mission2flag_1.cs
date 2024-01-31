using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mission2flag_1 : MissionEventFlag {
    // Start is called before the first frame update
    public GameObject player;
    private float kyori;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        kyori = (this.gameObject.transform.position - player.transform.position).magnitude;
        if(kyori <= 80) {
            OnFlagUp();
        }
    }
    
    /*private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            OnFlagUp();
        }
    }*/
}
