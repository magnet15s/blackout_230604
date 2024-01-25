using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDMissionDisplay : MonoBehaviour
{
    public static HUDMissionDisplay mainDisplay { get; private set; }

    [SerializeField] private TextMeshProUGUI missionTitle;
    [SerializeField] private HUDMissionList missionList;
    public HUDMissionList GetMissionList() { return missionList; }

    public void SetMissionTitle(string text) {
        missionTitle.text = "Misssion\n" + text;
    }
    public void SetMissionTitle(string subText, string topText) {
        missionTitle.text = topText + subText;
    }

    private void Awake() {
        if (mainDisplay == null) mainDisplay = this;
    }

}
