using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class GroundedSensor : MonoBehaviour
{

    public Vector3 sensorOriginOffset = Vector3.zero;
    public float sensorRange = 0.2f;
    public float sensorRadius = 1; 
    public bool layerMask = false; 
    public int catchLayer = 0;

    public bool drawRay = false;

    private Vector3 rayDirection;
    private Ray ray;
    private Ray[] arRay = new Ray[8];

    public float sleep { get; private set; }
    private bool sensorAns;//スリープ中の解答

    // Start is called before the first frame update
    void Start()
    {

        ray = new Ray(transform.position + sensorOriginOffset, -transform.up);
        /*int i = 0;
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) break;
                //arRay[i] = 
                i++;
            }
        }*/
        if (drawRay) Debug.DrawRay(ray.origin, ray.direction * sensorRange, Color.green, Time.deltaTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(sleep > 0)sleep -= Time.deltaTime;
        if (sleep < 0) sleep = 0;
    }

    public bool isGrounded()
    {
        if (sleep > 0) return sensorAns;
        ray = new Ray(transform.position + sensorOriginOffset, new Vector3(0,-1,0));
        RaycastHit result;
        return Physics.Raycast(ray, out result, sensorRange);
    }
    
    /// <summary>
    /// 接地判定
    /// </summary>
    /// <param name="normal">地面の法線　非接地時はVector3.zeroが入る スリープ中もノーマルは取得可能</param>
    /// 
    /// <returns></returns>
    public bool isGrounded(out Vector3 normal) {
        
        ray = new Ray(transform.position + sensorOriginOffset, new Vector3(0, -1, 0));
        RaycastHit result;
        bool ret = Physics.Raycast(ray, out result, sensorRange);
        if(ret) normal = result.normal;
        else normal = Vector3.zero;
        return sleep > 0 ? sensorAns : ret;
    }
    
    public void Sleep(float time, bool sensorState) {
        sleep = time;
        sensorAns = sensorState;
    }
    public void WakeUp() {
        sleep = 0;
    }
    
}
