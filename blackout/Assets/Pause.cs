using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] GameObject screen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) {
            Time.timeScale = 0;
            screen.SetActive(true);
        }
    }
    public void PauseGame() {
        Time.timeScale = 0;
    }

    public void ResumeGame() {
        Time.timeScale = 1;
        screen.gameObject.SetActive(false);
    }
    public void resurtGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void exitGame() {
        Initiate.Fade("menu_02", Color.black, 3f);
    }
}
