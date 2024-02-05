using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.SceneManagement;
using UnityEngine;

public class LiveEMP : MonoBehaviour
{
    public int damage;
    public float age;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        age += Time.deltaTime;
        if (age >= 6) {
            Destroy(this.gameObject);
        }
    }
    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag != "Player") {
            DamageReceiver dr;
            Debug.Log("hit!");

            dr = other.GetComponent<DamageReceiver>();
            Debug.Log("dr:" + dr);
            Vector3 hitPos = other.ClosestPointOnBounds(transform.position);
            dr.Damage(damage, hitPos, gameObject, "EMP");
        }
        
    }
}
