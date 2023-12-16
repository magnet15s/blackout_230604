using TMPro;
using UnityEngine;

public class text_load : MonoBehaviour {
    public TextAsset file;
    public TextMeshProUGUI text;

    void Start() {
        // テキストファイルの全文を取得
        if (mission_select.mission_number == 1) {
            file = Resources.Load("missions/mission01") as TextAsset;
        }else if(mission_select.mission_number == 2) {
            file = Resources.Load("missions/mission02") as TextAsset;
        }else if(mission_select.mission_number == 3) {
            file = Resources.Load("missions/mission03") as TextAsset;
        }else if(mission_select.mission_number == 4) {
            file = Resources.Load("missions/mission04") as TextAsset;
        }else if (mission_select.mission_number == 5) {
            file = Resources.Load("missions/mission05") as TextAsset;
        }else if (mission_select.mission_number == 101) {
            file = Resources.Load("missions/Mission_01") as TextAsset;
        }
        
        string loadText = file.text;
        
        text.text = loadText;
    }
}