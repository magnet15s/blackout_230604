using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class NavTest : MonoBehaviour
{
    public NavMeshAgent _nma;
    public Transform _target;
    //int age = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_nma.pathStatus != NavMeshPathStatus.PathInvalid)
            _nma.destination = _target.transform.position;
        else Debug.Log("ijijij");
        //Debug.Log($"{_nma.hasPath}  {++age}");
        //_nma.hasPath;
    }
}
