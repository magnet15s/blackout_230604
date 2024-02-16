using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class MissionScoreData
{

    static string path = Application.persistentDataPath +  "/MissionScoreData";
    public class ClearDatum {
        public string missionName;
        public int bestArmorPoint;
        public int maxArmorPoint;
        public float bestClearTime;
        public ClearDatum(string name, int ap, int maxap, float time) {
            missionName = name;
            bestArmorPoint = ap;
            maxArmorPoint = maxap;
            bestClearTime = time;
        }
        override public string ToString() {
            return $"{missionName} {bestArmorPoint}/{maxArmorPoint} {(int)bestClearTime % 3600}:{((int)bestClearTime / 60) % 60}:{bestClearTime % 60}.{(bestClearTime * 100) % 100}";
        }
    }

    private static string scoreDataText;

    public static ClearDatum lastMissionDatum = null;
    private static List<ClearDatum> clearData = null;
    public static List<ClearDatum> ClearData {
        get {
            if (clearData == null) LoadClearData();
            return new List<ClearDatum>(clearData);
        }
    }
    public static void AddClearData(ClearDatum datum) {
        if (ClearData.Exists(d => d.missionName == datum.missionName)) {
            Debug.LogWarning("[MissionScoreData] > 追加しようとしたdatumと同じ名前のdatumが存在するため、更新処理を行います : " + datum.missionName);
            UpdateClearData(datum);
            return; 
        }
        Debug.Log("[MissionScoreData] > add score : " + datum);
        //ファイル上のテキストを更新
        using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.GetEncoding("UTF-8"))) {
            sw.WriteLine($"{datum.missionName} {datum.bestArmorPoint} {datum.maxArmorPoint} {datum.bestClearTime}");
        }
        scoreDataText += $"\n{datum.missionName} {datum.bestArmorPoint} {datum.maxArmorPoint} {datum.bestClearTime}";
        //リストに追加
        clearData.Add(datum);
    }

    public static void UpdateClearData(ClearDatum datum) {
        Debug.Log("[MissionScoreData] > update data : " + datum.ToString() + "\n->\n" + scoreDataText);

        //ファイル上のテキストを更新
        using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.GetEncoding("UTF-8"))) {
            
            int datumIdx = scoreDataText.IndexOf(Environment.NewLine + datum.missionName) + 1;
            if(datumIdx == -1) { Debug.LogError("[MissionScoreData] > misionName is not found : " + datum.missionName); return; }

            int datumLength = scoreDataText.Substring(datumIdx).IndexOf(Environment.NewLine) + 1;
            
            scoreDataText = scoreDataText.Substring(0, datumIdx)
                + $"{datum.missionName} {datum.bestArmorPoint} {datum.maxArmorPoint} {datum.bestClearTime}"
                + (datumLength != -1 ? Environment.NewLine + scoreDataText.Substring(datumIdx + datumLength) : "");

            sw.Write(scoreDataText);            
        }

        //リストを更新
        if(ClearData.Exists((d) => d.missionName == datum.missionName)) {
            ClearDatum cd = clearData.Find(d => d.missionName == datum.missionName);
            cd.bestArmorPoint = datum.bestArmorPoint;
            cd.maxArmorPoint = datum.bestArmorPoint;
            cd.bestClearTime = datum.bestClearTime;
        }
    }

    private static void LoadClearData() {
        if (!File.Exists(path)) {
            Debug.LogWarning("[MissionScoreData] > file not found, file create a new");
            File.CreateText(path);
        }

        using (StreamReader sr = new StreamReader(path, System.Text.Encoding.GetEncoding("UTF-8"))) {
            clearData = new(); 
            clearData.Clear();
            scoreDataText = "";
            while(sr.Peek() > -1) {
                
                string md = sr.ReadLine();
                Debug.Log("read : " + md);
                if (md == "\n") continue;
                scoreDataText += md + Environment.NewLine; 
                string[] dParts = md.Split(' ');
                if (dParts.Length < 4) {
                    Debug.LogError("[MissionScoreData] > data error : "+ md);
                    continue;
                }

                try {
                    clearData.Add(new ClearDatum(dParts[0], int.Parse(dParts[1]), int.Parse(dParts[2]), float.Parse(dParts[3])));
                } catch (Exception e) {
                    Debug.LogError("[MissionScoreData] > file parse phase error : " + e);
                }

            }
        }

    }


    
}