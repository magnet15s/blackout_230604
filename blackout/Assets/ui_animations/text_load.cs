using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class text_load : MonoBehaviour {
    public TextAsset file;
    public TextMeshProUGUI text;
    public Image image;
    public Sprite sprite;

    void Start() {
        // テキストファイルの全文を取得
        if (mission_select.mission_number == 1) {
            file = Resources.Load("missions/mission01") as TextAsset;
            sprite= Resources.Load<Sprite>("missions/image_005_0000");
        }
        else if(mission_select.mission_number == 2) {
            file = Resources.Load("missions/mission02") as TextAsset;
            sprite = Resources.Load<Sprite>("missions/image_008_0000");
        }
        else if(mission_select.mission_number == 3) {
            file = Resources.Load("missions/mission03") as TextAsset;
            sprite = Resources.Load<Sprite>("missions/image_006_0000");
        }
        else if(mission_select.mission_number == 4) {
            file = Resources.Load("missions/mission04") as TextAsset;
            sprite = Resources.Load<Sprite>("missions/image_007_0000");
        }
        else if (mission_select.mission_number == 5) {
            file = Resources.Load("missions/mission05") as TextAsset;
        }
        
        
        Sprite obj = sprite;
        string loadText = file.text;
        image.sprite = obj;
        text.text = loadText;
    }

    public void OnButtonClickSyuutugeki() {
        if (mission_select.mission_number == 1) {
            Initiate.Fade("world01", Color.black, 3f);
        }
        else if (mission_select.mission_number == 2) {
            Initiate.Fade("world02", Color.black, 3f);
        }
        else if (mission_select.mission_number == 3) {
            Initiate.Fade("world03", Color.black, 3f);
        }
        else if (mission_select.mission_number == 4) {
            Initiate.Fade("world04", Color.black, 3f);
        }
        else if (mission_select.mission_number == 5) {
            Initiate.Fade("world05", Color.black, 3f);
        }
    }
}