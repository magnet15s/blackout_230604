using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCMExtension : CinemachineExtension
{

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime
    ){
        Debug.Log(stage + " " + Time.frameCount);
        if(stage == CinemachineCore.Stage.Finalize)
        {
            state.Lens.FieldOfView = Mathf.Clamp(Time.frameCount / 10, 1, 180);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }


}
