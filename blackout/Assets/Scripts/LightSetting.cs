using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSetting : MonoBehaviour
{
    private Material lightingDPSMat;
    [SerializeField] CamPostEffect cpfx;
    [SerializeField] Transform solarLight;
    
    private Material playerCamMat;
    private Material defPlayerCamMat;
    private Vector3 defSolarAng;

    private bool dpsEnable = false;
    private float solarAngle;
    private float solarDirection;
    private float sunlightColorOverride = 0f;
    private Color sunlightColor;

    private float camFXOverride = 0f;
    private float noiseIts;
    private float bloomThres;
    private float bloomIts;
    private float lightness;
    private float contrast;
    // Start is called before the first frame update
    void Start()
    {
        lightingDPSMat = (Material)Resources.Load("DevPrivateSetting/SettingMaterials/Lighting");
        if (lightingDPSMat == null) return;
        playerCamMat = new Material(cpfx.postEffect.shader);
        defPlayerCamMat = cpfx.postEffect;
        cpfx.postEffect = playerCamMat;

        defSolarAng = solarLight.localEulerAngles;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lightingDPSMat == null) return;

        float ud = lightingDPSMat.GetFloat("_UseDPS");
        

        if (ud == 1f)
        {
            //ライト
            float sa = lightingDPSMat.GetFloat("_SolarAngle");
            if(sa != solarAngle || !dpsEnable)
            {
                solarAngle = sa;
                solarLight.localEulerAngles = new Vector3(sa, solarLight.localEulerAngles.y, solarLight.localEulerAngles.z);
            } 

            float sd = lightingDPSMat.GetFloat("_SolarDirection");
            if (sd != solarDirection || !dpsEnable)
            {
                solarDirection = sd;
                Vector3 ea = solarLight.localEulerAngles;
                solarLight.localEulerAngles = new Vector3(solarLight.localEulerAngles.x, sd, solarLight.localEulerAngles.z);
            }

            float slco = lightingDPSMat.GetFloat("_SunlightColorOverride");
            if (slco == 1 || !dpsEnable)
            {

                Color slc = lightingDPSMat.GetColor("_SunlightColor");
                if (!slc.Equals(sunlightColor) || slco != sunlightColorOverride || !dpsEnable)
                {
                    solarLight.GetComponent<Light>().color = slc;
                    sunlightColor = slc;
                }

                sunlightColorOverride = slco;
            }

            //カメラシェーダー
            if (!dpsEnable) cpfx.postEffect = playerCamMat;

            float cfxo = lightingDPSMat.GetFloat("_CamFXOverride");
            if (cfxo == 1 || !dpsEnable)
            {
                float ni = lightingDPSMat.GetFloat("_NoiseIts");
                if(ni != noiseIts || cfxo != camFXOverride || !dpsEnable)
                {
                    playerCamMat.SetFloat("_NoiseIts", ni);
                    noiseIts = ni;
                }
                float bt = lightingDPSMat.GetFloat("_BloomThres");
                if (bt != bloomThres || cfxo != camFXOverride || !dpsEnable)
                {
                    playerCamMat.SetFloat("_BloomThres", bt);
                    bloomThres = bt;
                }
                float bi = lightingDPSMat.GetFloat("_BloomIts");
                if (bi != bloomIts || cfxo != camFXOverride || !dpsEnable)
                {
                    playerCamMat.SetFloat("_BloomIts", bi);
                    bloomIts = bi;
                }
                float ln = lightingDPSMat.GetFloat("_Lightness");
                if (ln != lightness || cfxo != camFXOverride || !dpsEnable)
                {
                    playerCamMat.SetFloat("_Lightness", ln);
                    lightness = ln;
                }
                float ct = lightingDPSMat.GetFloat("_Contrast");
                if (ct != contrast || cfxo != camFXOverride || !dpsEnable)
                {
                    playerCamMat.SetFloat("_Contrast", ct);
                    contrast = ct;
                }

                camFXOverride = cfxo;
            }




            if (dpsEnable == false) dpsEnable = true;
        }
        else
        {
            if (dpsEnable == true) {
                solarLight.localEulerAngles = defSolarAng;
                cpfx.postEffect = defPlayerCamMat;
                dpsEnable = false; 
            }
        }
    }
}
