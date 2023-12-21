using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scenechange : MonoBehaviour

{
    public AudioClip sound1;
    AudioSource audioSource;
    string Sname;
    // Start is called before the first frame update
    void Start()
    {
        Sname = SceneManager.GetActiveScene().name;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick() {
        if (Sname == "menu_01") {
            Initiate.Fade("menu_02", Color.black, 1.0f);
            audioSource.PlayOneShot(sound1);
        }
    }
}
