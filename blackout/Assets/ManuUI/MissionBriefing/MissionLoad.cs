using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionLoad : MonoBehaviour
{
    public AudioClip sound1;
    AudioSource audioSource;
    public static readonly Dictionary<int, string> MISSION_ID = new Dictionary<int, string>() {
        {1, "Mission_01" },
        {2, "Mission_02" },
        {3, "Mission_03" },
        {4, "Mission_04" }
    };
    public void Start() {
        audioSource = GetComponent<AudioSource>();
    }
    [SerializeField] private GameObject _loadingUI;
    [SerializeField] private Slider _slider;
    // Start is called before the first frame update
    public void OnSortie() {
        audioSource.PlayOneShot(sound1);
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
