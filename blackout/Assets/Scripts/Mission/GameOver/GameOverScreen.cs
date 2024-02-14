using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour {
    public static GameOverScreen screen;
    [SerializeField] TextMeshProUGUI messageBox;
    [SerializeField] Pause pause;
    [SerializeField] Image blackCurtain;
    [SerializeField] float CurtainCloseTime = 2;
    [SerializeField] Color CurtainColor = Color.black;
    [SerializeField] Image background;

    private void Awake() {
        screen = this;
    }

    public void Show(string message) {
        messageBox.text = message;
        pause.CursorSetActive(true);
        screen.gameObject.SetActive(true);
        StartCoroutine("BackShow");
        Time.timeScale = 0;
    }
    
    IEnumerator BackShow() {
        for(float t = 0; t < CurtainCloseTime; t += 0.02f) {
            background.color = new Color(0.7f, 0.7f, 0.7f, t * 1.4f / CurtainCloseTime);
            yield return new WaitForSecondsRealtime(0.02f);
        }
    }

    public void Restart() {
        Debug.Log("restart");
        pause.CursorSetActive(false);
        nextSceneName = SceneManager.GetActiveScene().name;
        StartCoroutine("OnSceneChange1");

    }

    public void Exit() {
        StartCoroutine("OnSceneChange2");
    }

    private string nextSceneName = "menu_02";
    
    private IEnumerator OnSceneChange1() {
        blackCurtain.color = CurtainColor;
        blackCurtain.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(CurtainCloseTime);
        Initiate.Fade(SceneManager.GetActiveScene().name, Color.black, 3f);
        Debug.Log(123);
        SceneManager.LoadScene(nextSceneName, 0);
        yield return null;
    }

    private IEnumerator OnSceneChange2() {
        blackCurtain.gameObject.SetActive(true);

        for (float t = 0; t < CurtainCloseTime; t += 0.02f) {
            blackCurtain.color = new Color(CurtainColor.r, CurtainColor.g, CurtainColor.b, (t*1.2f) / CurtainCloseTime);
            yield return new WaitForSecondsRealtime(0.02f);
        }
        SceneManager.LoadScene(nextSceneName, 0);
        yield return null;

    }
}

    
