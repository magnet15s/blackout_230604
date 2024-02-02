using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;
using static Unity.Burst.Intrinsics.X86.Avx;

public class HUDMissionList : MonoBehaviour {

    [SerializeField] private bool initializeMissionItem = true;
    [SerializeField] private GameObject missionItemPrefab;
    [SerializeField] private float itemSpacing = 20;
    [SerializeField] private Color clearedItemColor = Color.gray;

    private int lastID = -1;
    private Dictionary<int, RectTransform> missionItems = new();
    private Dictionary<int, bool> isMissionCleared = new();
    private float verticalOffset = 0;
    private void Awake() {
        if (initializeMissionItem) { 
            missionItems.Clear();
            isMissionCleared.Clear();
        }
    }

    /// <summary>
    /// ミッションを追加
    /// </summary>
    /// <param name="itemText"></param>
    /// <returns></returns>
    public int AddMissionItem(string itemText) {
        RectTransform t;
        lastID++;
        while (missionItems.TryGetValue(lastID, out t)) lastID++;
        return AddMissionItem(itemText, lastID);
    }

    /// <summary>
    /// ミッションを追加
    /// </summary>
    /// <param name="itemText"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public int AddMissionItem(string itemText, int id) {
        RectTransform t;
        if(missionItems.TryGetValue(id, out t)) {
            Debug.LogWarning($"[HUDMissionList.MissionClear] > 指定したIDのmissionItemがすでに存在しています（item:{t} id:{id}）");
            return -1;
        }
        RectTransform addItem = Instantiate(missionItemPrefab, transform).GetComponent<RectTransform>();
        missionItems.Add(id, addItem);
        isMissionCleared.Add(id, false);
        TextMeshProUGUI tmp = addItem.GetComponent<TextMeshProUGUI>();
        addItem.anchoredPosition = new Vector2(0, -verticalOffset);

        //height設定
        //縦幅計算
        float lineSpace = tmp.lineSpacing;
        float charSize = tmp.fontSize;
        float lineHeight = charSize + charSize * (lineSpace / 100);
        int lineCnt = 1, textIdx = 0 ;
        //行数計算のついでに整形
        itemText = "-" + itemText;
        while((textIdx = itemText.IndexOf("\n", textIdx)) != -1) {
            if (textIdx < itemText.Length - 2) {
                itemText = itemText.Substring(0, textIdx + 1) + " " + itemText.Substring(textIdx + 1);
            }
            textIdx += 3;
            if (textIdx >= itemText.Length) break;
            lineCnt++;
        }
        tmp.SetText(itemText);

        //height反映
        addItem.sizeDelta = new Vector2(addItem.sizeDelta.x, lineHeight * lineCnt);
        
        //縦オフセット加算
        verticalOffset += addItem.sizeDelta.y + itemSpacing;

        return id;
    }

    /// <summary>
    /// ミッションのテキスト更新
    /// </summary>
    /// <param name="id"></param>
    /// <param name="itemText"></param>
    public void UpdateMissionItemText(int id, string itemText) {
        RectTransform item;
        if(missionItems.TryGetValue(id, out item)) {
            TextMeshProUGUI tmp = item.GetComponent<TextMeshProUGUI>();
            int textIdx = 0;
            itemText = "-" + itemText;
            while ((textIdx = itemText.IndexOf("\n", textIdx)) != -1) {
                if (textIdx < itemText.Length - 2) {
                    itemText = itemText.Substring(0, textIdx + 1) + " " + itemText.Substring(textIdx + 1);
                }
                textIdx += 3;
                if (textIdx >= itemText.Length) break;
            }
            tmp.SetText(itemText);
        } else {
            Debug.LogWarning($"[HUDMissionList.MissionClear] > 指定したIDのmissionItemは存在していません（id:{id}）");

        }
    }
    public void UpdateMissionItemTextNonCal(int id, string itemText) {
        RectTransform item;
        if (missionItems.TryGetValue(id, out item)) {
            item.GetComponent<TextMeshProUGUI>().SetText(itemText);
        } else {
            Debug.LogWarning($"[HUDMissionList.MissionClear] > 指定したIDのmissionItemは存在していません（id:{id}）");

        }
    }

        /// <summary>
        /// ミッションのテキスト取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string getMissionItemText(int id) {
        RectTransform item;
        if (missionItems.TryGetValue(id, out item)) {
            return item.GetComponent<TextMeshProUGUI>().text;
        }else {
            Debug.LogWarning($"[HUDMissionList.MissionClear] > 指定したIDのmissionItemは存在していません（id:{id}）");
            return null;
        }

    }
        void Update() {
        //if (lastID < 3) Debug.Log(AddMissionItem("aiueoおお\nkakikukeko\nhahahaあいう" + (lastID).ToString()));
        //else RemoveMissionItems();
    }

    /// <summary>
    /// ミッションをクリア済みにする
    /// </summary>
    /// <param name="id"></param>
    public void MissionClear(int id) {
        bool c;
        if(isMissionCleared.TryGetValue(id, out c)) {
            if (!c) {
                RectTransform item;
                missionItems.TryGetValue(id, out item);
                item.GetComponent<TextMeshProUGUI>().color = clearedItemColor;
                isMissionCleared[id] = true;
            }
        } else {
            Debug.LogWarning($"[HUDMissionList.MissionClear] > 指定したIDのmissionItemは存在していません（id:{id}）");
        }
    }

    /// <summary>
    /// 全ミッションを消去
    /// </summary>
    public void RemoveMissionItems() {
        foreach(KeyValuePair<int, RectTransform> item in missionItems) {
            Destroy(item.Value.gameObject);
        }
        missionItems.Clear();
        isMissionCleared.Clear();
        verticalOffset = 0;
    }


}
