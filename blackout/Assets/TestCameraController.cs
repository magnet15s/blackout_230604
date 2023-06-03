using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestCameraController : MonoBehaviour
{
    // Start is called before the first frame update

    Transform cam;
    public float speed = 5;
    Vector3 movement;
    void Start()
    {
        cam = this.transform;
    }

    // Update is called once per frame
    void Update()
    {

        //move

        movement.x = 0;
        movement.y = 0;
        movement.z = 0;

        if (Input.GetKey(KeyCode.W))
        {
            movement.z += -1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement.z += 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement.x += -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement.x += 1;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            movement.y += 1;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movement.y += -1;
        }

        movement.Normalize();
        movement *= speed * Time.deltaTime;
        Debug.Log(movement);
        //movement += cam.position;
        //Debug.Log(movement); 
        cam.localPosition += movement;

        //rotate
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            
        }
    }
}
