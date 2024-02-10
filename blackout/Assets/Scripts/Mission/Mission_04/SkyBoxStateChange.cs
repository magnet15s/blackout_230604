using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxStateChange : MissionEventNode
{
    private static Material skyboxMat = null;
    [SerializeField] Transform sun;
    [SerializeField] Vector3 sunRotateAddition;
    [SerializeField] List<string> setFloatNames;
    [SerializeField] List<float> setFloatAdditions;
    [SerializeField] List<string> setColorNames;
    [SerializeField] List<Color> setColorAdditions;
    [SerializeField] float transitionTime = 5;
    [SerializeField] bool sinSmoothing = false;

    override protected void Awake() {
        base.Awake();
        skyboxMat = new Material(RenderSettings.skybox);
        
        
        RenderSettings.skybox = skyboxMat;

    }
    public override void EventFire() {
        StartCoroutine("Event");
    }

    IEnumerator Event() {
        if(setFloatAdditions.Count != setFloatNames.Count) {
            Debug.LogError("value count missmatch");
            yield break;
        }

        for(float i = 0; i < transitionTime; i += Time.deltaTime) {
            float delta = calcCurrentProgress(i) - calcCurrentProgress(i - Time.deltaTime);

            foreach(var name in setFloatNames) {
                Debug.Log($"{skyboxMat.GetFloat(name)}");
                skyboxMat.SetFloat(name, skyboxMat.GetFloat(name) + setFloatAdditions[setFloatNames.IndexOf(name)] * delta);
            }
            foreach (var name in setColorNames) {
                skyboxMat.SetColor(name, skyboxMat.GetColor(name) + setColorAdditions[setColorNames.IndexOf(name)] * delta);
            }
            sun.localEulerAngles = sun.localEulerAngles + (sunRotateAddition * delta);

            yield return null;
        }
    }

    float calcCurrentProgress(float currentTime) {
        float linerProgress = currentTime / transitionTime;
        return sinSmoothing ? Mathf.Cos((linerProgress + 1) * Mathf.PI) : linerProgress;
    }

}
