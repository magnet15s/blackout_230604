using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class desttest : MonoBehaviour
{
    // Start is called before the first frame update
    
    public void Damage() {
        Vector3 v = transform.position;
        v.x += 1;
        transform.position = v;
    }

    public void Dest() {
        Vector3 v = transform.position;
        v.y += 10;
        transform.position = v;
        //Destroy(gameObject);
    }
}
