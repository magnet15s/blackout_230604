using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scenechange : MonoBehaviour

{
    string Sname;
    // Start is called before the first frame update
    void Start()
    {
        Sname = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick() {
        if (Sname == "menu_01") {
            Initiate.Fade("menu_02", Color.black, 1.0f);
        }
        else if(Sname=="menu_02"){
            Initiate.Fade("menu_01", Color.black, 1.0f);
        }else if (Sname == "menu_03") {
            Initiate.Fade("menu_01", Color.black, 1.0f);
        }
    }
}
