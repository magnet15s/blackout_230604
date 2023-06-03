using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class GroundedSensor : MonoBehaviour
{

    public Vector3 sensorOriginOffset = Vector3.zero;
    public float sensorRange = 0.2f;

    public bool layerMask = false; 
    public int catchLayer = 0;

    public bool drawRay = false;

    private Vector3 rayDirection;
    private Ray ray;

    // Start is called before the first frame update
    void Start()
    {
        
        ray = new Ray(transform.position + sensorOriginOffset, -transform.up);
        if (drawRay) Debug.DrawRay(ray.origin, ray.direction * sensorRange, Color.green, Time.deltaTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*ray = new Ray(transform.position + sensorOriginOffset, -transform.up);
        RaycastHit result;
        Debug.Log(Physics.Raycast(ray, out result, sensorRange));
        if (drawRay)Debug.DrawRay(ray.origin, ray.direction * sensorRange, Color.green, Time.deltaTime);*/
    }

    public bool isGrounded()
    {
        ray = new Ray(transform.position + sensorOriginOffset, -transform.up);
        RaycastHit result;
        return Physics.Raycast(ray, out result, sensorRange);
    }
    
    /// <summary>
    /// 接地判定
    /// </summary>
    /// <param name="normal">地面の法線　非接地時はVector3.zeroが入る</param>
    /// <returns></returns>
    public bool isGrounded(out Vector3 normal) {
        ray = new Ray(transform.position + sensorOriginOffset, -transform.up);
        RaycastHit result;
        bool ret = Physics.Raycast(ray, out result, sensorRange);
        if(ret) normal = result.normal;
        else normal = Vector3.zero;
        return ret;
    }
    
}
