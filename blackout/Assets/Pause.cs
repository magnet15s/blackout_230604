using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject screen;
    [SerializeField] GameObject subcam;
    // Start is called before the first frame update
    private bool stop = false;
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneExit;
        CursorSetActive(false);
    }

    void OnSceneExit(Scene s, LoadSceneMode l) {
        CursorSetActive(true);
        Time.timeScale = 1;
        SceneManager.sceneLoaded -= OnSceneExit;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)&&stop==false) {

            CursorSetActive(true);
            Time.timeScale = 0;
            screen.SetActive(true);
            stop= true;
        }else if(Input.GetKeyDown(KeyCode.Escape) && stop == true) {

            CursorSetActive(false);
            Time.timeScale = 1;
            screen.gameObject.SetActive(false);
            stop = false;
        }
    }
    public void PauseGame() {
        Time.timeScale = 0;

        CursorSetActive(true);
    }

    public void ResumeGame() {
        Time.timeScale = 1;
        CursorSetActive(false);
        screen.gameObject.SetActive(false);
    }
    public void resurtGame() {
        Time.timeScale = 1;
        CursorSetActive(false);
        Initiate.Fade(SceneManager.GetActiveScene().name, Color.black, 3f);

    }
    public void exitGame() {
        Time.timeScale = 1;
        StartCoroutine(Exit());
        Initiate.Fade("menu_02", Color.black, 3f);
    }
    IEnumerator Exit() {
        yield return new WaitForSeconds(2.9f);
        if(subcam) subcam.SetActive(false);
    }
    public void CursorSetActive(bool b) {
        if (b) { 
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = b;
    }

}
