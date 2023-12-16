using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class mission_select : MonoBehaviour
{
    [SerializeField] private GameObject _loadingUI;
    [SerializeField] private Slider _slider;
    public static int mission_number=0;
    // Start is called before the first frame update
    public void OnButtonClick01() {
        mission_number = 1;
        _loadingUI.SetActive(true);
        StartCoroutine(LoadScene());
    }
    public void OnButtonClick02() {
        mission_number = 2;
        _loadingUI.SetActive(true);
        StartCoroutine(LoadScene());
    }
    public void OnButtonClick03() {
        mission_number = 3;
        _loadingUI.SetActive(true);
        StartCoroutine(LoadScene());
    }
    public void OnButtonClick04() {
        mission_number = 4;
        _loadingUI.SetActive(true);
        StartCoroutine(LoadScene());
    }
    public void OnButtonClick05() {
        mission_number = 5;
        _loadingUI.SetActive(true);
        StartCoroutine(LoadScene());
    }
    public void OnClickM01() {
        mission_number = 101;
        _loadingUI.SetActive(true);
        StartCoroutine(LoadScene());
    }
    public void OnButtonClickRe() {
        Initiate.Fade("menu_02", Color.black, 3f);
    }

    public void OnButtonClickInmenu() {
        Initiate.Fade("menu_03", Color.black, 3f);
    }

    IEnumerator LoadScene() {
        AsyncOperation async = SceneManager.LoadSceneAsync("menu_04");

        while (!async.isDone) {
            _slider.value = async.progress;
            yield return null;
        }
    }
}
