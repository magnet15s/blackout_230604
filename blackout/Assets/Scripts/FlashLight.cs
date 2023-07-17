using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public Transform illuminateDirRef;
    

    // Start is called before the first frame update
    void Start()
    {
        if(illuminateDirRef == null)illuminateDirRef = transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = illuminateDirRef.eulerAngles;
    }
}
