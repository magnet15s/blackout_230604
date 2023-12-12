using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class text_load : MonoBehaviour {
    public TextAsset file;
    public TextMeshProUGUI text;
    public Image image;
    public Sprite sprite;
    [SerializeField] private GameObject _loadingUI;
    [SerializeField] private Slider _slider;

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