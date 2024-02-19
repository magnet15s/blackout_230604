using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class text_load : MonoBehaviour {
    public TextAsset file;
    public TextAsset file2;
    public TextMeshProUGUI text;
    public TextMeshProUGUI text2;
    public Image image;
    public Sprite sprite;
    [SerializeField] private GameObject _loadingUI;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _title;
    public static Dictionary<string, string> MISSION_SUBTITLES = new Dictionary<string, string>() {
        {"Mission_01", "Mission01 - チュートリアル" },
        {"Mission_02", "Mission02 - 花園の斥候" },
        {"Mission_03", "Mission03 - 蠢動" },
        {"Mission_04", "Mission04 - 朧々たる誘蛾灯" },
    };
    void Start() {
        // テキストファイルの全文を取得
        if (mission_select.mission_number == 1) {
            _title.text = MISSION_SUBTITLES["Mission_01"];
            file = Resources.Load("missions/mission01") as TextAsset;
            file2 = Resources.Load("missions/mission01_e") as TextAsset;
            sprite = Resources.Load<Sprite>("missions/image_018_0000");
        }
        else if(mission_select.mission_number == 2) {
            _title.text = MISSION_SUBTITLES["Mission_02"];
            file = Resources.Load("missions/mission02") as TextAsset;
            file2 = Resources.Load("missions/mission02_e") as TextAsset;
            sprite = Resources.Load<Sprite>("missions/image_026_0003");
        }
        else if(mission_select.mission_number == 3) {
            _title.text = MISSION_SUBTITLES["Mission_03"];
            file = Resources.Load("missions/mission03") as TextAsset;
            file2 = Resources.Load("missions/mission03_e") as TextAsset;
            sprite = Resources.Load<Sprite>("missions/image_001_0000");
        }
        else if(mission_select.mission_number == 4) {
            _title.text = MISSION_SUBTITLES["Mission_04"];
            file = Resources.Load("missions/mission04") as TextAsset;
            file2 = Resources.Load("missions/mission04_e") as TextAsset;
            sprite = Resources.Load<Sprite>("missions/image_006_0000");
        }
        else if (mission_select.mission_number == 5) {
            file = Resources.Load("missions/mission05") as TextAsset;
        }else if (mission_select.mission_number == 101) {
            file = Resources.Load("missions/Mission_01") as TextAsset;
        }
        
        
        Sprite obj = sprite;
        string loadText = file.text;
        string loadText2 = file2.text;
        image.sprite = obj;
        text.text = loadText;
        text2.text = loadText2;
    }

    public void OnButtonClickSyuutugeki() {
        if (mission_select.mission_number == 1) {
            //Initiate.Fade("world01", Color.black, 3f);
            _loadingUI.SetActive(true);
            StartCoroutine(LoadScene1());
        }
        else if (mission_select.mission_number == 2) {
            //Initiate.Fade("world02", Color.black, 3f);
            _loadingUI.SetActive(true);
            StartCoroutine(LoadScene2());
        }
        else if (mission_select.mission_number == 3) {
            //Initiate.Fade("world03", Color.black, 3f);
            _loadingUI.SetActive(true);
            StartCoroutine(LoadScene3());
        }
        else if (mission_select.mission_number == 4) {
            //Initiate.Fade("world04", Color.black, 3f);
            _loadingUI.SetActive(true);
            StartCoroutine(LoadScene4());
        }
        else if (mission_select.mission_number == 5) {
            //Initiate.Fade("world05", Color.black, 3f);
            _loadingUI.SetActive(true);
            StartCoroutine(LoadScene5());
        }
    }

    IEnumerator LoadScene1() {
        AsyncOperation async = SceneManager.LoadSceneAsync("world01");

        while (!async.isDone) {
            _slider.value = async.progress;
            yield return null;
        }
    }
    IEnumerator LoadScene2() {
        AsyncOperation async = SceneManager.LoadSceneAsync("world02");

        while (!async.isDone) {
            _slider.value = async.progress;
            yield return null;
        }
    }
    IEnumerator LoadScene3() {
        AsyncOperation async = SceneManager.LoadSceneAsync("world03");

        while (!async.isDone) {
            _slider.value = async.progress;
            yield return null;
        }
    }
    IEnumerator LoadScene4() {
        AsyncOperation async = SceneManager.LoadSceneAsync("world04");

        while (!async.isDone) {
            _slider.value = async.progress;
            yield return null;
        }
    }
    IEnumerator LoadScene5() {
        AsyncOperation async = SceneManager.LoadSceneAsync("world05");

        while (!async.isDone) {
            _slider.value = async.progress;
            yield return null;
        }
    }
}