using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{

    private float nRotZ;
    
    // Start is called before the first frame update
    void Start()
    {
        nRotZ = transform.eulerAngles.z;
    }

    // Update is called once per frame
    private void Update()
    {
        
    }
    void LateUpdate()
    {
        Vector3 old = transform.eulerAngles;
        transform.eulerAngles = new Vector3(old.x, old.y, nRotZ);
    }
}
