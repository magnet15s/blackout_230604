using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class PlayerCameraCood : CinemachineExtension
{

    private float neutRotZ = 0;

    private int zoomOutCnt = 0;
    private int zoomOutWaitFlame = 2;
    private float zoomOutTime = 0;
    private bool zoom = false;
    
    private string zoomDebug = "";
    [SerializeField, Range(1, 180)] private float zoomInFOV = 60;
    [SerializeField] private float zoomInOutTime = 0.1f;
    private float zoomOutFOV;
    private float currentZoomVelocity = 0;
    private float currentFOV = -1;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam, 
        CinemachineCore.Stage stage, 
        ref CameraState state, 
        float deltaTime
    ){
        if (zoomOutFOV == -1)
        {
            zoomOutFOV = state.Lens.FieldOfView;
            currentFOV = zoomOutFOV;
        }

        if (stage == CinemachineCore.Stage.Finalize)
        {
            if (zoomOutCnt > 0) zoomOutCnt--;
            else if (zoomOutTime <= 0) zoom = false;

            if (zoomOutTime > 0) zoomOutTime -= Time.deltaTime;
            else if (zoomOutCnt <= 0) zoom = false;

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, neutRotZ);
            //Debug.Log(zoom  +"zoom");
            if (zoom)
            {
                currentFOV = Mathf.SmoothDamp(
                    currentFOV,
                    zoomInFOV,
                    ref currentZoomVelocity,
                    zoomInOutTime,
                    float.MaxValue,
                    deltaTime                    
                );
                state.Lens.FieldOfView = currentFOV;
                Debug.Log(currentFOV);
            }
            else
            {
                currentFOV = Mathf.SmoothDamp(
                    currentFOV,
                    zoomOutFOV,
                    ref currentZoomVelocity,
                    zoomInOutTime,
                    float.MaxValue,
                    deltaTime
                );
                state.Lens.FieldOfView = currentFOV;
            }
            //Debug.Log("z : " + zoomDebug);
            zoomDebug = "";


        }
    }
    public void Zoom()
    {
        zoom = true;
        zoomOutCnt = zoomOutWaitFlame;
    }

    public void Zoom(float time)
    {
        zoom = true;
        zoomOutTime = time;
    }
    // Start is called before the first frame update
    void Start()
    {
        neutRotZ = transform.eulerAngles.z;
        zoomOutFOV = -1;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
