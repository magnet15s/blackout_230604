using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIBGM : MonoBehaviour
{
    // Start is called before the first frame update
    private static bool isLoad = false;// 自身がすでにロードされているかを判定するフラグ
    private void Awake() {
        
        
        if (isLoad==true) { // すでにロードされていたら
            Destroy(this.gameObject); // 自分自身を破棄して終了
            return;
        }
        else {
            isLoad = true;
        }
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "menu_03") {
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        else if (SceneManager.GetActiveScene().name == "menu_04") {
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        else if (isLoad == true) {
            isLoad = false;
            Destroy(this.gameObject); // 自分自身を破棄して終了
            
            return;
        }
    }
}
