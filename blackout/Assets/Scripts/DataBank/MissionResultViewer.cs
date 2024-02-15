using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionResultViewer : MonoBehaviour
{
    [SerializeField] GameObject viewer;
    [SerializeField] Image curtain;
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI missionTitle;
    [SerializeField] TextMeshProUGUI apView;
    [SerializeField] TextMeshProUGUI bestapView;
    [SerializeField] GameObject apNewRecIcon;
    [SerializeField] TextMeshProUGUI timeView;
    [SerializeField] TextMeshProUGUI besttimeView;
    [SerializeField] GameObject timeNewRecIcon;

    public static Dictionary<string, string> missionImagePath = new Dictionary<string, string>{
        { "Mission_01", "missions/image_018_0000" },
        { "Mission_02", "missions/image_026_0003" },
        { "Mission_03", "missions/image_001_0000" },
        { "Mission_04", "missions/image_006_0000" },

    };



    // Start is called before the first frame update
    void Start()
    {
        if (MissionEventNode.missionsAllCleared) {
            MissionEventNode.missionsAllCleared = false;
            viewer.SetActive(true);
            curtain.gameObject.SetActive(true);
            StartCoroutine("CurtainOpen");

            MissionScoreData.ClearDatum lcd = MissionScoreData.lastMissionDatum;
            missionTitle.text = text_load.MISSION_SUBTITLES[lcd.missionName];

            apView.text = ((lcd.bestArmorPoint * 100) / lcd.maxArmorPoint) + "%";
            timeView.text = (lcd.bestClearTime / 60).ToString("F0") + ":" + (lcd.bestClearTime % 60).ToString("F2");

            MissionScoreData.ClearDatum bcd = null;
            if (MissionScoreData.ClearData.Exists(d => d.missionName == lcd.missionName)) {
                bcd = MissionScoreData.ClearData.Find(d => d.missionName .Equals( lcd.missionName));
                bestapView.text = ((bcd.bestArmorPoint * 100) / bcd.maxArmorPoint) + "%";
                besttimeView.text =(bcd.bestClearTime / 60).ToString("F0") +":"+ (bcd.bestClearTime % 60 < 10 ? "0":"") + (bcd.bestClearTime % 60).ToString("F2");
                MissionScoreData.ClearDatum newDatum = bcd;

                if (lcd.bestArmorPoint / lcd.maxArmorPoint > bcd.bestArmorPoint / lcd.maxArmorPoint) {
                    apNewRecIcon.SetActive(true);
                    newDatum = new MissionScoreData.ClearDatum(newDatum.missionName, lcd.bestArmorPoint, lcd.maxArmorPoint, newDatum.bestClearTime);
                }
                if (lcd.bestClearTime < bcd.bestClearTime) {
                    timeNewRecIcon.SetActive(true);
                    newDatum = new MissionScoreData.ClearDatum(newDatum.missionName, newDatum.bestArmorPoint, newDatum.maxArmorPoint, lcd.bestClearTime);
                }

                if (!newDatum.Equals(bcd)) MissionScoreData.UpdateClearData(newDatum);

            } else {
                bestapView.text = "-";
                besttimeView.text = "-";
                MissionScoreData.AddClearData(lcd);
            }


            string bgPath;
            if(missionImagePath.TryGetValue(lcd.missionName, out bgPath)) {
                Sprite bgSpr;
                if ((bgSpr = Resources.Load<Sprite>(bgPath)) != null)
                    background.sprite = bgSpr;
                background.color = new Color(0.6f, 0.6f, 0.6f, 1);
            }
            
                
        }
    }

    IEnumerator CurtainOpen() {
        for(float t = 0; t < 1; t+=0.02f) {
            curtain.color = new Color(0, 0, 0,1-( t / 0.9f));
            yield return new WaitForSeconds(0.02f);
        }
        curtain.gameObject.SetActive(false);
    }

    public void Close() {
        viewer.SetActive(false);
    }
    
}
