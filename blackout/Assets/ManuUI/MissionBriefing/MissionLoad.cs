using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionLoad : MonoBehaviour
{
    public static readonly Dictionary<int, string> MISSION_ID = new Dictionary<int, string>() {
        {101, "Mission_01" }
    };

    [SerializeField] private GameObject _loadingUI;
    [SerializeField] private Slider _slider;
    // Start is called before the first frame update
    public void OnSortie() {
        _loadingUI.SetActive(true);
        StartCoroutine(LoadScene());
    }
    IEnumerator LoadScene() {
        string sceneName;
        AsyncOperation async;
        if (MISSION_ID.TryGetValue(mission_select.mission_number, out sceneName)) {
            async = SceneManager.LoadSceneAsync(sceneName);
        } else {
            async = SceneManager.LoadSceneAsync("SampleScene");
        }

         

        while (!async.isDone) {
            _slider.value = async.progress;
            yield return null;
        }
    }
}
