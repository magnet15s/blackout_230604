using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class PlayerCameraCood : CinemachineExtension
{

    private float neutRotZ = 0;

    public bool zoom = false;
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
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, neutRotZ);
            
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
            
            
        }
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
